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
	public class VendaConfiguration : IEntityTypeConfiguration<Venda>
	{
		public void Configure(EntityTypeBuilder<Venda> builder)
		{
			builder.ToTable("Vendas");
			builder.HasKey(v => v.Id);

			builder.Property(v => v.Id)
				.ValueGeneratedOnAdd();


			builder.Property(v => v.DataVenda)
				.IsRequired()
				.HasColumnType("datetime(6)");


			builder.Property(v => v.ValorTotal)
				.IsRequired()
				.HasColumnType("decimal(12,2)")
				.HasPrecision(12, 2)
				.HasDefaultValue(0.00m);

			builder.Property(v => v.Status)
				.IsRequired()
				.HasConversion<string>()
				.HasMaxLength(20);

			builder.Property(v => v.ClienteId)
				.IsRequired();

			builder.HasIndex(v => v.ClienteId)
				.HasDatabaseName("IX_Vendas_Cliente");

			builder.HasIndex(v => v.DataVenda)
				.HasDatabaseName("IX_Vendas_Data");

			builder.HasIndex(v => v.Status)
				.HasDatabaseName("IX_Vendas_Status");

		
			builder.HasIndex(v => new { v.DataVenda, v.Status })
				.HasDatabaseName("IX_Vendas_Data_Status");

	
			builder.HasOne(v => v.Cliente)
				.WithMany()
				.HasForeignKey(v => v.ClienteId)
				.OnDelete(DeleteBehavior.Restrict)
				.HasConstraintName("FK_Vendas_Cliente");

			builder.HasMany(v => v.Itens)
				.WithOne(i => i.Venda)
				.HasForeignKey(i => i.VendaId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ItensVenda_Venda");
		}
	}
}
