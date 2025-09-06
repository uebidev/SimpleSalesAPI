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
	public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
	{
		public void Configure(EntityTypeBuilder<Cliente> builder)
		{
			builder.ToTable("Clientes");
			builder.HasKey(c => c.Id);

			builder.Property(c => c.Id)
				.ValueGeneratedOnAdd();

			builder.Property(c => c.Nome)
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(c => c.Email)
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(c => c.Telefone)
				.IsRequired()
				.HasMaxLength(20);

			builder.Property(c => c.Endereco)
				.HasMaxLength(200);

			// ÍNDICES PARA PERFORMANCE
			builder.HasIndex(c => c.Email)
				.IsUnique()
				.HasDatabaseName("IX_Clientes_Email");

			builder.HasIndex(c => c.Nome)
				.HasDatabaseName("IX_Clientes_Nome");
		}
	}
}
