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
	public class ClientesController(IClienteService clienteService, IVendaService vendaService) : ControllerBase
	{
		private readonly IClienteService _clienteService = clienteService ??
			throw new ArgumentNullException(nameof(clienteService));
		private readonly IVendaService _vendaService = vendaService ??
			throw new ArgumentNullException(nameof(vendaService));

		/// <summary>
		/// Lista todos os clientes 
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<List<ClienteResponse>>> GetAll()
		{
			var clientes = await _clienteService.ListarClientesAsync();
			return Ok(clientes);
		}

		/// <summary>
		/// Busca cliente por ID 
		/// </summary>
		[HttpGet("{id}")]
		public async Task<ActionResult<ClienteResponse>> GetById(int id)
		{
			var cliente = await _clienteService.ObterClientePorIdAsync(id);
			return cliente == null ? NotFound() : Ok(cliente);
		}

		/// <summary>
		/// Pesquisa clientes
		/// </summary>
		[HttpGet("search")]
		public async Task<ActionResult<List<ClienteResponse>>> Search(
			[FromQuery] string? nome,
			[FromQuery] string? email)
		{
			var clientes = await _clienteService.PesquisarClientesAsync(nome, email);
			return Ok(clientes);
		}

		/// <summary>
		/// Lista vendas do cliente
		/// </summary>
		[HttpGet("{id}/vendas")]
		public async Task<ActionResult<List<VendaResponse>>> GetVendas(int id)
		{
			var cliente = await _clienteService.ObterClientePorIdAsync(id);
			if (cliente == null)
				return NotFound("Cliente não encontrado");

			var vendas = await _vendaService.ListarVendasPorClienteAsync(id);
			return Ok(vendas);
		}

		/// <summary>
		/// Cria cliente
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<ClienteResponse>> Create([FromBody] CriarClienteRequest request)
		{
			var cliente = await _clienteService.CriarClienteAsync(request);
			return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
		}

		/// <summary>
		/// Atualiza cliente
		/// </summary>
		[HttpPut("{id}")]
		public async Task<ActionResult<ClienteResponse>> Update(int id, [FromBody] CriarClienteRequest request)
		{
			var cliente = await _clienteService.AtualizarClienteAsync(id, request);
			return Ok(cliente);
		}

		/// <summary>
		/// Exclui cliente 
		/// </summary>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			await _clienteService.ExcluirClienteAsync(id);
			return NoContent();
		}
	}
}