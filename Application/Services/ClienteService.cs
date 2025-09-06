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

		public ClienteService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		}

		public async Task<ClienteResponse> CriarClienteAsync(CriarClienteRequest request)
		{
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
			var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id) ??
				throw new NotFoundException($"Cliente com ID {id} não encontrado");

			cliente.Nome = request.Nome;
			cliente.Email = request.Email;
			cliente.Telefone = request.Telefone;
			cliente.Endereco = request.Endereco;

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
