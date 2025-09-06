using SimpleSalesAPI.Application.Dtos.Requests;
using SimpleSalesAPI.Application.Dtos.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Services.Interfaces
{
	public interface IProdutoService
	{
		Task<ProdutoResponse> CriarProdutoAsync(CriarProdutoRequest request);
		Task<ProdutoResponse?> ObterProdutoPorIdAsync(int id);
		Task<List<ProdutoResponse>> ListarProdutosAtivosAsync();
		Task<List<ProdutoResponse>> ListarProdutosPorCategoriaAsync(int categoriaId);
		Task<List<ProdutoResponse>> PesquisarProdutosAsync(string? nome, decimal? precoMin, decimal? precoMax);
		Task<List<ProdutoResponse>> ListarProdutosBaixoEstoqueAsync(int limite = 10);
		Task<ProdutoResponse> AtualizarProdutoAsync(int id, AtualizarProdutoRequest request);
		Task AtivarProdutoAsync(int id);
		Task DesativarProdutoAsync(int id);
		Task ExcluirProdutoAsync(int id);
	}
}
