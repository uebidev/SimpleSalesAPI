using Microsoft.AspNetCore.Mvc;
using SimpleSalesAPI.Application.Dtos.Requests;
using SimpleSalesAPI.Application.Dtos.Responses;
using SimpleSalesAPI.Application.Services.Interfaces;
using SimpleSalesAPI.Domain.Exceptions;
using SimpleSalesAPI.Domain.Models;
using SimpleSalesAPI.Infrastructure.Data.Repositories.Interfaces;

namespace SimpleSalesAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProdutosController(IProdutoService produtoService) : ControllerBase
	{
		private readonly IProdutoService _produtoService = produtoService ??
			throw new ArgumentNullException(nameof(produtoService));

		/// <summary>
		/// Lista produtos ativos
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<List<ProdutoResponse>>> GetAll()
		{
			var produtos = await _produtoService.ListarProdutosAtivosAsync();
			return Ok(produtos);
		}

		/// <summary>
		/// Busca produto por ID
		/// </summary>
		[HttpGet("{id}")]
		public async Task<ActionResult<ProdutoResponse>> GetById(int id)
		{
			var produto = await _produtoService.ObterProdutoPorIdAsync(id);
			return produto == null ? NotFound() : Ok(produto);
		}

		/// <summary>
		/// Lista produtos por categoria
		/// </summary>
		[HttpGet("categoria/{categoriaId}")]
		public async Task<ActionResult<List<ProdutoResponse>>> GetByCategoria(int categoriaId)
		{
			var produtos = await _produtoService.ListarProdutosPorCategoriaAsync(categoriaId);
			return Ok(produtos);
		}

		/// <summary>
		/// Pesquisa produtos 
		/// </summary>
		[HttpGet("search")]
		public async Task<ActionResult<List<ProdutoResponse>>> Search(
			[FromQuery] string? nome,
			[FromQuery] decimal? precoMin,
			[FromQuery] decimal? precoMax)
		{
			var produtos = await _produtoService.PesquisarProdutosAsync(nome, precoMin, precoMax);
			return Ok(produtos);
		}

		/// <summary>
		/// Produtos com baixo estoque 
		/// </summary>
		[HttpGet("baixo-estoque")]
		public async Task<ActionResult<List<ProdutoResponse>>> GetBaixoEstoque([FromQuery] int limite = 10)
		{
			var produtos = await _produtoService.ListarProdutosBaixoEstoqueAsync(limite);
			return Ok(produtos);
		}

		/// <summary>
		/// Cria produto 
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<ProdutoResponse>> Create([FromBody] CriarProdutoRequest request)
		{
			var produto = await _produtoService.CriarProdutoAsync(request);
			return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
		}

		/// <summary>
		/// Atualiza produto
		/// </summary>
		[HttpPut("{id}")]
		public async Task<ActionResult<ProdutoResponse>> Update(int id, [FromBody] AtualizarProdutoRequest request)
		{
			var produto = await _produtoService.AtualizarProdutoAsync(id, request);
			return Ok(produto);
		}

		/// <summary>
		/// Ativa produto 
		/// </summary>
		[HttpPatch("{id}/ativar")]
		public async Task<IActionResult> Ativar(int id)
		{
			await _produtoService.AtivarProdutoAsync(id);
			return NoContent();
		}

		/// <summary>
		/// Desativa produto
		/// </summary>
		[HttpPatch("{id}/desativar")]
		public async Task<IActionResult> Desativar(int id)
		{

			await _produtoService.DesativarProdutoAsync(id);
			return NoContent();
		}

		/// <summary>
		/// Exclui produto
		/// </summary>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			await _produtoService.ExcluirProdutoAsync(id);
			return NoContent();
		}
	}
}