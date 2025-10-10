using FluentValidation.TestHelper;
using SimpleSalesAPI.Application.Dtos.Requests;
using SimpleSalesAPI.Application.Validators;
using Xunit;

namespace SimpleSalesAPI.Tests.Validators;

public class CriarClienteRequestValidatorTests
{
    private readonly CriarClienteRequestValidator _validator = new();

    [Fact]
    public void Deve_Apresentar_Erro_Quando_Email_For_Invalido()
    {
        var request = new CriarClienteRequest
        {
            Nome = "João Silva",
            Email = "email-invalido",
            Telefone = "(11) 98765-4321",
            Endereco = "Rua A, 123"
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorCode("INVALID_CLIENTE_EMAIL");
    }

    [Fact]
    public void Deve_Apresentar_Erro_Quando_Telefone_For_Invalido()
    {
        var request = new CriarClienteRequest
        {
            Nome = "João Silva",
            Email = "joao@empresa.com",
            Telefone = "11987654321",
            Endereco = "Rua A, 123"
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Telefone)
              .WithErrorCode("INVALID_CLIENTE_TELEFONE");
    }

    [Fact]
    public void Deve_Passar_Quando_Dados_Forem_Válidos()
    {
        var request = new CriarClienteRequest
        {
            Nome = "João Silva",
            Email = "joao.silva@empresa.com",
            Telefone = "(11) 98765-4321",
            Endereco = "Rua A, 123"
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
