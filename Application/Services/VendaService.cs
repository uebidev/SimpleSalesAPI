using FluentValidation;
using SimpleSalesAPI.Application.Dtos.Requests;
using SimpleSalesAPI.Application.Dtos.Responses;
using SimpleSalesAPI.Application.Services.Interfaces;
using SimpleSalesAPI.Domain.Enums;
using SimpleSalesAPI.Domain.Exceptions;
using SimpleSalesAPI.Domain.Models;
using SimpleSalesAPI.Infrastructure.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Services
{
	public class VendaService(IUnitOfWork unitOfWork, IValidator<CriarVendaRequest> criarVendaValidator) : IVendaService
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		private readonly IValidator<CriarVendaRequest> _criarVendaValidator = criarVendaValidator ?? throw new ArgumentNullException(nameof(criarVendaValidator));
		public async Task<VendaResponse> CriarVendaAsync(CriarVendaRequest request)
		{
			var validationResult = await _criarVendaValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
				throw new ValidationException(validationResult.Errors);

			var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(request.ClienteId) ??
				throw new NotFoundException($"Cliente com ID {request.ClienteId} não encontrado");

			if (!request.Itens.Any())
				throw new BusinessException("Venda deve conter pelo menos um item");

			// Validação dos produtos e estoque
			var produtoIds = request.Itens.Select(i => i.ProdutoId).Distinct().ToList();
			var produtos = await _unitOfWork.Repository<Produto>()
				.GetFilterAsync(p => produtoIds.Contains(p.Id) && p.Ativo, tracking: true);

			var produtosList = produtos.ToList();
			if (produtosList.Count != produtoIds.Count)
				throw new BusinessException("Um ou mais produtos são inválidos ou inativos");

			// Criar a venda
			var venda = new Venda
			{
				ClienteId = request.ClienteId,
				DataVenda = DateTime.UtcNow,
				Status = StatusVenda.Pendente,
				Itens = []
			};

			decimal valorTotal = 0;

			foreach (var itemRequest in request.Itens)
			{
				var produto = produtosList.First(p => p.Id == itemRequest.ProdutoId);

				// Validação de estoque
				if (produto.EstoqueAtual < itemRequest.Quantidade)
					throw new InsufficientStockException(produto.Nome, itemRequest.Quantidade, produto.EstoqueAtual);

				var item = new ItemVenda
				{
					ProdutoId = produto.Id,
					Quantidade = itemRequest.Quantidade,
					PrecoUnitario = produto.Preco
				};

				// Atualizar estoque
				produto.EstoqueAtual -= itemRequest.Quantidade;
				_unitOfWork.Repository<Produto>().Update(produto);

				valorTotal += item.Subtotal;
				venda.Itens.Add(item);
			}

			venda.ValorTotal = valorTotal;

			await _unitOfWork.Repository<Venda>().AddAsync(venda);
			await _unitOfWork.CommitAsync();

			return await ObterVendaPorIdAsync(venda.Id) ?? throw new BusinessException("Erro ao recuperar venda criada");
		}

		public async Task<VendaResponse?> ObterVendaPorIdAsync(int id)
		{
			var vendas = await _unitOfWork.Repository<Venda>()
				.GetFilterAsync(v => v.Id == id, tracking: false,
					v => v.Cliente,
					v => v.Itens);

			var venda = vendas.FirstOrDefault();
			if (venda == null) return null;

			// Buscar produtos dos itens para ter nomes
			var produtoIds = venda.Itens.Select(i => i.ProdutoId).ToList();
			var produtos = await _unitOfWork.Repository<Produto>()
				.GetFilterAsync(p => produtoIds.Contains(p.Id), tracking: false);
			var produtoDict = produtos.ToDictionary(p => p.Id, p => p.Nome);

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

			return result;
		}

		public async Task<List<VendaResponse>> ListarVendasPorClienteAsync(int clienteId)
		{
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

			return result;
		}

		public async Task<List<VendaResponse>> ListarVendasPorStatusAsync(StatusVenda status)
		{
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

			return result;
		}

		public async Task<List<VendaResponse>> ListarVendasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
		{
			var vendas = await _unitOfWork.Repository<Venda>()
				.GetFilterAsync(v => v.DataVenda >= dataInicio && v.DataVenda <= dataFim, tracking: false,
					v => v.Cliente,
					v => v.Itens);

			var result = new List<VendaResponse>();
			foreach (var venda in vendas)
			{
				var vendaResponse = await ObterVendaPorIdAsync(venda.Id);
				if (vendaResponse != null)
					result.Add(vendaResponse);
			}

			return result;
		}

		public async Task ConfirmarVendaAsync(int id)
		{
			var venda = await _unitOfWork.Repository<Venda>().GetByIdAsync(id) ??
				throw new NotFoundException($"Venda com ID {id} não encontrada");

			if (venda.Status != StatusVenda.Pendente)
				throw new BusinessException("Apenas vendas pendentes podem ser confirmadas");

			venda.Status = StatusVenda.Confirmada;
			_unitOfWork.Repository<Venda>().Update(venda);
			await _unitOfWork.CommitAsync();
		}

		public async Task CancelarVendaAsync(int id)
		{
			var vendas = await _unitOfWork.Repository<Venda>()
				.GetFilterAsync(v => v.Id == id, tracking: true, v => v.Itens);

			var venda = vendas.FirstOrDefault() ??
				throw new NotFoundException($"Venda com ID {id} não encontrada");

			if (venda.Status == StatusVenda.Entregue)
				throw new BusinessException("Não é possível cancelar uma venda já entregue");

			// Devolver estoque
			foreach (var item in venda.Itens)
			{
				var produto = await _unitOfWork.Repository<Produto>().GetByIdAsync(item.ProdutoId);
				if (produto != null)
				{
					produto.EstoqueAtual += item.Quantidade;
					_unitOfWork.Repository<Produto>().Update(produto);
				}
			}

			venda.Status = StatusVenda.Cancelada;
			_unitOfWork.Repository<Venda>().Update(venda);
			await _unitOfWork.CommitAsync();
		}

		public async Task EntregarVendaAsync(int id)
		{
			var venda = await _unitOfWork.Repository<Venda>().GetByIdAsync(id) ??
				throw new NotFoundException($"Venda com ID {id} não encontrada");

			if (venda.Status != StatusVenda.Confirmada)
				throw new BusinessException("Apenas vendas confirmadas podem ser entregues");

			venda.Status = StatusVenda.Entregue;
			_unitOfWork.Repository<Venda>().Update(venda);
			await _unitOfWork.CommitAsync();
		}

		public async Task ExcluirVendaAsync(int id)
		{
			var venda = await _unitOfWork.Repository<Venda>().GetByIdAsync(id);
			if (venda == null)
				throw new NotFoundException($"Venda com ID {id} não encontrada");

			if (venda.Status == StatusVenda.Confirmada || venda.Status == StatusVenda.Entregue)
				throw new BusinessException("Não é possível excluir vendas confirmadas ou entregues");

			_unitOfWork.Repository<Venda>().Remove(venda);
			await _unitOfWork.CommitAsync();
		}
	}
}
