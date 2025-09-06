using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleSalesAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Infrastructure.Data.Configurations
{
	public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
	{
		public void Configure(EntityTypeBuilder<Categoria> builder)
		{
			builder.ToTable("Categorias");
			builder.HasKey(c => c.Id);

			builder.Property(c => c.Id)
				.ValueGeneratedOnAdd();

			builder.Property(c => c.Nome)
				.IsRequired()
				.HasMaxLength(50);

			builder.Property(c => c.Descricao)
				.HasMaxLength(200);

			// Índice único para nome
			builder.HasIndex(c => c.Nome)
				.IsUnique()
				.HasDatabaseName("IX_Categorias_Nome");

			// Relacionamento com Produtos
			builder.HasMany(c => c.Produtos)
				.WithOne(p => p.Categoria)
				.HasForeignKey(p => p.CategoriaId)
				.OnDelete(DeleteBehavior.Restrict)
				.HasConstraintName("FK_Produtos_Categoria");
		}
	}
}
