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
	public class ItemVendaConfiguration : IEntityTypeConfiguration<ItemVenda>
	{
		public void Configure(EntityTypeBuilder<ItemVenda> builder)
		{
			builder.ToTable("ItensVenda");
			builder.HasKey(i => i.Id);

			builder.Property(i => i.Id)
				.ValueGeneratedOnAdd();

			builder.Property(i => i.Quantidade)
				.IsRequired();

			builder.Property(i => i.PrecoUnitario)
				.IsRequired()
				.HasColumnType("decimal(10,2)")
				.HasPrecision(10, 2);

			builder.Property(i => i.VendaId)
				.IsRequired();

			builder.Property(i => i.ProdutoId)
				.IsRequired();

			builder.Ignore(i => i.Subtotal);
		
			builder.HasIndex(i => i.VendaId)
				.HasDatabaseName("IX_ItensVenda_Venda");

			builder.HasIndex(i => i.ProdutoId)
				.HasDatabaseName("IX_ItensVenda_Produto");

			// Índice único composto - Produto único por venda
			builder.HasIndex(i => new { i.VendaId, i.ProdutoId })
				.IsUnique()
				.HasDatabaseName("IX_ItensVenda_Venda_Produto");

			// Relacionamentos
			builder.HasOne(i => i.Venda)
				.WithMany(v => v.Itens)
				.HasForeignKey(i => i.VendaId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ItensVenda_Venda");

			builder.HasOne(i => i.Produto)
				.WithMany()
				.HasForeignKey(i => i.ProdutoId)
				.OnDelete(DeleteBehavior.Restrict)
				.HasConstraintName("FK_ItensVenda_Produto");
		}
	}
}
