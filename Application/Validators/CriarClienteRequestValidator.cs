using FluentValidation;
using SimpleSalesAPI.Application.Dtos.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Validators
{
	/// <summary>
	/// Validação para clientes - Porque CRM com dados inválidos é piada!
	/// </summary>
	public class CriarClienteRequestValidator : AbstractValidator<CriarClienteRequest>
	{
		public CriarClienteRequestValidator()
		{
			// Nome - Essencial para qualquer cliente
			RuleFor(x => x.Nome)
				.NotEmpty()
				.WithMessage("Nome do cliente é obrigatório")
				.Length(2, 100)
				.WithMessage("Nome deve ter entre 2 e 100 caracteres")
				.Matches(@"^[a-zA-ZÀ-ÿ\s\.]+$")
				.WithMessage("Nome deve conter apenas letras e espaços")
				.WithErrorCode("INVALID_CLIENTE_NOME");

			// Email
			RuleFor(x => x.Email)
				.NotEmpty()
				.WithMessage("Email é obrigatório")
				.EmailAddress()
				.WithMessage("Email deve ter formato válido")
				.Length(5, 100)
				.WithMessage("Email deve ter entre 5 e 100 caracteres")
				.Must(BeValidEmailDomain)
				.WithMessage("Domínio do email não é válido")
				.WithErrorCode("INVALID_CLIENTE_EMAIL");

			// Telefone
			RuleFor(x => x.Telefone)
				.NotEmpty()
				.WithMessage("Telefone é obrigatório")
				.Matches(@"^\(\d{2}\)\s\d{4,5}-\d{4}$")
				.WithMessage("Telefone deve estar no formato (11) 99999-9999")
				.WithErrorCode("INVALID_CLIENTE_TELEFONE");

			// Endereço 
			RuleFor(x => x.Endereco)
				.MaximumLength(200)
				.WithMessage("Endereço não pode exceder 200 caracteres")
				.WithErrorCode("INVALID_CLIENTE_ENDERECO");
		}

		/// <summary>
		/// Valida domínios de email 
		/// </summary>
		private static bool BeValidEmailDomain(string email)
		{
			if (string.IsNullOrEmpty(email)) return false;

			var domain = email.Split('@').LastOrDefault()?.ToLower();
			if (string.IsNullOrEmpty(domain)) return false;

			// Lista de domínios suspeitos
			var suspiciousDomains = new[] { "test.com", "fake.com", "invalid.com", "temp.com" };

			return !suspiciousDomains.Contains(domain) && domain.Contains('.');
		}
	}
}
