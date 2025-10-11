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
	public class VendasController(IVendaService vendaService) : ControllerBase
	{
		private readonly IVendaService _vendaService = vendaService ??
			throw new ArgumentNullException(nameof(vendaService));

		/// <summary>
		/// Lista todas as vendas 
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<List<VendaResponse>>> GetAll()
		{
			var vendas = await _vendaService.ListarVendasAsync();
			return Ok(vendas);
		}

		/// <summary>
		/// Busca venda por ID 
		/// </summary>
		[HttpGet("{id}")]
		public async Task<ActionResult<VendaResponse>> GetById(int id)
		{
			var venda = await _vendaService.ObterVendaPorIdAsync(id);
			return venda == null ? NotFound() : Ok(venda);
		}

		/// <summary>
		/// Lista vendas por cliente 
		/// </summary>
		[HttpGet("cliente/{clienteId}")]
		public async Task<ActionResult<List<VendaResponse>>> GetByCliente(int clienteId)
		{
			var vendas = await _vendaService.ListarVendasPorClienteAsync(clienteId);
			return Ok(vendas);
		}

		/// <summary>
		/// Lista vendas por status 
		/// </summary>
		[HttpGet("status/{status}")]
		public async Task<ActionResult<List<VendaResponse>>> GetByStatus(StatusVenda status)
		{
			var vendas = await _vendaService.ListarVendasPorStatusAsync(status);
			return Ok(vendas);
		}

		/// <summary>
		/// Lista vendas por período 
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
		/// Cria venda
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<VendaResponse>> Create([FromBody] CriarVendaRequest request)
		{
			var venda = await _vendaService.CriarVendaAsync(request);
			return CreatedAtAction(nameof(GetById), new { id = venda.Id }, venda);
		}

		/// <summary>
		/// Confirma venda
		/// </summary>
		[HttpPatch("{id}/confirmar")]
		public async Task<IActionResult> Confirmar(int id)
		{
			await _vendaService.ConfirmarVendaAsync(id);
			return NoContent();
		}

		/// <summary>
		/// Cancela venda
		/// </summary>
		[HttpPatch("{id}/cancelar")]
		public async Task<IActionResult> Cancelar(int id)
		{
			await _vendaService.CancelarVendaAsync(id);
			return NoContent();
		}

		/// <summary>
		/// Marca como entregue 
		/// </summary>
		[HttpPatch("{id}/entregar")]
		public async Task<IActionResult> Entregar(int id)
		{
			await _vendaService.EntregarVendaAsync(id);
			return NoContent();
		}

		/// <summary>
		/// Exclui venda 
		/// </summary>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{

			await _vendaService.ExcluirVendaAsync(id);
			return NoContent();
		}
	}
}