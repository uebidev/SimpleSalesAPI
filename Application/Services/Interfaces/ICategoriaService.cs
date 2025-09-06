using SimpleSalesAPI.Application.Dtos.Requests;
using SimpleSalesAPI.Application.Dtos.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Services.Interfaces
{
	public interface ICategoriaService
	{
		Task<CategoriaResponse> CriarCategoriaAsync(CriarCategoriaRequest request);
		Task<CategoriaResponse?> ObterCategoriaPorIdAsync(int id);
		Task<List<CategoriaResponse>> ListarCategoriasAsync();
		Task<CategoriaResponse> AtualizarCategoriaAsync(int id, CriarCategoriaRequest request);
		Task ExcluirCategoriaAsync(int id);
	}
}
