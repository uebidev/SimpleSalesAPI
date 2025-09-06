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
	/// Validação para categorias 
	/// </summary>
	public class CriarCategoriaRequestValidator : AbstractValidator<CriarCategoriaRequest>
	{
		public CriarCategoriaRequestValidator()
		{
			// Nome da categoria 
			RuleFor(x => x.Nome)
				.NotEmpty()
				.WithMessage("Nome da categoria é obrigatório")
				.Length(2, 50)
				.WithMessage("Nome deve ter entre 2 e 50 caracteres")
				.Matches(@"^[a-zA-ZÀ-ÿ\s\-]+$")
				.WithMessage("Nome deve conter apenas letras, espaços e hífens")
				.WithErrorCode("INVALID_CATEGORIA_NOME");

			// Descrição
			RuleFor(x => x.Descricao)
				.MaximumLength(200)
				.WithMessage("Descrição não pode exceder 200 caracteres")
				.WithErrorCode("INVALID_CATEGORIA_DESCRICAO");
		}
	}
}

