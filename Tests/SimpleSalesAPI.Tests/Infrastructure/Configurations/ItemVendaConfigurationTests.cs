using System;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SimpleSalesAPI.Domain.Models;
using SimpleSalesAPI.Infrastructure.Data.Context;
using Xunit;

namespace SimpleSalesAPI.Tests.Infrastructure.Configurations;

public class ItemVendaConfigurationTests
{
    [Fact]
    public void Deve_Configurar_PrecoUnitario_Com_Precisao_De_Duas_Casas()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var entity = context.Model.FindEntityType(typeof(ItemVenda))!;

        var precoUnitario = entity.FindProperty(nameof(ItemVenda.PrecoUnitario))!;
        precoUnitario.GetPrecision().Should().Be(10);
        precoUnitario.GetScale().Should().Be(2);
        precoUnitario.GetColumnType().Should().Be("decimal(10,2)");

        entity.GetForeignKeys()
              .Should()
              .Contain(fk => fk.PrincipalEntityType.ClrType == typeof(Venda));
    }
}
