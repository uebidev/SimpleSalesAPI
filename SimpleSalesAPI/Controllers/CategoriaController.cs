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
	public class CategoriasController(ICategoriaService categoriaService, IProdutoService produtoService) : ControllerBase
	{
		private readonly ICategoriaService _categoriaService = categoriaService ??
			throw new ArgumentNullException(nameof(categoriaService));
		private readonly IProdutoService _produtoService = produtoService ??
			throw new ArgumentNullException(nameof(produtoService));

		/// <summary>
		/// Lista todas as categorias
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<List<CategoriaResponse>>> GetAll()
		{
			var categorias = await _categoriaService.ListarCategoriasAsync();
			return Ok(categorias);
		}

		/// <summary>
		/// Busca categoria por ID 
		/// </summary>
		[HttpGet("{id}")]
		public async Task<ActionResult<CategoriaResponse>> GetById(int id)
		{
			var categoria = await _categoriaService.ObterCategoriaPorIdAsync(id);
			return categoria == null ? NotFound() : Ok(categoria);
		}

		/// <summary>
		/// Lista produtos da categoria 
		/// </summary>
		[HttpGet("{id}/produtos")]
		public async Task<ActionResult<List<ProdutoResponse>>> GetProdutos(int id)
		{
			var categoria = await _categoriaService.ObterCategoriaPorIdAsync(id);
			if (categoria == null)
				return NotFound("Categoria não encontrada");

			var produtos = await _produtoService.ListarProdutosPorCategoriaAsync(id);
			return Ok(produtos);
		}

		/// <summary>
		/// Cria categoria 
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<CategoriaResponse>> Create([FromBody] CriarCategoriaRequest request)
		{
			var categoria = await _categoriaService.CriarCategoriaAsync(request);
			return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
		}

		/// <summary>
		/// Atualiza categoria
		/// </summary>
		[HttpPut("{id}")]
		public async Task<ActionResult<CategoriaResponse>> Update(int id, [FromBody] CriarCategoriaRequest request)
		{
			var categoria = await _categoriaService.AtualizarCategoriaAsync(id, request);
			return Ok(categoria);
		}

		/// <summary>
		/// Exclui categoria
		/// </summary>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			await _categoriaService.ExcluirCategoriaAsync(id);
			return NoContent();
		}
	}
}