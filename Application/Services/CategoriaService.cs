using FluentValidation;
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
	public class CategoriaService(IUnitOfWork unitOfWork, IValidator<CriarCategoriaRequest> criarCategoriaValidator) : ICategoriaService
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork ??
			throw new ArgumentNullException(nameof(unitOfWork));

		private readonly IValidator<CriarCategoriaRequest> _criarCategoriaValidator = criarCategoriaValidator ??
		throw new ArgumentNullException(nameof(criarCategoriaValidator));

		public async Task<CategoriaResponse> CriarCategoriaAsync(CriarCategoriaRequest request)
		{
			var validationResult = await _criarCategoriaValidator.ValidateAsync(request);

			if (!validationResult.IsValid)
				throw new ValidationException(validationResult.Errors);

			var nomeExistente = await _unitOfWork.Repository<Categoria>()
			  .GetFilterAsync(c => c.Nome.ToLower() == request.Nome.ToLower().Trim(), tracking: false);

			if (nomeExistente.Any())
				throw new BusinessException($"Já existe uma categoria com o nome '{request.Nome}'");

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
			var validationResult = await _criarCategoriaValidator.ValidateAsync(request);

			if (!validationResult.IsValid)
				throw new ValidationException(validationResult.Errors);

			var categoria = await _unitOfWork.Repository<Categoria>().GetByIdAsync(id) ??
				throw new NotFoundException($"Categoria com ID {id} não encontrada");

			var nomeExistente = await _unitOfWork.Repository<Categoria>()
			   .GetFilterAsync(c => c.Nome.ToLower() == request.Nome.ToLower().Trim() && c.Id != id, tracking: false);

			if (nomeExistente.Any())
				throw new BusinessException($"Já existe outra categoria com o nome '{request.Nome}'");

			categoria.Nome = request.Nome.Trim();
			categoria.Descricao = request.Descricao?.Trim() ?? string.Empty;

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
