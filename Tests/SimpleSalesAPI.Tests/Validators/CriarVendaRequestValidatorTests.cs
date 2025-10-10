using System.Collections.Generic;
using FluentValidation.TestHelper;
using SimpleSalesAPI.Application.Dtos.Requests;
using SimpleSalesAPI.Application.Validators;
using Xunit;

namespace SimpleSalesAPI.Tests.Validators;

public class CriarVendaRequestValidatorTests
{
    private readonly CriarVendaRequestValidator _validator = new();

    [Fact]
    public void Deve_Apresentar_Erro_Quando_ClienteId_For_Invalido()
    {
        var request = new CriarVendaRequest
        {
            ClienteId = 0,
            Itens = new List<ItemVendaRequest>
            {
                new() { ProdutoId = 1, Quantidade = 1 }
            }
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ClienteId)
              .WithErrorCode("INVALID_CLIENTE_ID");
    }

    [Fact]
    public void Deve_Apresentar_Erro_Quando_Itens_Estiverem_Vazios()
    {
        var request = new CriarVendaRequest
        {
            ClienteId = 1,
            Itens = new List<ItemVendaRequest>()
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Itens)
              .WithErrorMessage("Venda deve conter pelo menos um item");
    }

    [Fact]
    public void Deve_Apresentar_Erro_Quando_Houver_Produtos_Duplicados()
    {
        var request = new CriarVendaRequest
        {
            ClienteId = 1,
            Itens = new List<ItemVendaRequest>
            {
                new() { ProdutoId = 1, Quantidade = 1 },
                new() { ProdutoId = 1, Quantidade = 2 }
            }
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Itens)
              .WithErrorMessage("Não é permitido produtos duplicados na mesma venda");
    }

    [Fact]
    public void Deve_Apresentar_Erro_Quando_Quantidade_De_Item_For_Maior_Que_Limite()
    {
        var request = new CriarVendaRequest
        {
            ClienteId = 1,
            Itens = new List<ItemVendaRequest>
            {
                new() { ProdutoId = 1, Quantidade = 1_001 }
            }
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor("Itens[0].Quantidade")
              .WithErrorCode("INVALID_QUANTIDADE");
    }
}
