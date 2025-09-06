using SimpleSalesAPI.Application.Dtos.Requests;
using SimpleSalesAPI.Application.Dtos.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Services.Interfaces
{
	public interface IClienteService
	{
		Task<ClienteResponse> CriarClienteAsync(CriarClienteRequest request);
		Task<ClienteResponse?> ObterClientePorIdAsync(int id);
		Task<List<ClienteResponse>> ListarClientesAsync();
		Task<List<ClienteResponse>> PesquisarClientesAsync(string? nome, string? email);
		Task<ClienteResponse> AtualizarClienteAsync(int id, CriarClienteRequest request);
		Task ExcluirClienteAsync(int id);
	}
}
