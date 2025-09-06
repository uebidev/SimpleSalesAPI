using Microsoft.EntityFrameworkCore;
using SimpleSalesAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Infrastructure.Data.Context
{
	public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
	{
		public DbSet<Cliente> Clientes { get; set; }
		public DbSet<Categoria> Categorias { get; set; }
		public DbSet<Produto> Produtos { get; set; }
		public DbSet<Venda> Vendas { get; set; }
		public DbSet<ItemVenda> ItensVenda { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// apenas para garantir que os decimais sejam mapeados corretamente
			modelBuilder.Entity<Produto>()
				.Property(p => p.Preco)
				.HasColumnType("decimal(18,2)");

			modelBuilder.Entity<ItemVenda>()
				.Property(i => i.PrecoUnitario)
				.HasColumnType("decimal(18,2)");

			modelBuilder.Entity<Venda>()
				.Property(v => v.ValorTotal)
				.HasColumnType("decimal(18,2)");

		}
	}
}
