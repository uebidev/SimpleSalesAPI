using FluentValidation;
using Microsoft.Extensions.Logging;
using SimpleSalesAPI.Application.Dtos.Requests;
using SimpleSalesAPI.Application.Dtos.Responses;
using SimpleSalesAPI.Application.Services.Interfaces;
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

	public class ProdutoService : IProdutoService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CriarProdutoRequest> _criarProdutoValidator;
		private readonly IValidator<AtualizarProdutoRequest> _atualizarProdutoValidator;
		private readonly ILogger<ProdutoService> _logger;

		public ProdutoService(
			IUnitOfWork unitOfWork,
			IValidator<CriarProdutoRequest> criarProdutoValidator,
			IValidator<AtualizarProdutoRequest> atualizarProdutoValidator,
			ILogger<ProdutoService> logger)
		{
			_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
			_criarProdutoValidator = criarProdutoValidator ?? throw new ArgumentNullException(nameof(criarProdutoValidator));
			_atualizarProdutoValidator = atualizarProdutoValidator ?? throw new ArgumentNullException(nameof(atualizarProdutoValidator));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ProdutoResponse> CriarProdutoAsync(CriarProdutoRequest request)
		{
			using var activity = _logger.BeginScope(new Dictionary<string, object>
			{
				["Operation"] = nameof(CriarProdutoAsync),
				["ProdutoNome"] = request.Nome,
				["CategoriaId"] = request.CategoriaId
			});

			_logger.LogInformation("Iniciando criação de produto {ProdutoNome} na categoria {CategoriaId}",
				request.Nome, request.CategoriaId);

			var validationResult = await _criarProdutoValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				_logger.LogWarning("Validação falhou para criação de produto. Erros: {@ValidationErrors}",
					validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

				throw new ValidationException(validationResult.Errors);
			}

			var categoria = await _unitOfWork.Repository<Categoria>().GetByIdAsync(request.CategoriaId);
			if (categoria == null)
			{
				_logger.LogWarning("Categoria {CategoriaId} não encontrada para produto {ProdutoNome}",
					request.CategoriaId, request.Nome);
				throw new NotFoundException("Categoria", request.CategoriaId);
			}

			var produto = new Produto
			{
				Nome = request.Nome.Trim(),
				Descricao = request.Descricao?.Trim() ?? string.Empty,
				Preco = request.Preco,
				EstoqueAtual = request.EstoqueAtual,
				CategoriaId = request.CategoriaId,
				Ativo = true
			};

			await _unitOfWork.Repository<Produto>().AddAsync(produto);
			await _unitOfWork.CommitAsync();

			_logger.LogInformation("Produto {ProdutoId} ({ProdutoNome}) criado com sucesso. " +
								 "Preço: {Preco:C}, Estoque: {EstoqueAtual}",
				produto.Id, produto.Nome, produto.Preco, produto.EstoqueAtual);

			return await ObterProdutoPorIdAsync(produto.Id) ??
				   throw new BusinessException("Erro ao recuperar produto criado");
		}

		public async Task<ProdutoResponse?> ObterProdutoPorIdAsync(int id)
		{
			_logger.LogDebug("Buscando produto {ProdutoId}", id);

			var produtos = await _unitOfWork.Repository<Produto>()
				.GetFilterAsync(p => p.Id == id, tracking: false, p => p.Categoria);

			var produto = produtos.FirstOrDefault();
			if (produto == null)
			{
				_logger.LogWarning("Produto {ProdutoId} não encontrado", id);
				return null;
			}

			return new ProdutoResponse
			{
				Id = produto.Id,
				Nome = produto.Nome,
				Descricao = produto.Descricao,
				Preco = produto.Preco,
				EstoqueAtual = produto.EstoqueAtual,
				Ativo = produto.Ativo,
				Categoria = new CategoriaResumoResponse
				{
					Id = produto.Categoria.Id,
					Nome = produto.Categoria.Nome
				}
			};
		}

		public async Task<List<ProdutoResponse>> ListarProdutosAtivosAsync()
		{
			_logger.LogDebug("Listando produtos ativos");

			var produtos = await _unitOfWork.Repository<Produto>()
				.GetFilterAsync(p => p.Ativo, tracking: false, p => p.Categoria);

			var result = new List<ProdutoResponse>();
			foreach (var produto in produtos)
			{
				var produtoResponse = await ObterProdutoPorIdAsync(produto.Id);
				if (produtoResponse != null)
					result.Add(produtoResponse);
			}

			_logger.LogInformation("Encontrados {QuantidadeProdutos} produtos ativos", result.Count);
			return result;
		}

		public async Task<List<ProdutoResponse>> ListarProdutosPorCategoriaAsync(int categoriaId)
		{
			_logger.LogDebug("Listando produtos da categoria {CategoriaId}", categoriaId);

			var produtos = await _unitOfWork.Repository<Produto>()
				.GetFilterAsync(p => p.CategoriaId == categoriaId && p.Ativo, tracking: false, p => p.Categoria);

			var result = new List<ProdutoResponse>();
			foreach (var produto in produtos)
			{
				var produtoResponse = await ObterProdutoPorIdAsync(produto.Id);
				if (produtoResponse != null)
					result.Add(produtoResponse);
			}

			_logger.LogInformation("Encontrados {QuantidadeProdutos} produtos ativos na categoria {CategoriaId}",
				result.Count, categoriaId);
			return result;
		}

		public async Task<List<ProdutoResponse>> PesquisarProdutosAsync(string? nome, decimal? precoMin, decimal? precoMax)
		{
			using var activity = _logger.BeginScope(new Dictionary<string, object>
			{
				["Operation"] = nameof(PesquisarProdutosAsync),
				["Nome"] = nome ?? "null",
				["PrecoMin"] = precoMin,
				["PrecoMax"] = precoMax
			});

			_logger.LogDebug("Pesquisando produtos com filtros: Nome={Nome}, PrecoMin={PrecoMin}, PrecoMax={PrecoMax}",
				nome, precoMin, precoMax);

			var produtos = await _unitOfWork.Repository<Produto>()
				.GetFilterAsync(p =>
					p.Ativo &&
					(string.IsNullOrEmpty(nome) || p.Nome.Contains(nome)) &&
					(!precoMin.HasValue || p.Preco >= precoMin.Value) &&
					(!precoMax.HasValue || p.Preco <= precoMax.Value),
					tracking: false,
					p => p.Categoria);

			var result = new List<ProdutoResponse>();
			foreach (var produto in produtos)
			{
				var produtoResponse = await ObterProdutoPorIdAsync(produto.Id);
				if (produtoResponse != null)
					result.Add(produtoResponse);
			}

			_logger.LogInformation("Pesquisa retornou {QuantidadeResultados} produtos", result.Count);
			return result;
		}

		public async Task<List<ProdutoResponse>> ListarProdutosBaixoEstoqueAsync(int limite = 10)
		{
			_logger.LogDebug("Listando produtos com estoque abaixo de {Limite}", limite);

			var produtos = await _unitOfWork.Repository<Produto>()
				.GetFilterAsync(p => p.Ativo && p.EstoqueAtual <= limite, tracking: false, p => p.Categoria);

			var result = new List<ProdutoResponse>();
			foreach (var produto in produtos)
			{
				var produtoResponse = await ObterProdutoPorIdAsync(produto.Id);
				if (produtoResponse != null)
					result.Add(produtoResponse);
			}

			if (result.Any())
			{
				_logger.LogWarning("Encontrados {QuantidadeProdutos} produtos com estoque baixo (≤{Limite})",
					result.Count, limite);
			}

			return result;
		}

		public async Task<ProdutoResponse> AtualizarProdutoAsync(int id, AtualizarProdutoRequest request)
		{
			using var activity = _logger.BeginScope(new Dictionary<string, object>
			{
				["Operation"] = nameof(AtualizarProdutoAsync),
				["ProdutoId"] = id,
				["NovoNome"] = request.Nome
			});

			_logger.LogInformation("Atualizando produto {ProdutoId}", id);

			var validationResult = await _atualizarProdutoValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				_logger.LogWarning("Validação falhou para atualização do produto {ProdutoId}. Erros: {@ValidationErrors}",
					id, validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

				throw new ValidationException(validationResult.Errors);
			}

			var produto = await _unitOfWork.Repository<Produto>().GetByIdAsync(id);
			if (produto == null)
			{
				_logger.LogWarning("Tentativa de atualizar produto inexistente {ProdutoId}", id);
				throw new NotFoundException("Produto", id);
			}

			var categoria = await _unitOfWork.Repository<Categoria>().GetByIdAsync(request.CategoriaId);
			if (categoria == null)
			{
				_logger.LogWarning("Categoria {CategoriaId} não encontrada para atualização do produto {ProdutoId}",
					request.CategoriaId, id);
				throw new NotFoundException("Categoria", request.CategoriaId);
			}

			var valoresAnteriores = new
			{
				produto.Nome,
				produto.Preco,
				produto.EstoqueAtual,
				produto.CategoriaId
			};

			produto.Nome = request.Nome.Trim();
			produto.Descricao = request.Descricao?.Trim() ?? string.Empty;
			produto.Preco = request.Preco;
			produto.EstoqueAtual = request.EstoqueAtual;
			produto.CategoriaId = request.CategoriaId;

			_unitOfWork.Repository<Produto>().Update(produto);
			await _unitOfWork.CommitAsync();

			_logger.LogInformation("Produto {ProdutoId} atualizado. " +
								 "Nome: {NomeAntigo} -> {NomeNovo}, " +
								 "Preço: {PrecoAntigo:C} -> {PrecoNovo:C}, " +
								 "Estoque: {EstoqueAntigo} -> {EstoqueNovo}",
				id, valoresAnteriores.Nome, produto.Nome,
				valoresAnteriores.Preco, produto.Preco,
				valoresAnteriores.EstoqueAtual, produto.EstoqueAtual);

			return await ObterProdutoPorIdAsync(id) ??
				   throw new BusinessException("Erro ao recuperar produto atualizado");
		}

		public async Task AtivarProdutoAsync(int id)
		{
			_logger.LogInformation("Ativando produto {ProdutoId}", id);

			var produto = await _unitOfWork.Repository<Produto>().GetByIdAsync(id);
			if (produto == null)
			{
				_logger.LogWarning("Tentativa de ativar produto inexistente {ProdutoId}", id);
				throw new NotFoundException("Produto", id);
			}

			if (produto.Ativo)
			{
				_logger.LogInformation("Produto {ProdutoId} já estava ativo", id);
				return;
			}

			produto.Ativo = true;
			_unitOfWork.Repository<Produto>().Update(produto);
			await _unitOfWork.CommitAsync();

			_logger.LogInformation("Produto {ProdutoId} ({ProdutoNome}) ativado com sucesso", id, produto.Nome);
		}

		public async Task DesativarProdutoAsync(int id)
		{
			_logger.LogInformation("Desativando produto {ProdutoId}", id);

			var produto = await _unitOfWork.Repository<Produto>().GetByIdAsync(id);
			if (produto == null)
			{
				_logger.LogWarning("Tentativa de desativar produto inexistente {ProdutoId}", id);
				throw new NotFoundException("Produto", id);
			}

			if (!produto.Ativo)
			{
				_logger.LogInformation("Produto {ProdutoId} já estava inativo", id);
				return;
			}

			produto.Ativo = false;
			_unitOfWork.Repository<Produto>().Update(produto);
			await _unitOfWork.CommitAsync();

			_logger.LogInformation("Produto {ProdutoId} ({ProdutoNome}) desativado com sucesso", id, produto.Nome);
		}

		public async Task ExcluirProdutoAsync(int id)
		{
			_logger.LogWarning("Excluindo produto {ProdutoId}", id);

			var produto = await _unitOfWork.Repository<Produto>().GetByIdAsync(id);
			if (produto == null)
			{
				_logger.LogWarning("Tentativa de excluir produto inexistente {ProdutoId}", id);
				throw new NotFoundException("Produto", id);
			}

			_unitOfWork.Repository<Produto>().Remove(produto);
			await _unitOfWork.CommitAsync();

			_logger.LogWarning("Produto {ProdutoId} ({ProdutoNome}) excluído permanentemente", id, produto.Nome);
		}
	}
}
