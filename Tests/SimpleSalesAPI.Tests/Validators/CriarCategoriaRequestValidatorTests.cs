using FluentValidation.TestHelper;
using SimpleSalesAPI.Application.Dtos.Requests;
using SimpleSalesAPI.Application.Validators;
using Xunit;

namespace SimpleSalesAPI.Tests.Validators;

public class CriarCategoriaRequestValidatorTests
{
    private readonly CriarCategoriaRequestValidator _validator = new();

    [Fact]
    public void Deve_Apresentar_Erro_Quando_Nome_For_Muito_Curto()
    {
        var request = new CriarCategoriaRequest
        {
            Nome = "A",
            Descricao = "Categoria"
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Nome)
              .WithErrorCode("INVALID_CATEGORIA_NOME");
    }

    [Fact]
    public void Deve_Passar_Quando_Dados_Forem_Válidos()
    {
        var request = new CriarCategoriaRequest
        {
            Nome = "Informática",
            Descricao = "Produtos de tecnologia"
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
