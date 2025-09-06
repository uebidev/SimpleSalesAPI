using Microsoft.AspNetCore.Mvc;
using SimpleSalesAPI.Application.Dtos.Requests;
using SimpleSalesAPI.Application.Dtos.Responses;
using SimpleSalesAPI.Application.Services.Interfaces;
using SimpleSalesAPI.Domain.Enums;
using SimpleSalesAPI.Domain.Exceptions;
using SimpleSalesAPI.Domain.Models;
using SimpleSalesAPI.Infrastructure.Data.Repositories.Interfaces;

namespace SimpleSalesAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class VendasController : ControllerBase
	{
		private readonly IVendaService _vendaService;

		public VendasController(IVendaService vendaService)
		{
			_vendaService = vendaService ?? throw new ArgumentNullException(nameof(vendaService));
		}

		/// <summary>
		/// Lista todas as vendas - Simples como deveria ser!
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<List<VendaResponse>>> GetAll()
		{
			var vendas = await _vendaService.ListarVendasAsync();
			return Ok(vendas);
		}

		/// <summary>
		/// Busca venda por ID - 3 linhas, não 50!
		/// </summary>
		[HttpGet("{id}")]
		public async Task<ActionResult<VendaResponse>> GetById(int id)
		{
			var venda = await _vendaService.ObterVendaPorIdAsync(id);
			return venda == null ? NotFound() : Ok(venda);
		}

		/// <summary>
		/// Lista vendas por cliente - Controller que sabe seu lugar!
		/// </summary>
		[HttpGet("cliente/{clienteId}")]
		public async Task<ActionResult<List<VendaResponse>>> GetByCliente(int clienteId)
		{
			var vendas = await _vendaService.ListarVendasPorClienteAsync(clienteId);
			return Ok(vendas);
		}

		/// <summary>
		/// Lista vendas por status - Enum binding automático (que maravilha!)
		/// </summary>
		[HttpGet("status/{status}")]
		public async Task<ActionResult<List<VendaResponse>>> GetByStatus(StatusVenda status)
		{
			var vendas = await _vendaService.ListarVendasPorStatusAsync(status);
			return Ok(vendas);
		}

		/// <summary>
		/// Lista vendas por período - Query parameters como gente civilizada!
		/// </summary>
		[HttpGet("periodo")]
		public async Task<ActionResult<List<VendaResponse>>> GetByPeriodo(
			[FromQuery] DateTime dataInicio,
			[FromQuery] DateTime dataFim)
		{
			var vendas = await _vendaService.ListarVendasPorPeriodoAsync(dataInicio, dataFim);
			return Ok(vendas);
		}

		/// <summary>
		/// Cria venda - Finalmente usando DTO adequado!
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<VendaResponse>> Create([FromBody] CriarVendaRequest request)
		{
			try
			{
				var venda = await _vendaService.CriarVendaAsync(request);
				return CreatedAtAction(nameof(GetById), new { id = venda.Id }, venda);
			}
			catch (NotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (InsufficientStockException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (BusinessException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Confirma venda - PATCH para mudança de estado (REST correto!)
		/// </summary>
		[HttpPatch("{id}/confirmar")]
		public async Task<IActionResult> Confirmar(int id)
		{
			try
			{
				await _vendaService.ConfirmarVendaAsync(id);
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

		/// <summary>
		/// Cancela venda - Service cuida da lógica de estoque!
		/// </summary>
		[HttpPatch("{id}/cancelar")]
		public async Task<IActionResult> Cancelar(int id)
		{
			try
			{
				await _vendaService.CancelarVendaAsync(id);
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

		/// <summary>
		/// Marca como entregue - Controller limpo como lágrima!
		/// </summary>
		[HttpPatch("{id}/entregar")]
		public async Task<IActionResult> Entregar(int id)
		{
			try
			{
				await _vendaService.EntregarVendaAsync(id);
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

		/// <summary>
		/// Exclui venda - Validações no Service onde pertencem!
		/// </summary>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				await _vendaService.ExcluirVendaAsync(id);
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