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
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Services
{
	public class ClienteService(IUnitOfWork unitOfWork, IValidator<CriarClienteRequest> criarClienteValidator) : IClienteService
	{
		private readonly IUnitOfWork _unitOfWork = unitOfWork ??
			throw new ArgumentNullException(nameof(unitOfWork));

		private readonly IValidator<CriarClienteRequest> _criarClienteValidator = criarClienteValidator ??
			throw new ArgumentNullException(nameof(criarClienteValidator));

		public async Task<ClienteResponse> CriarClienteAsync(CriarClienteRequest request)
		{
			var validationResult = await _criarClienteValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
				throw new ValidationException(validationResult.Errors);

			var emailExistente = await _unitOfWork.Repository<Cliente>()
			 .GetFilterAsync(c => c.Email.ToLower() == request.Email.ToLower().Trim(), tracking: false);

			if (emailExistente.Any())
				throw new BusinessException($"Já existe um cliente cadastrado com o email {request.Email}");

			var cliente = new Cliente
			{
				Nome = request.Nome,
				Email = request.Email,
				Telefone = request.Telefone,
				Endereco = request.Endereco
			};

			await _unitOfWork.Repository<Cliente>().AddAsync(cliente);
			await _unitOfWork.CommitAsync();

			return await ObterClientePorIdAsync(cliente.Id) ?? throw new BusinessException("Erro ao recuperar cliente criado");
		}

		public async Task<ClienteResponse?> ObterClientePorIdAsync(int id)
		{
			var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id);
			if (cliente == null) return null;

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
			var clientes = await _unitOfWork.Repository<Cliente>().GetAllAsync();

			return [.. clientes.Select(c => new ClienteResponse
			{
				Id = c.Id,
				Nome = c.Nome,
				Email = c.Email,
				Telefone = c.Telefone,
				Endereco = c.Endereco
			})];
		}

		public async Task<List<ClienteResponse>> PesquisarClientesAsync(string? nome, string? email)
		{
			var clientes = await _unitOfWork.Repository<Cliente>()
				.GetFilterAsync(c =>
					(string.IsNullOrEmpty(nome) || c.Nome.Contains(nome)) &&
					(string.IsNullOrEmpty(email) || c.Email.Contains(email)),
					tracking: false);

			return [.. clientes.Select(c => new ClienteResponse
			{
				Id = c.Id,
				Nome = c.Nome,
				Email = c.Email,
				Telefone = c.Telefone,
				Endereco = c.Endereco
			})];
		}

		public async Task<ClienteResponse> AtualizarClienteAsync(int id, CriarClienteRequest request)
		{

			var validationResult = await _criarClienteValidator.ValidateAsync(request);

			if (!validationResult.IsValid)
				throw new ValidationException(validationResult.Errors);

			var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id) ??
				throw new NotFoundException($"Cliente com ID {id} não encontrado");

			var emailExistente = await _unitOfWork.Repository<Cliente>()
			   .GetFilterAsync(c => c.Email.ToLower() == request.Email.ToLower().Trim() && c.Id != id, tracking: false);

			if (emailExistente.Any())
				throw new BusinessException($"Já existe outro cliente cadastrado com o email {request.Email}");

			cliente.Nome = request.Nome.Trim();
			cliente.Email = request.Email.ToLower().Trim();
			cliente.Telefone = request.Telefone.Trim();
			cliente.Endereco = request.Endereco?.Trim() ?? string.Empty;

			_unitOfWork.Repository<Cliente>().Update(cliente);
			await _unitOfWork.CommitAsync();

			return await ObterClientePorIdAsync(id) ?? throw new BusinessException("Erro ao recuperar cliente atualizado");
		}

		public async Task ExcluirClienteAsync(int id)
		{
			var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id) ??
				throw new NotFoundException($"Cliente com ID {id} não encontrado");

			_unitOfWork.Repository<Cliente>().Remove(cliente);
			await _unitOfWork.CommitAsync();
		}
	}
}
