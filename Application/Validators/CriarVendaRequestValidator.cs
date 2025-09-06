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
	/// Validação para criação de vendas - Porque input inválido é coisa de amador!
	/// </summary>
	public class CriarVendaRequestValidator : AbstractValidator<CriarVendaRequest>
	{
		public CriarVendaRequestValidator()
		{
			// Validação do Cliente
			RuleFor(x => x.ClienteId)
				.GreaterThan(0)
				.WithMessage("ID do cliente deve ser um valor positivo válido")
				.WithErrorCode("INVALID_CLIENTE_ID");

			// Validação dos Itens 
			RuleFor(x => x.Itens)
				.NotNull()
				.WithMessage("Lista de itens não pode ser nula")
				.NotEmpty()
				.WithMessage("Venda deve conter pelo menos um item")
				.Must(HaveValidItems)
				.WithMessage("Todos os itens devem ser válidos")
				.Must(HaveUniqueProducts)
				.WithMessage("Não é permitido produtos duplicados na mesma venda");

			// Validação individual de cada item
			RuleForEach(x => x.Itens)
				.SetValidator(new ItemVendaRequestValidator());

			// Regra de negócio: máximo 50 itens por venda
			RuleFor(x => x.Itens)
				.Must(itens => itens.Count <= 50)
				.WithMessage("Venda não pode ter mais de 50 itens")
				.When(x => x.Itens != null);
		}

		/// <summary>
		/// Valida se todos os itens são válidos 
		/// </summary>
		private static bool HaveValidItems(List<ItemVendaRequest> itens)
		{
			if (itens == null) return false;

			return itens.All(item =>
				item.ProdutoId > 0 &&
				item.Quantidade > 0 &&
				item.Quantidade <= 1000);
		}

		/// <summary>
		/// Evita produtos duplicados
		/// </summary>
		private static bool HaveUniqueProducts(List<ItemVendaRequest> itens)
		{
			if (itens == null) return false;

			var produtoIds = itens.Select(i => i.ProdutoId).ToList();
			return produtoIds.Count == produtoIds.Distinct().Count();
		}
	}
}
