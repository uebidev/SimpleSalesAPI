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
	/// Validação para itens de venda 
	/// </summary>
	public class ItemVendaRequestValidator : AbstractValidator<ItemVendaRequest>
	{
		public ItemVendaRequestValidator()
		{
			// Produto ID deve ser válido
			RuleFor(x => x.ProdutoId)
				.GreaterThan(0)
				.WithMessage("ID do produto deve ser um valor positivo válido")
				.WithErrorCode("INVALID_PRODUTO_ID");

			// Quantidade 
			RuleFor(x => x.Quantidade)
				.GreaterThan(0)
				.WithMessage("Quantidade deve ser maior que zero")
				.LessThanOrEqualTo(1000)
				.WithMessage("Quantidade não pode exceder 1000 unidades por item")
				.WithErrorCode("INVALID_QUANTIDADE");
		}
	}
}
