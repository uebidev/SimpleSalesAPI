using SimpleSalesAPI.Application.Dtos.Requests;
using SimpleSalesAPI.Application.Dtos.Responses;
using SimpleSalesAPI.Application.Services.Interfaces;
using SimpleSalesAPI.Domain.Exceptions;
using SimpleSalesAPI.Domain.Models;
using SimpleSalesAPI.Infrastructure.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Services
{
	public class CategoriaService(IUnitOfWork unitOfWork) : ICategoriaService
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

		public async Task<CategoriaResponse> CriarCategoriaAsync(CriarCategoriaRequest request)
		{
			var categoria = new Categoria
			{
				Nome = request.Nome,
				Descricao = request.Descricao
			};

			await _unitOfWork.Repository<Categoria>().AddAsync(categoria);
			await _unitOfWork.CommitAsync();

			return await ObterCategoriaPorIdAsync(categoria.Id) ?? throw new BusinessException("Erro ao recuperar categoria criada");
		}

		public async Task<CategoriaResponse?> ObterCategoriaPorIdAsync(int id)
		{
			
			var categoria = await _unitOfWork.Repository<Categoria>().GetByIdAsync(id);

			return categoria == null
				? null
				: new CategoriaResponse
				{
					Id = categoria.Id,
					Nome = categoria.Nome,
					Descricao = categoria.Descricao
				};
		}

		public async Task<List<CategoriaResponse>> ListarCategoriasAsync()
		{
			var categorias = await _unitOfWork.Repository<Categoria>().GetAllAsync();

			return [.. categorias.Select(c => new CategoriaResponse
			{
				Id = c.Id,
				Nome = c.Nome,
				Descricao = c.Descricao
			})];
		}

		public async Task<CategoriaResponse> AtualizarCategoriaAsync(int id, CriarCategoriaRequest request)
		{
			var categoria = await _unitOfWork.Repository<Categoria>().GetByIdAsync(id) ??
				throw new NotFoundException($"Categoria com ID {id} não encontrada");

			categoria.Nome = request.Nome;
			categoria.Descricao = request.Descricao;

			_unitOfWork.Repository<Categoria>().Update(categoria);
			await _unitOfWork.CommitAsync();

			return await ObterCategoriaPorIdAsync(id) ?? throw new BusinessException("Erro ao recuperar categoria atualizada");
		}

		public async Task ExcluirCategoriaAsync(int id)
		{
			var categoria = await _unitOfWork.Repository<Categoria>().GetByIdAsync(id) ??
				throw new NotFoundException($"Categoria com ID {id} não encontrada");

			_unitOfWork.Repository<Categoria>().Remove(categoria);
			await _unitOfWork.CommitAsync();
		}
	}
}
