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
	/// Validação para criação de produtos - Porque catálogo furado é vergonha!
	/// </summary>
	public class CriarProdutoRequestValidator : AbstractValidator<CriarProdutoRequest>
	{
		public CriarProdutoRequestValidator()
		{
			// Nome do produto
			RuleFor(x => x.Nome)
				.NotEmpty()
				.WithMessage("Nome do produto é obrigatório")
				.Length(2, 100)
				.WithMessage("Nome deve ter entre 2 e 100 caracteres")
				.Matches(@"^[a-zA-Z0-9\s\-\.À-ÿ]+$")
				.WithMessage("Nome contém caracteres inválidos")
				.WithErrorCode("INVALID_PRODUTO_NOME");

			// Descrição
			RuleFor(x => x.Descricao)
				.MaximumLength(500)
				.WithMessage("Descrição não pode exceder 500 caracteres")
				.WithErrorCode("INVALID_PRODUTO_DESCRICAO");

			// Preço
			RuleFor(x => x.Preco)
				.GreaterThan(0)
				.WithMessage("Preço deve ser maior que zero")
				.LessThanOrEqualTo(999999.99m)
				.WithMessage("Preço não pode exceder R$ 999.999,99")
				.WithMessage("Preço deve ter no máximo 2 casas decimais")
				.WithErrorCode("INVALID_PRODUTO_PRECO");

			// Estoque
			RuleFor(x => x.EstoqueAtual)
				.GreaterThanOrEqualTo(0)
				.WithMessage("Estoque não pode ser negativo")
				.LessThanOrEqualTo(999999)
				.WithMessage("Estoque não pode exceder 999.999 unidades")
				.WithErrorCode("INVALID_PRODUTO_ESTOQUE");

			// Categoria 
			RuleFor(x => x.CategoriaId)
				.GreaterThan(0)
				.WithMessage("Categoria deve ser válida")
				.WithErrorCode("INVALID_CATEGORIA_ID");
		}
	}
}
