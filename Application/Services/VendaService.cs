using FluentValidation;
using Microsoft.Extensions.Logging;
using SimpleSalesAPI.Application.Dtos.Requests;
using SimpleSalesAPI.Application.Dtos.Responses;
using SimpleSalesAPI.Application.Services.Interfaces;
using SimpleSalesAPI.Domain.Enums;
using SimpleSalesAPI.Domain.Exceptions;
using SimpleSalesAPI.Domain.Models;
using SimpleSalesAPI.Infrastructure.Data.Repositories.Interfaces;
using InvalidOperationException = SimpleSalesAPI.Domain.Exceptions.InvalidOperationException;

namespace SimpleSalesAPI.Application.Services
{
	public class VendaService : IVendaService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CriarVendaRequest> _criarVendaValidator;
		private readonly ILogger<VendaService> _logger;

		public VendaService(
			IUnitOfWork unitOfWork,
			IValidator<CriarVendaRequest> criarVendaValidator,
			ILogger<VendaService> logger)
		{
			_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
			_criarVendaValidator = criarVendaValidator ?? throw new ArgumentNullException(nameof(criarVendaValidator));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<VendaResponse> CriarVendaAsync(CriarVendaRequest request)
		{
			using var activity = _logger.BeginScope(new Dictionary<string, object>
			{
				["Operation"] = nameof(CriarVendaAsync),
				["ClienteId"] = request.ClienteId,
				["QuantidadeItens"] = request.Itens?.Count ?? 0
			});

			_logger.LogInformation("Iniciando criação de venda para cliente {ClienteId} com {QuantidadeItens} itens",
				request.ClienteId, request.Itens?.Count ?? 0);

			var validationResult = await _criarVendaValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				_logger.LogWarning("Validação falhou para criação de venda. Erros: {@ValidationErrors}",
					validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

				throw new ValidationException(validationResult.Errors);
			}

			var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(request.ClienteId);
			if (cliente == null)
			{
				_logger.LogWarning("Cliente {ClienteId} não encontrado para criação de venda", request.ClienteId);
				throw new NotFoundException("Cliente", request.ClienteId);
			}

			var produtoIds = request.Itens.Select(i => i.ProdutoId).Distinct().ToList();

			_logger.LogDebug("Verificando disponibilidade de {QuantidadeProdutos} produtos: {@ProdutoIds}",
				produtoIds.Count, produtoIds);

			var produtos = await _unitOfWork.Repository<Produto>()
				.GetFilterAsync(p => produtoIds.Contains(p.Id) && p.Ativo, tracking: true);

			var produtosList = produtos.ToList();
			if (produtosList.Count != produtoIds.Count)
			{
				var produtosEncontrados = produtosList.Select(p => p.Id);
				var produtosNaoEncontrados = produtoIds.Except(produtosEncontrados);

				_logger.LogWarning("Produtos não encontrados ou inativos: {@ProdutosNaoEncontrados}",
					produtosNaoEncontrados);

				throw new BusinessException("Um ou mais produtos são inválidos ou inativos");
			}

			var venda = new Venda
			{
				ClienteId = request.ClienteId,
				DataVenda = DateTime.UtcNow,
				Status = StatusVenda.Pendente,
				Itens = new List<ItemVenda>()
			};

			decimal valorTotal = 0;
			var estoqueInsuficiente = new List<(string Nome, int Solicitado, int Disponivel)>();

			foreach (var itemRequest in request.Itens)
			{
				var produto = produtosList.First(p => p.Id == itemRequest.ProdutoId);

				if (produto.EstoqueAtual < itemRequest.Quantidade)
				{
					estoqueInsuficiente.Add((produto.Nome, itemRequest.Quantidade, produto.EstoqueAtual));
					continue;
				}

				var item = new ItemVenda
				{
					ProdutoId = produto.Id,
					Quantidade = itemRequest.Quantidade,
					PrecoUnitario = produto.Preco
				};

				produto.EstoqueAtual -= itemRequest.Quantidade;
				_unitOfWork.Repository<Produto>().Update(produto);

				valorTotal += item.Subtotal;
				venda.Itens.Add(item);

				_logger.LogDebug("Item adicionado à venda: Produto {ProdutoId} ({ProdutoNome}), " +
							   "Quantidade: {Quantidade}, Preço: {PrecoUnitario:C}, Subtotal: {Subtotal:C}",
					produto.Id, produto.Nome, item.Quantidade, item.PrecoUnitario, item.Subtotal);
			}

			if (estoqueInsuficiente.Any())
			{
				_logger.LogWarning("Estoque insuficiente para produtos: {@EstoqueInsuficiente}",
					estoqueInsuficiente);

				var primeiro = estoqueInsuficiente.First();
				throw new InsufficientStockException(primeiro.Nome, primeiro.Solicitado, primeiro.Disponivel);
			}

			venda.ValorTotal = valorTotal;

			_logger.LogInformation("Salvando venda com valor total {ValorTotal:C} e {QuantidadeItens} itens",
				valorTotal, venda.Itens.Count);

			await _unitOfWork.Repository<Venda>().AddAsync(venda);
			await _unitOfWork.CommitAsync();

			_logger.LogInformation("Venda {VendaId} criada com sucesso para cliente {ClienteId}. " +
								 "Valor total: {ValorTotal:C}",
				venda.Id, request.ClienteId, valorTotal);

			return await ObterVendaPorIdAsync(venda.Id) ??
				   throw new BusinessException("Erro ao recuperar venda criada");
		}

		public async Task<VendaResponse?> ObterVendaPorIdAsync(int id)
		{
			_logger.LogDebug("Buscando venda {VendaId}", id);

			var vendas = await _unitOfWork.Repository<Venda>()
				.GetFilterAsync(v => v.Id == id, tracking: false,
					v => v.Cliente,
					v => v.Itens);

			var venda = vendas.FirstOrDefault();
			if (venda == null)
			{
				_logger.LogWarning("Venda {VendaId} não encontrada", id);
				return null;
			}

			var produtoIds = venda.Itens.Select(i => i.ProdutoId).ToList();
			var produtos = await _unitOfWork.Repository<Produto>()
				.GetFilterAsync(p => produtoIds.Contains(p.Id), tracking: false);
			var produtoDict = produtos.ToDictionary(p => p.Id, p => p.Nome);

			_logger.LogDebug("Venda {VendaId} encontrada com {QuantidadeItens} itens",
				id, venda.Itens.Count);

			return new VendaResponse
			{
				Id = venda.Id,
				Cliente = new ClienteResumoResponse
				{
					Id = venda.Cliente.Id,
					Nome = venda.Cliente.Nome,
					Email = venda.Cliente.Email
				},
				DataVenda = venda.DataVenda,
				ValorTotal = venda.ValorTotal,
				Status = venda.Status.ToString(),
				Itens = venda.Itens.Select(i => new ItemVendaResponse
				{
					Id = i.Id,
					ProdutoId = i.ProdutoId,
					ProdutoNome = produtoDict.GetValueOrDefault(i.ProdutoId, "Produto não encontrado"),
					Quantidade = i.Quantidade,
					PrecoUnitario = i.PrecoUnitario,
					Subtotal = i.Subtotal
				}).ToList()
			};
		}

		public async Task<List<VendaResponse>> ListarVendasAsync()
		{
			_logger.LogDebug("Listando todas as vendas");

			var vendas = await _unitOfWork.Repository<Venda>()
				.GetFilterAsync(v => true, tracking: false,
					v => v.Cliente,
					v => v.Itens);

			var result = new List<VendaResponse>();
			foreach (var venda in vendas)
			{
				var vendaResponse = await ObterVendaPorIdAsync(venda.Id);
				if (vendaResponse != null)
					result.Add(vendaResponse);
			}

			_logger.LogInformation("Encontradas {QuantidadeVendas} vendas", result.Count);
			return result;
		}

		public async Task<List<VendaResponse>> ListarVendasPorClienteAsync(int clienteId)
		{
			using var activity = _logger.BeginScope(new Dictionary<string, object>
			{
				["Operation"] = nameof(ListarVendasPorClienteAsync),
				["ClienteId"] = clienteId
			});

			_logger.LogDebug("Listando vendas do cliente {ClienteId}", clienteId);

			var vendas = await _unitOfWork.Repository<Venda>()
				.GetFilterAsync(v => v.ClienteId == clienteId, tracking: false,
					v => v.Cliente,
					v => v.Itens);

			var result = new List<VendaResponse>();
			foreach (var venda in vendas)
			{
				var vendaResponse = await ObterVendaPorIdAsync(venda.Id);
				if (vendaResponse != null)
					result.Add(vendaResponse);
			}

			_logger.LogInformation("Cliente {ClienteId} possui {QuantidadeVendas} vendas",
				clienteId, result.Count);
			return result;
		}

		public async Task<List<VendaResponse>> ListarVendasPorStatusAsync(StatusVenda status)
		{
			using var activity = _logger.BeginScope(new Dictionary<string, object>
			{
				["Operation"] = nameof(ListarVendasPorStatusAsync),
				["Status"] = status.ToString()
			});

			_logger.LogDebug("Listando vendas com status {Status}", status);

			var vendas = await _unitOfWork.Repository<Venda>()
				.GetFilterAsync(v => v.Status == status, tracking: false,
					v => v.Cliente,
					v => v.Itens);

			var result = new List<VendaResponse>();
			foreach (var venda in vendas)
			{
				var vendaResponse = await ObterVendaPorIdAsync(venda.Id);
				if (vendaResponse != null)
					result.Add(vendaResponse);
			}

			_logger.LogInformation("Encontradas {QuantidadeVendas} vendas com status {Status}",
				result.Count, status);
			return result;
		}

		public async Task<List<VendaResponse>> ListarVendasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
		{
			using var activity = _logger.BeginScope(new Dictionary<string, object>
			{
				["Operation"] = nameof(ListarVendasPorPeriodoAsync),
				["DataInicio"] = dataInicio,
				["DataFim"] = dataFim
			});

			_logger.LogDebug("Listando vendas entre {DataInicio:yyyy-MM-dd} e {DataFim:yyyy-MM-dd}",
				dataInicio, dataFim);

			var vendas = await _unitOfWork.Repository<Venda>()
				.GetFilterAsync(v => v.DataVenda >= dataInicio && v.DataVenda <= dataFim, tracking: false,
					v => v.Cliente,
					v => v.Itens);

			var result = new List<VendaResponse>();
			decimal valorTotalPeriodo = 0;

			foreach (var venda in vendas)
			{
				var vendaResponse = await ObterVendaPorIdAsync(venda.Id);
				if (vendaResponse != null)
				{
					result.Add(vendaResponse);
					valorTotalPeriodo += vendaResponse.ValorTotal;
				}
			}

			_logger.LogInformation("Período {DataInicio:yyyy-MM-dd} a {DataFim:yyyy-MM-dd}: " +
								 "{QuantidadeVendas} vendas, valor total {ValorTotal:C}",
				dataInicio, dataFim, result.Count, valorTotalPeriodo);
			return result;
		}

		public async Task ConfirmarVendaAsync(int id)
		{
			using var activity = _logger.BeginScope(new Dictionary<string, object>
			{
				["Operation"] = nameof(ConfirmarVendaAsync),
				["VendaId"] = id
			});

			_logger.LogInformation("Confirmando venda {VendaId}", id);

			var venda = await _unitOfWork.Repository<Venda>().GetByIdAsync(id);
			if (venda == null)
			{
				_logger.LogWarning("Tentativa de confirmar venda inexistente {VendaId}", id);
				throw new NotFoundException("Venda", id);
			}

			if (venda.Status != StatusVenda.Pendente)
			{
				_logger.LogWarning("Tentativa de confirmar venda {VendaId} com status {StatusAtual}",
					id, venda.Status);
				throw new InvalidOperationException(
					venda.Status.ToString(),
					"Confirmar",
					"Apenas vendas pendentes podem ser confirmadas");
			}

			venda.Status = StatusVenda.Confirmada;
			_unitOfWork.Repository<Venda>().Update(venda);
			await _unitOfWork.CommitAsync();

			_logger.LogInformation("Venda {VendaId} confirmada com sucesso", id);
		}

		public async Task CancelarVendaAsync(int id)
		{
			using var activity = _logger.BeginScope(new Dictionary<string, object>
			{
				["Operation"] = nameof(CancelarVendaAsync),
				["VendaId"] = id
			});

			_logger.LogInformation("Iniciando cancelamento da venda {VendaId}", id);

			var vendas = await _unitOfWork.Repository<Venda>()
				.GetFilterAsync(v => v.Id == id, tracking: true, v => v.Itens);
			var venda = vendas.FirstOrDefault();

			if (venda == null)
			{
				_logger.LogWarning("Tentativa de cancelar venda inexistente {VendaId}", id);
				throw new NotFoundException("Venda", id);
			}

			if (venda.Status == StatusVenda.Entregue)
			{
				_logger.LogWarning("Tentativa de cancelar venda já entregue {VendaId}", id);
				throw new InvalidOperationException(
					StatusVenda.Entregue.ToString(),
					"Cancelar",
					"Não é possível cancelar uma venda já entregue");
			}

			_logger.LogInformation("Revertendo estoque para {QuantidadeItens} itens da venda {VendaId}",
				venda.Itens.Count, id);

			foreach (var item in venda.Itens)
			{
				var produto = await _unitOfWork.Repository<Produto>().GetByIdAsync(item.ProdutoId);
				if (produto != null)
				{
					var estoqueAnterior = produto.EstoqueAtual;
					produto.EstoqueAtual += item.Quantidade;
					_unitOfWork.Repository<Produto>().Update(produto);

					_logger.LogDebug("Estoque do produto {ProdutoId} revertido: {EstoqueAnterior} -> {EstoqueNovo}",
						produto.Id, estoqueAnterior, produto.EstoqueAtual);
				}
			}

			venda.Status = StatusVenda.Cancelada;
			_unitOfWork.Repository<Venda>().Update(venda);
			await _unitOfWork.CommitAsync();

			_logger.LogInformation("Venda {VendaId} cancelada com sucesso. " +
								 "Estoque revertido para {QuantidadeItens} produtos",
				id, venda.Itens.Count);
		}

		public async Task EntregarVendaAsync(int id)
		{
			using var activity = _logger.BeginScope(new Dictionary<string, object>
			{
				["Operation"] = nameof(EntregarVendaAsync),
				["VendaId"] = id
			});

			_logger.LogInformation("Marcando venda {VendaId} como entregue", id);

			var venda = await _unitOfWork.Repository<Venda>().GetByIdAsync(id);
			if (venda == null)
			{
				_logger.LogWarning("Tentativa de entregar venda inexistente {VendaId}", id);
				throw new NotFoundException("Venda", id);
			}

			if (venda.Status != StatusVenda.Confirmada)
			{
				_logger.LogWarning("Tentativa de entregar venda {VendaId} com status {StatusAtual}",
					id, venda.Status);
				throw new InvalidOperationException(
					venda.Status.ToString(),
					"Entregar",
					"Apenas vendas confirmadas podem ser entregues");
			}

			venda.Status = StatusVenda.Entregue;
			_unitOfWork.Repository<Venda>().Update(venda);
			await _unitOfWork.CommitAsync();

			_logger.LogInformation("Venda {VendaId} marcada como entregue com sucesso", id);
		}

		public async Task ExcluirVendaAsync(int id)
		{
			using var activity = _logger.BeginScope(new Dictionary<string, object>
			{
				["Operation"] = nameof(ExcluirVendaAsync),
				["VendaId"] = id
			});

			_logger.LogWarning("Excluindo venda {VendaId}", id);

			var vendas = await _unitOfWork.Repository<Venda>()
				.GetFilterAsync(v => v.Id == id, tracking: true, v => v.Itens);
			var venda = vendas.FirstOrDefault();

			if (venda == null)
			{
				_logger.LogWarning("Tentativa de excluir venda inexistente {VendaId}", id);
				throw new NotFoundException("Venda", id);
			}

			if (venda.Status == StatusVenda.Confirmada || venda.Status == StatusVenda.Entregue)
			{
				_logger.LogWarning("Tentativa de excluir venda {VendaId} com status {Status}",
					id, venda.Status);
				throw new BusinessException("Não é possível excluir vendas confirmadas ou entregues");
			}

			// Se venda estava pendente, reverter estoque
			if (venda.Status == StatusVenda.Pendente)
			{
				_logger.LogInformation("Revertendo estoque antes da exclusão da venda {VendaId}", id);

				foreach (var item in venda.Itens)
				{
					var produto = await _unitOfWork.Repository<Produto>().GetByIdAsync(item.ProdutoId);
					if (produto != null)
					{
						produto.EstoqueAtual += item.Quantidade;
						_unitOfWork.Repository<Produto>().Update(produto);

						_logger.LogDebug("Estoque do produto {ProdutoId} revertido em {Quantidade} unidades",
							produto.Id, item.Quantidade);
					}
				}
			}

			_unitOfWork.Repository<Venda>().Remove(venda);
			await _unitOfWork.CommitAsync();

			_logger.LogWarning("Venda {VendaId} (valor {ValorTotal:C}) excluída permanentemente",
				id, venda.ValorTotal);
		}
	}
}