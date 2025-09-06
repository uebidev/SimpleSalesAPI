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
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Services
{
	public class ClienteService : IClienteService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CriarClienteRequest> _criarClienteValidator;
		private readonly ILogger<ClienteService> _logger;

		public ClienteService(
			IUnitOfWork unitOfWork,
			IValidator<CriarClienteRequest> criarClienteValidator,
			ILogger<ClienteService> logger)
		{
			_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
			_criarClienteValidator = criarClienteValidator ?? throw new ArgumentNullException(nameof(criarClienteValidator));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<ClienteResponse> CriarClienteAsync(CriarClienteRequest request)
		{
			using var activity = _logger.BeginScope(new Dictionary<string, object>
			{
				["Operation"] = nameof(CriarClienteAsync),
				["ClienteNome"] = request.Nome,
				["ClienteEmail"] = request.Email
			});

			_logger.LogInformation("Iniciando criação de cliente {ClienteNome} ({ClienteEmail})",
				request.Nome, request.Email);

			var validationResult = await _criarClienteValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				_logger.LogWarning("Validação falhou para criação de cliente. Erros: {@ValidationErrors}",
					validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

				throw new ValidationException(validationResult.Errors);
			}

			var emailExistente = await _unitOfWork.Repository<Cliente>()
				.GetFilterAsync(c => c.Email.ToLower() == request.Email.ToLower().Trim(), tracking: false);

			if (emailExistente.Any())
			{
				_logger.LogWarning("Tentativa de criar cliente com email já existente: {Email}", request.Email);
				throw new BusinessException($"Já existe um cliente cadastrado com o email {request.Email}");
			}

			var cliente = new Cliente
			{
				Nome = request.Nome.Trim(),
				Email = request.Email.ToLower().Trim(),
				Telefone = request.Telefone.Trim(),
				Endereco = request.Endereco?.Trim() ?? string.Empty
			};

			await _unitOfWork.Repository<Cliente>().AddAsync(cliente);
			await _unitOfWork.CommitAsync();

			_logger.LogInformation("Cliente {ClienteId} ({ClienteNome}) criado com sucesso",
				cliente.Id, cliente.Nome);

			return await ObterClientePorIdAsync(cliente.Id) ??
				   throw new BusinessException("Erro ao recuperar cliente criado");
		}

		public async Task<ClienteResponse?> ObterClientePorIdAsync(int id)
		{
			_logger.LogDebug("Buscando cliente {ClienteId}", id);

			var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id);
			if (cliente == null)
			{
				_logger.LogWarning("Cliente {ClienteId} não encontrado", id);
				return null;
			}

			return new ClienteResponse
			{
				Id = cliente.Id,
				Nome = cliente.Nome,
				Email = cliente.Email,
				Telefone = cliente.Telefone,
				Endereco = cliente.Endereco
			};
		}

		public async Task<List<ClienteResponse>> ListarClientesAsync()
		{
			_logger.LogDebug("Listando todos os clientes");

			var clientes = await _unitOfWork.Repository<Cliente>().GetAllAsync();

			var result = clientes.Select(c => new ClienteResponse
			{
				Id = c.Id,
				Nome = c.Nome,
				Email = c.Email,
				Telefone = c.Telefone,
				Endereco = c.Endereco
			}).ToList();

			_logger.LogInformation("Encontrados {QuantidadeClientes} clientes", result.Count);
			return result;
		}

		public async Task<List<ClienteResponse>> PesquisarClientesAsync(string? nome, string? email)
		{
			using var activity = _logger.BeginScope(new Dictionary<string, object>
			{
				["Operation"] = nameof(PesquisarClientesAsync),
				["Nome"] = nome ?? "null",
				["Email"] = email ?? "null"
			});

			_logger.LogDebug("Pesquisando clientes com filtros: Nome={Nome}, Email={Email}", nome, email);

			var clientes = await _unitOfWork.Repository<Cliente>()
				.GetFilterAsync(c =>
					(string.IsNullOrEmpty(nome) || c.Nome.Contains(nome)) &&
					(string.IsNullOrEmpty(email) || c.Email.Contains(email)),
					tracking: false);

			var result = clientes.Select(c => new ClienteResponse
			{
				Id = c.Id,
				Nome = c.Nome,
				Email = c.Email,
				Telefone = c.Telefone,
				Endereco = c.Endereco
			}).ToList();

			_logger.LogInformation("Pesquisa de clientes retornou {QuantidadeResultados} resultados", result.Count);
			return result;
		}

		public async Task<ClienteResponse> AtualizarClienteAsync(int id, CriarClienteRequest request)
		{
			using var activity = _logger.BeginScope(new Dictionary<string, object>
			{
				["Operation"] = nameof(AtualizarClienteAsync),
				["ClienteId"] = id,
				["NovoEmail"] = request.Email
			});

			_logger.LogInformation("Atualizando cliente {ClienteId}", id);

			var validationResult = await _criarClienteValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				_logger.LogWarning("Validação falhou para atualização do cliente {ClienteId}. Erros: {@ValidationErrors}",
					id, validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

				throw new ValidationException(validationResult.Errors);
			}

			var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id);
			if (cliente == null)
			{
				_logger.LogWarning("Tentativa de atualizar cliente inexistente {ClienteId}", id);
				throw new NotFoundException("Cliente", id);
			}

			var emailExistente = await _unitOfWork.Repository<Cliente>()
				.GetFilterAsync(c => c.Email.ToLower() == request.Email.ToLower().Trim() && c.Id != id, tracking: false);

			if (emailExistente.Any())
			{
				_logger.LogWarning("Tentativa de atualizar cliente {ClienteId} com email já existente: {Email}",
					id, request.Email);
				throw new BusinessException($"Já existe outro cliente cadastrado com o email {request.Email}");
			}

			var valoresAnteriores = new
			{
				Nome = cliente.Nome,
				Email = cliente.Email,
				Telefone = cliente.Telefone
			};

			cliente.Nome = request.Nome.Trim();
			cliente.Email = request.Email.ToLower().Trim();
			cliente.Telefone = request.Telefone.Trim();
			cliente.Endereco = request.Endereco?.Trim() ?? string.Empty;

			_unitOfWork.Repository<Cliente>().Update(cliente);
			await _unitOfWork.CommitAsync();

			_logger.LogInformation("Cliente {ClienteId} atualizado. " +
								 "Nome: {NomeAntigo} -> {NomeNovo}, " +
								 "Email: {EmailAntigo} -> {EmailNovo}",
				id, valoresAnteriores.Nome, cliente.Nome,
				valoresAnteriores.Email, cliente.Email);

			return await ObterClientePorIdAsync(id) ??
				   throw new BusinessException("Erro ao recuperar cliente atualizado");
		}

		public async Task ExcluirClienteAsync(int id)
		{
			_logger.LogWarning("Excluindo cliente {ClienteId}", id);

			var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id);
			if (cliente == null)
			{
				_logger.LogWarning("Tentativa de excluir cliente inexistente {ClienteId}", id);
				throw new NotFoundException("Cliente", id);
			}

			_unitOfWork.Repository<Cliente>().Remove(cliente);
			await _unitOfWork.CommitAsync();

			_logger.LogWarning("Cliente {ClienteId} ({ClienteNome}) excluído permanentemente", id, cliente.Nome);
		}
	}
}
