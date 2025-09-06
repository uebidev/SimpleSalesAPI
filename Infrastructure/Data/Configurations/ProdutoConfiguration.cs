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
	public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
	{
		public void Configure(EntityTypeBuilder<Produto> builder)
		{
			builder.ToTable("Produtos");
			builder.HasKey(p => p.Id);

			builder.Property(p => p.Id)
				.ValueGeneratedOnAdd();

			builder.Property(p => p.Nome)
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(p => p.Descricao)
				.HasMaxLength(500);

			builder.Property(p => p.Preco)
				.IsRequired()
				.HasColumnType("decimal(10,2)")
				.HasPrecision(10, 2);

			builder.Property(p => p.EstoqueAtual)
				.IsRequired()
				.HasDefaultValue(0);

			builder.Property(p => p.Ativo)
				.IsRequired()
				.HasDefaultValue(true);

			builder.Property(p => p.CategoriaId)
				.IsRequired();

			builder.HasIndex(p => p.Nome)
				.HasDatabaseName("IX_Produtos_Nome");

			builder.HasIndex(p => p.CategoriaId)
				.HasDatabaseName("IX_Produtos_Categoria");

			builder.HasIndex(p => p.Ativo)
				.HasDatabaseName("IX_Produtos_Ativo");

			// Índice composto 
			builder.HasIndex(p => new { p.CategoriaId, p.Ativo })
				.HasDatabaseName("IX_Produtos_Categoria_Ativo");

			// Relacionamento explícito
			builder.HasOne(p => p.Categoria)
				.WithMany(c => c.Produtos)
				.HasForeignKey(p => p.CategoriaId)
				.OnDelete(DeleteBehavior.Restrict)
				.HasConstraintName("FK_Produtos_Categoria");
		}
	}
}
