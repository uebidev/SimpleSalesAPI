using FluentValidation;
using Microsoft.Extensions.Logging;
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
	public class CategoriaService : ICategoriaService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CriarCategoriaRequest> _criarCategoriaValidator;
		private readonly ILogger<CategoriaService> _logger;

		public CategoriaService(
			IUnitOfWork unitOfWork,
			IValidator<CriarCategoriaRequest> criarCategoriaValidator,
			ILogger<CategoriaService> logger)
		{
			_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
			_criarCategoriaValidator = criarCategoriaValidator ?? throw new ArgumentNullException(nameof(criarCategoriaValidator));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<CategoriaResponse> CriarCategoriaAsync(CriarCategoriaRequest request)
		{
			using var activity = _logger.BeginScope(new Dictionary<string, object>
			{
				["Operation"] = nameof(CriarCategoriaAsync),
				["CategoriaNome"] = request.Nome
			});

			_logger.LogInformation("Iniciando criação de categoria {CategoriaNome}", request.Nome);

			var validationResult = await _criarCategoriaValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				_logger.LogWarning("Validação falhou para criação de categoria. Erros: {@ValidationErrors}",
					validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

				throw new ValidationException(validationResult.Errors);
			}

			var nomeExistente = await _unitOfWork.Repository<Categoria>()
				.GetFilterAsync(c => c.Nome.ToLower() == request.Nome.ToLower().Trim(), tracking: false);

			if (nomeExistente.Any())
			{
				_logger.LogWarning("Tentativa de criar categoria com nome já existente: {Nome}", request.Nome);
				throw new BusinessException($"Já existe uma categoria com o nome '{request.Nome}'");
			}

			var categoria = new Categoria
			{
				Nome = request.Nome.Trim(),
				Descricao = request.Descricao?.Trim() ?? string.Empty
			};

			await _unitOfWork.Repository<Categoria>().AddAsync(categoria);
			await _unitOfWork.CommitAsync();

			_logger.LogInformation("Categoria {CategoriaId} ({CategoriaNome}) criada com sucesso",
				categoria.Id, categoria.Nome);

			return await ObterCategoriaPorIdAsync(categoria.Id) ??
				   throw new BusinessException("Erro ao recuperar categoria criada");
		}

		public async Task<CategoriaResponse?> ObterCategoriaPorIdAsync(int id)
		{
			_logger.LogDebug("Buscando categoria {CategoriaId}", id);

			var categoria = await _unitOfWork.Repository<Categoria>().GetByIdAsync(id);
			if (categoria == null)
			{
				_logger.LogWarning("Categoria {CategoriaId} não encontrada", id);
				return null;
			}

			return new CategoriaResponse
			{
				Id = categoria.Id,
				Nome = categoria.Nome,
				Descricao = categoria.Descricao
			};
		}

		public async Task<List<CategoriaResponse>> ListarCategoriasAsync()
		{
			_logger.LogDebug("Listando todas as categorias");

			var categorias = await _unitOfWork.Repository<Categoria>().GetAllAsync();

			var result = categorias.Select(c => new CategoriaResponse
			{
				Id = c.Id,
				Nome = c.Nome,
				Descricao = c.Descricao
			}).ToList();

			_logger.LogInformation("Encontradas {QuantidadeCategorias} categorias", result.Count);
			return result;
		}

		public async Task<CategoriaResponse> AtualizarCategoriaAsync(int id, CriarCategoriaRequest request)
		{
			using var activity = _logger.BeginScope(new Dictionary<string, object>
			{
				["Operation"] = nameof(AtualizarCategoriaAsync),
				["CategoriaId"] = id,
				["NovoNome"] = request.Nome
			});

			_logger.LogInformation("Atualizando categoria {CategoriaId}", id);

			var validationResult = await _criarCategoriaValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				_logger.LogWarning("Validação falhou para atualização da categoria {CategoriaId}. Erros: {@ValidationErrors}",
					id, validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

				throw new ValidationException(validationResult.Errors);
			}

			var categoria = await _unitOfWork.Repository<Categoria>().GetByIdAsync(id);
			if (categoria == null)
			{
				_logger.LogWarning("Tentativa de atualizar categoria inexistente {CategoriaId}", id);
				throw new NotFoundException("Categoria", id);
			}

			var nomeExistente = await _unitOfWork.Repository<Categoria>()
				.GetFilterAsync(c => c.Nome.ToLower() == request.Nome.ToLower().Trim() && c.Id != id, tracking: false);

			if (nomeExistente.Any())
			{
				_logger.LogWarning("Tentativa de atualizar categoria {CategoriaId} com nome já existente: {Nome}",
					id, request.Nome);
				throw new BusinessException($"Já existe outra categoria com o nome '{request.Nome}'");
			}

			var valoresAnteriores = new
			{
				Nome = categoria.Nome,
				Descricao = categoria.Descricao
			};

			categoria.Nome = request.Nome.Trim();
			categoria.Descricao = request.Descricao?.Trim() ?? string.Empty;

			_unitOfWork.Repository<Categoria>().Update(categoria);
			await _unitOfWork.CommitAsync();

			_logger.LogInformation("Categoria {CategoriaId} atualizada. " +
								 "Nome: {NomeAntigo} -> {NomeNovo}",
				id, valoresAnteriores.Nome, categoria.Nome);

			return await ObterCategoriaPorIdAsync(id) ??
				   throw new BusinessException("Erro ao recuperar categoria atualizada");
		}

		public async Task ExcluirCategoriaAsync(int id)
		{
			_logger.LogWarning("Excluindo categoria {CategoriaId}", id);

			var categoria = await _unitOfWork.Repository<Categoria>().GetByIdAsync(id);
			if (categoria == null)
			{
				_logger.LogWarning("Tentativa de excluir categoria inexistente {CategoriaId}", id);
				throw new NotFoundException("Categoria", id);
			}

			// Verificar se existem produtos associados
			var produtosAssociados = await _unitOfWork.Repository<Produto>()
				.GetFilterAsync(p => p.CategoriaId == id, tracking: false);

			if (produtosAssociados.Any())
			{
				_logger.LogWarning("Tentativa de excluir categoria {CategoriaId} com {QuantidadeProdutos} produtos associados",
					id, produtosAssociados.Count());
				throw new BusinessException($"Não é possível excluir a categoria '{categoria.Nome}' pois possui produtos associados");
			}

			_unitOfWork.Repository<Categoria>().Remove(categoria);
			await _unitOfWork.CommitAsync();

			_logger.LogWarning("Categoria {CategoriaId} ({CategoriaNome}) excluída permanentemente",
				id, categoria.Nome);
		}
	}
}
