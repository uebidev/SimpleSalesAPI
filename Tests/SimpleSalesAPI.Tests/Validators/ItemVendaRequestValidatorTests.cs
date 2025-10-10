using FluentValidation.TestHelper;
using SimpleSalesAPI.Application.Dtos.Requests;
using SimpleSalesAPI.Application.Validators;
using Xunit;

namespace SimpleSalesAPI.Tests.Validators;

public class ItemVendaRequestValidatorTests
{
    private readonly ItemVendaRequestValidator _validator = new();

    [Fact]
    public void Deve_Apresentar_Erro_Quando_ProdutoId_For_Invalido()
    {
        var item = new ItemVendaRequest { ProdutoId = 0, Quantidade = 1 };

        var result = _validator.TestValidate(item);

        result.ShouldHaveValidationErrorFor(x => x.ProdutoId)
              .WithErrorCode("INVALID_PRODUTO_ID");
    }

    [Fact]
    public void Deve_Apresentar_Erro_Quando_Quantidade_For_Maior_Que_Limite()
    {
        var item = new ItemVendaRequest { ProdutoId = 1, Quantidade = 1_001 };

        var result = _validator.TestValidate(item);

        result.ShouldHaveValidationErrorFor(x => x.Quantidade)
              .WithErrorCode("INVALID_QUANTIDADE");
    }

    [Fact]
    public void Deve_Passar_Quando_Item_For_Válido()
    {
        var item = new ItemVendaRequest { ProdutoId = 1, Quantidade = 5 };

        var result = _validator.TestValidate(item);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
