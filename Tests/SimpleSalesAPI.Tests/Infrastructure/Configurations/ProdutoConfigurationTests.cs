using System;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SimpleSalesAPI.Domain.Models;
using SimpleSalesAPI.Infrastructure.Data.Context;
using Xunit;

namespace SimpleSalesAPI.Tests.Infrastructure.Configurations;

public class ProdutoConfigurationTests
{
    [Fact]
    public void Deve_Configurar_Precisao_E_Valores_Padrao_Corretamente()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var entity = context.Model.FindEntityType(typeof(Produto))!;

        var preco = entity.FindProperty(nameof(Produto.Preco))!;
        preco.GetPrecision().Should().Be(10);
        preco.GetScale().Should().Be(2);

        var estoque = entity.FindProperty(nameof(Produto.EstoqueAtual))!;
        estoque.GetDefaultValue().Should().Be(0);

        var ativo = entity.FindProperty(nameof(Produto.Ativo))!;
        ativo.GetDefaultValue().Should().Be(true);

        entity.GetIndexes()
              .Should()
              .Contain(index => index.Properties.Select(p => p.Name)
                  .SequenceEqual(new[] { nameof(Produto.Nome) }));
    }
}
