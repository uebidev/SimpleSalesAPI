using System;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SimpleSalesAPI.Domain.Models;
using SimpleSalesAPI.Infrastructure.Data.Context;
using Xunit;

namespace SimpleSalesAPI.Tests.Infrastructure.Configurations;

public class VendaConfigurationTests
{
    [Fact]
    public void Deve_Configurar_Campos_De_Valor_E_Status_Corretamente()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var entity = context.Model.FindEntityType(typeof(Venda))!;

        var valorTotal = entity.FindProperty(nameof(Venda.ValorTotal))!;
        valorTotal.GetPrecision().Should().Be(12);
        valorTotal.GetScale().Should().Be(2);
        valorTotal.GetDefaultValue().Should().Be(0.00m);

        var status = entity.FindProperty(nameof(Venda.Status))!;
        status.GetConversion().ProviderClrType.Should().Be(typeof(string));
        status.GetMaxLength().Should().Be(20);

        entity.GetForeignKeys()
              .Should()
              .Contain(fk => fk.Properties.Any(p => p.Name == nameof(Venda.ClienteId)) &&
                             fk.DeleteBehavior == DeleteBehavior.Restrict);
    }
}
