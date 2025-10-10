using System;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SimpleSalesAPI.Domain.Models;
using SimpleSalesAPI.Infrastructure.Data.Context;
using Xunit;

namespace SimpleSalesAPI.Tests.Infrastructure.Configurations;

public class CategoriaConfigurationTests
{
    [Fact]
    public void Deve_Definir_Indice_Unico_Para_Nome()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var entity = context.Model.FindEntityType(typeof(Categoria))!;

        entity.GetIndexes()
              .Should()
              .Contain(index => index.IsUnique &&
                               index.Properties.Select(p => p.Name)
                                    .SequenceEqual(new[] { nameof(Categoria.Nome) }));
    }
}
