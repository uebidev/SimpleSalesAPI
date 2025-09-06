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
	public class ClientesController : ControllerBase
	{
		private readonly IClienteService _clienteService;
		private readonly IVendaService _vendaService;

		public ClientesController(IClienteService clienteService, IVendaService vendaService)
		{
			_clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
			_vendaService = vendaService ?? throw new ArgumentNullException(nameof(vendaService));
		}

		/// <summary>
		/// Lista todos os clientes - Simples e direto!
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<List<ClienteResponse>>> GetAll()
		{
			var clientes = await _clienteService.ListarClientesAsync();
			return Ok(clientes);
		}

		/// <summary>
		/// Busca cliente por ID - DTO adequado sempre!
		/// </summary>
		[HttpGet("{id}")]
		public async Task<ActionResult<ClienteResponse>> GetById(int id)
		{
			var cliente = await _clienteService.ObterClientePorIdAsync(id);
			return cliente == null ? NotFound() : Ok(cliente);
		}

		/// <summary>
		/// Pesquisa clientes - Filtros opcionais bem implementados!
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
		/// Lista vendas do cliente - Composição de Services!
		/// </summary>
		[HttpGet("{id}/vendas")]
		public async Task<ActionResult<List<VendaResponse>>> GetVendas(int id)
		{
			// Verifica se cliente existe primeiro (boa prática!)
			var cliente = await _clienteService.ObterClientePorIdAsync(id);
			if (cliente == null)
				return NotFound("Cliente não encontrado");

			var vendas = await _vendaService.ListarVendasPorClienteAsync(id);
			return Ok(vendas);
		}

		/// <summary>
		/// Cria cliente - Request específico para criação!
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<ClienteResponse>> Create([FromBody] CriarClienteRequest request)
		{
			try
			{
				var cliente = await _clienteService.CriarClienteAsync(request);
				return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
			}
			catch (BusinessException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Atualiza cliente - Mesmo DTO de criação (faz sentido!)
		/// </summary>
		[HttpPut("{id}")]
		public async Task<ActionResult<ClienteResponse>> Update(int id, [FromBody] CriarClienteRequest request)
		{
			try
			{
				var cliente = await _clienteService.AtualizarClienteAsync(id, request);
				return Ok(cliente);
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
		/// Exclui cliente - Service cuida das validações!
		/// </summary>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				await _clienteService.ExcluirClienteAsync(id);
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