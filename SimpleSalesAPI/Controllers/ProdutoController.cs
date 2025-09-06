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
	public class ProdutosController : ControllerBase
	{
		private readonly IProdutoService _produtoService;

		public ProdutosController(IProdutoService produtoService)
		{
			_produtoService = produtoService ?? throw new ArgumentNullException(nameof(produtoService));
		}

		/// <summary>
		/// Lista produtos ativos - DTOs com contexto de categoria!
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<List<ProdutoResponse>>> GetAll()
		{
			var produtos = await _produtoService.ListarProdutosAtivosAsync();
			return Ok(produtos);
		}

		/// <summary>
		/// Busca produto por ID - Com categoria incluída automaticamente!
		/// </summary>
		[HttpGet("{id}")]
		public async Task<ActionResult<ProdutoResponse>> GetById(int id)
		{
			var produto = await _produtoService.ObterProdutoPorIdAsync(id);
			return produto == null ? NotFound() : Ok(produto);
		}

		/// <summary>
		/// Lista produtos por categoria - Relationship navigation no Service!
		/// </summary>
		[HttpGet("categoria/{categoriaId}")]
		public async Task<ActionResult<List<ProdutoResponse>>> GetByCategoria(int categoriaId)
		{
			var produtos = await _produtoService.ListarProdutosPorCategoriaAsync(categoriaId);
			return Ok(produtos);
		}

		/// <summary>
		/// Pesquisa produtos - Query complexa delegada ao Service!
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
		/// Produtos com baixo estoque - Analytics no Service onde pertencem!
		/// </summary>
		[HttpGet("baixo-estoque")]
		public async Task<ActionResult<List<ProdutoResponse>>> GetBaixoEstoque([FromQuery] int limite = 10)
		{
			var produtos = await _produtoService.ListarProdutosBaixoEstoqueAsync(limite);
			return Ok(produtos);
		}

		/// <summary>
		/// Cria produto - Request DTO específico para criação!
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<ProdutoResponse>> Create([FromBody] CriarProdutoRequest request)
		{
			try
			{
				var produto = await _produtoService.CriarProdutoAsync(request);
				return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
			}
			catch (NotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (BusinessException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Atualiza produto - DTO diferente para update!
		/// </summary>
		[HttpPut("{id}")]
		public async Task<ActionResult<ProdutoResponse>> Update(int id, [FromBody] AtualizarProdutoRequest request)
		{
			try
			{
				var produto = await _produtoService.AtualizarProdutoAsync(id, request);
				return Ok(produto);
			}
			catch (NotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (BusinessException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Ativa produto - PATCH para mudança de estado específica!
		/// </summary>
		[HttpPatch("{id}/ativar")]
		public async Task<IActionResult> Ativar(int id)
		{
			try
			{
				await _produtoService.AtivarProdutoAsync(id);
				return NoContent();
			}
			catch (NotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}

		/// <summary>
		/// Desativa produto - Soft delete como deve ser!
		/// </summary>
		[HttpPatch("{id}/desativar")]
		public async Task<IActionResult> Desativar(int id)
		{
			try
			{
				await _produtoService.DesativarProdutoAsync(id);
				return NoContent();
			}
			catch (NotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}

		/// <summary>
		/// Exclui produto - Hard delete quando necessário!
		/// </summary>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				await _produtoService.ExcluirProdutoAsync(id);
				return NoContent();
			}
			catch (NotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}
	}
}