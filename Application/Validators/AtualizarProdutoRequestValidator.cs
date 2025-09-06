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
	/// Validação para atualização de produtos - Mesmas regras, contexto diferente!
	/// </summary>
	public class AtualizarProdutoRequestValidator : AbstractValidator<AtualizarProdutoRequest>
	{
		public AtualizarProdutoRequestValidator()
		{
			RuleFor(x => x.Nome)
				.NotEmpty().WithMessage("Nome do produto é obrigatório")
				.Length(2, 100).WithMessage("Nome deve ter entre 2 e 100 caracteres");

			RuleFor(x => x.Preco)
				.GreaterThan(0).WithMessage("Preço deve ser maior que zero");

			RuleFor(x => x.EstoqueAtual)
				.GreaterThanOrEqualTo(0).WithMessage("Estoque não pode ser negativo");

			RuleFor(x => x.CategoriaId)
				.GreaterThan(0).WithMessage("Categoria deve ser válida");
		}
	}
}
