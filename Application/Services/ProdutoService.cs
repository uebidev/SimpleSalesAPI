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

		public ProdutoService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		}

		public async Task<ProdutoResponse> CriarProdutoAsync(CriarProdutoRequest request)
		{
	
			var categoria = await _unitOfWork.Repository<Categoria>().GetByIdAsync(request.CategoriaId);
			if (categoria == null)
				throw new NotFoundException($"Categoria com ID {request.CategoriaId} não encontrada");

			var produto = new Produto
			{
				Nome = request.Nome,
				Descricao = request.Descricao,
				Preco = request.Preco,
				EstoqueAtual = request.EstoqueAtual,
				CategoriaId = request.CategoriaId,
				Ativo = true
			};

			await _unitOfWork.Repository<Produto>().AddAsync(produto);
			await _unitOfWork.CommitAsync();

			return await ObterProdutoPorIdAsync(produto.Id) ?? throw new BusinessException("Erro ao recuperar produto criado");
		}

		public async Task<ProdutoResponse?> ObterProdutoPorIdAsync(int id)
		{
			var produtos = await _unitOfWork.Repository<Produto>()
				.GetFilterAsync(p => p.Id == id, tracking: false, p => p.Categoria);

			var produto = produtos.FirstOrDefault();
			if (produto == null) return null;

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
			var produtos = await _unitOfWork.Repository<Produto>()
				.GetFilterAsync(p => p.Ativo, tracking: false, p => p.Categoria);

			var result = new List<ProdutoResponse>();
			foreach (var produto in produtos)
			{
				var produtoResponse = await ObterProdutoPorIdAsync(produto.Id);
				if (produtoResponse != null)
					result.Add(produtoResponse);
			}

			return result;
		}

		public async Task<List<ProdutoResponse>> ListarProdutosPorCategoriaAsync(int categoriaId)
		{
			var produtos = await _unitOfWork.Repository<Produto>()
				.GetFilterAsync(p => p.CategoriaId == categoriaId && p.Ativo, tracking: false, p => p.Categoria);

			var result = new List<ProdutoResponse>();
			foreach (var produto in produtos)
			{
				var produtoResponse = await ObterProdutoPorIdAsync(produto.Id);
				if (produtoResponse != null)
					result.Add(produtoResponse);
			}

			return result;
		}

		public async Task<List<ProdutoResponse>> PesquisarProdutosAsync(string? nome, decimal? precoMin, decimal? precoMax)
		{
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

			return result;
		}

		public async Task<List<ProdutoResponse>> ListarProdutosBaixoEstoqueAsync(int limite = 10)
		{
			var produtos = await _unitOfWork.Repository<Produto>()
				.GetFilterAsync(p => p.Ativo && p.EstoqueAtual <= limite, tracking: false, p => p.Categoria);

			var result = new List<ProdutoResponse>();
			foreach (var produto in produtos)
			{
				var produtoResponse = await ObterProdutoPorIdAsync(produto.Id);
				if (produtoResponse != null)
					result.Add(produtoResponse);
			}

			return result;
		}

		public async Task<ProdutoResponse> AtualizarProdutoAsync(int id, AtualizarProdutoRequest request)
		{
			var produto = await _unitOfWork.Repository<Produto>().GetByIdAsync(id);
			if (produto == null)
				throw new NotFoundException($"Produto com ID {id} não encontrado");

			var categoria = await _unitOfWork.Repository<Categoria>().GetByIdAsync(request.CategoriaId);
			if (categoria == null)
				throw new NotFoundException($"Categoria com ID {request.CategoriaId} não encontrada");

			produto.Nome = request.Nome;
			produto.Descricao = request.Descricao;
			produto.Preco = request.Preco;
			produto.EstoqueAtual = request.EstoqueAtual;
			produto.CategoriaId = request.CategoriaId;

			_unitOfWork.Repository<Produto>().Update(produto);
			await _unitOfWork.CommitAsync();

			return await ObterProdutoPorIdAsync(id) ?? throw new BusinessException("Erro ao recuperar produto atualizado");
		}

		public async Task AtivarProdutoAsync(int id)
		{
			var produto = await _unitOfWork.Repository<Produto>().GetByIdAsync(id);
			if (produto == null)
				throw new NotFoundException($"Produto com ID {id} não encontrado");

			produto.Ativo = true;
			_unitOfWork.Repository<Produto>().Update(produto);
			await _unitOfWork.CommitAsync();
		}

		public async Task DesativarProdutoAsync(int id)
		{
			var produto = await _unitOfWork.Repository<Produto>().GetByIdAsync(id);
			if (produto == null)
				throw new NotFoundException($"Produto com ID {id} não encontrado");

			produto.Ativo = false;
			_unitOfWork.Repository<Produto>().Update(produto);
			await _unitOfWork.CommitAsync();
		}

		public async Task ExcluirProdutoAsync(int id)
		{
			var produto = await _unitOfWork.Repository<Produto>().GetByIdAsync(id);
			if (produto == null)
				throw new NotFoundException($"Produto com ID {id} não encontrado");

			_unitOfWork.Repository<Produto>().Remove(produto);
			await _unitOfWork.CommitAsync();
		}
	}
}
