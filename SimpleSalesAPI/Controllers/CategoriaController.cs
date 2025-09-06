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
	public class CategoriasController : ControllerBase
	{
		private readonly ICategoriaService _categoriaService;
		private readonly IProdutoService _produtoService;

		public CategoriasController(ICategoriaService categoriaService, IProdutoService produtoService)
		{
			_categoriaService = categoriaService ?? throw new ArgumentNullException(nameof(categoriaService));
			_produtoService = produtoService ?? throw new ArgumentNullException(nameof(produtoService));
		}

		/// <summary>
		/// Lista todas as categorias - Direto ao ponto!
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<List<CategoriaResponse>>> GetAll()
		{
			var categorias = await _categoriaService.ListarCategoriasAsync();
			return Ok(categorias);
		}

		/// <summary>
		/// Busca categoria por ID - Sem firulas desnecessárias!
		/// </summary>
		[HttpGet("{id}")]
		public async Task<ActionResult<CategoriaResponse>> GetById(int id)
		{
			var categoria = await _categoriaService.ObterCategoriaPorIdAsync(id);
			return categoria == null ? NotFound() : Ok(categoria);
		}

		/// <summary>
		/// Lista produtos da categoria - Composição inteligente de Services!
		/// </summary>
		[HttpGet("{id}/produtos")]
		public async Task<ActionResult<List<ProdutoResponse>>> GetProdutos(int id)
		{
			// Verifica se categoria existe (validação no lugar certo!)
			var categoria = await _categoriaService.ObterCategoriaPorIdAsync(id);
			if (categoria == null)
				return NotFound("Categoria não encontrada");

			var produtos = await _produtoService.ListarProdutosPorCategoriaAsync(id);
			return Ok(produtos);
		}

		/// <summary>
		/// Cria categoria - Request simples e objetivo!
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<CategoriaResponse>> Create([FromBody] CriarCategoriaRequest request)
		{
			try
			{
				var categoria = await _categoriaService.CriarCategoriaAsync(request);
				return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
			}
			catch (BusinessException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Atualiza categoria - Consistência na API!
		/// </summary>
		[HttpPut("{id}")]
		public async Task<ActionResult<CategoriaResponse>> Update(int id, [FromBody] CriarCategoriaRequest request)
		{
			try
			{
				var categoria = await _categoriaService.AtualizarCategoriaAsync(id, request);
				return Ok(categoria);
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
		/// Exclui categoria - Service decide se pode ou não!
		/// </summary>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				await _categoriaService.ExcluirCategoriaAsync(id);
				return NoContent();
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
	}
}