using SimpleSalesAPI.Application.Dtos.Requests;
using SimpleSalesAPI.Application.Dtos.Responses;
using SimpleSalesAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Services.Interfaces
{
	public interface IVendaService
	{
		Task<VendaResponse> CriarVendaAsync(CriarVendaRequest request);
		Task<VendaResponse?> ObterVendaPorIdAsync(int id);
		Task<List<VendaResponse>> ListarVendasAsync();
		Task<List<VendaResponse>> ListarVendasPorClienteAsync(int clienteId);
		Task<List<VendaResponse>> ListarVendasPorStatusAsync(StatusVenda status);
		Task<List<VendaResponse>> ListarVendasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
		Task ConfirmarVendaAsync(int id);
		Task CancelarVendaAsync(int id);
		Task EntregarVendaAsync(int id);
		Task ExcluirVendaAsync(int id);
	}
}
