using FluentValidation.TestHelper;
using SimpleSalesAPI.Application.Dtos.Requests;
using SimpleSalesAPI.Application.Validators;
using Xunit;

namespace SimpleSalesAPI.Tests.Validators;

public class CriarProdutoRequestValidatorTests
{
    private readonly CriarProdutoRequestValidator _validator = new();

    [Fact]
    public void Deve_Apresentar_Erro_Quando_Nome_Estiver_Vazio()
    {
        var request = new CriarProdutoRequest
        {
            Nome = string.Empty,
            Descricao = "Alguma descrição",
            Preco = 100,
            EstoqueAtual = 10,
            CategoriaId = 1
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Nome)
              .WithErrorCode("INVALID_PRODUTO_NOME");
    }

    [Fact]
    public void Deve_Passar_Quando_Dados_Forem_Válidos()
    {
        var request = new CriarProdutoRequest
        {
            Nome = "Mouse Gamer",
            Descricao = "Mouse com 6 botões programáveis",
            Preco = 249.90m,
            EstoqueAtual = 50,
            CategoriaId = 2
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Deve_Apresentar_Erro_Quando_Preco_For_Maior_Que_Limite()
    {
        var request = new CriarProdutoRequest
        {
            Nome = "Produto Caro",
            Descricao = "Descrição",
            Preco = 1_000_000m,
            EstoqueAtual = 1,
            CategoriaId = 1
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Preco)
              .WithErrorCode("INVALID_PRODUTO_PRECO");
    }

    [Fact]
    public void Deve_Apresentar_Erro_Quando_Estoque_For_Negativo()
    {
        var request = new CriarProdutoRequest
        {
            Nome = "Produto",
            Descricao = "Descrição",
            Preco = 10,
            EstoqueAtual = -1,
            CategoriaId = 1
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.EstoqueAtual)
              .WithErrorCode("INVALID_PRODUTO_ESTOQUE");
    }
}
