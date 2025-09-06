using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleSalesAPI.Domain.Enums;
using SimpleSalesAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

			ConfigureGlobalSettings(modelBuilder);

			SeedInitialData(modelBuilder);
		}
		private static void ConfigureGlobalSettings(ModelBuilder modelBuilder)
		{
			// Configuração global para strings 
			foreach (var entity in modelBuilder.Model.GetEntityTypes())
			{
				foreach (var property in entity.GetProperties())
				{
					if (property.ClrType == typeof(string) && property.GetMaxLength() == null)
					{
						property.SetMaxLength(255); 
					}
				}
			}
			// Configuração global para decimais 
			foreach (var entity in modelBuilder.Model.GetEntityTypes())
			{
				foreach (var property in entity.GetProperties())
				{
					if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
					{
						if (property.GetPrecision() == null)
						{
							property.SetPrecision(18);
							property.SetScale(2);
						}
					}
				}
			}
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder
					.EnableSensitiveDataLogging(false)
					.EnableDetailedErrors(false)
					.LogTo(Console.WriteLine, LogLevel.Warning);
			}
		}
		/// <summary>
		/// Dados iniciais para desenvolvimento e testes, ia usar o cocos, mas como é um projeto simples, vou fazer direto aqui
		/// </summary>
		private static void SeedInitialData(ModelBuilder modelBuilder)
		{
			// ========================================
			// CATEGORIAS
			// ========================================
			modelBuilder.Entity<Categoria>().HasData(
				new Categoria { Id = 1, Nome = "Eletrônicos", Descricao = "Smartphones, tablets, notebooks e acessórios tecnológicos" },
				new Categoria { Id = 2, Nome = "Vestuário", Descricao = "Roupas, calçados e acessórios de moda" },
				new Categoria { Id = 3, Nome = "Livros e Mídia", Descricao = "Livros, e-books, filmes e materiais educativos" },
				new Categoria { Id = 4, Nome = "Casa e Decoração", Descricao = "Móveis, decoração e utensílios domésticos" },
				new Categoria { Id = 5, Nome = "Esportes e Fitness", Descricao = "Equipamentos esportivos e suplementos" },
				new Categoria { Id = 6, Nome = "Beleza e Cuidados", Descricao = "Cosméticos, perfumes e produtos de higiene" },
				new Categoria { Id = 7, Nome = "Alimentação", Descricao = "Alimentos, bebidas e produtos gourmet" },
				new Categoria { Id = 8, Nome = "Automotivo", Descricao = "Peças, acessórios e ferramentas automotivas" }
			);

			// ========================================
			// CLIENTES
			// ========================================
			modelBuilder.Entity<Cliente>().HasData(
				new Cliente
				{
					Id = 1,
					Nome = "Ana Carolina Silva",
					Email = "ana.carolina@gmail.com",
					Telefone = "(11) 98765-4321",
					Endereco = "Rua das Flores, 234 - Vila Madalena, São Paulo - SP"
				},
				new Cliente
				{
					Id = 2,
					Nome = "Carlos Eduardo Santos",
					Email = "carlos.eduardo@hotmail.com",
					Telefone = "(21) 97654-3210",
					Endereco = "Avenida Atlântica, 1500 - Copacabana, Rio de Janeiro - RJ"
				},
				new Cliente
				{
					Id = 3,
					Nome = "Maria José Oliveira",
					Email = "mariajose@outlook.com",
					Telefone = "(31) 99876-5432",
					Endereco = "Rua da Liberdade, 789 - Centro, Belo Horizonte - MG"
				},
				new Cliente
				{
					Id = 4,
					Nome = "João Pedro Rodrigues",
					Email = "joaopedro@yahoo.com.br",
					Telefone = "(47) 98123-4567",
					Endereco = "Rua XV de Novembro, 456 - Centro, Blumenau - SC"
				},
				new Cliente
				{
					Id = 5,
					Nome = "Fernanda Costa Lima",
					Email = "fernanda.lima@gmail.com",
					Telefone = "(85) 97456-1234",
					Endereco = "Avenida Beira Mar, 2100 - Meireles, Fortaleza - CE"
				},
				new Cliente
				{
					Id = 6,
					Nome = "Ricardo Almeida Pereira",
					Email = "ricardo.almeida@uol.com.br",
					Telefone = "(61) 99345-6789",
					Endereco = "SQN 308, Bloco A - Asa Norte, Brasília - DF"
				},
				new Cliente
				{
					Id = 7,
					Nome = "Juliana Ferreira Martins",
					Email = "juliana.ferreira@terra.com.br",
					Telefone = "(51) 98567-2341",
					Endereco = "Rua da Praia, 1200 - Centro Histórico, Porto Alegre - RS"
				},
				new Cliente
				{
					Id = 8,
					Nome = "Gabriel Henrique Souza",
					Email = "gabriel.souza@gmail.com",
					Telefone = "(62) 97234-5678",
					Endereco = "Avenida T-4, 890 - Setor Bueno, Goiânia - GO"
				}
			);

			// ========================================
			// PRODUTOS 
			// ========================================
			modelBuilder.Entity<Produto>().HasData(
				// ELETRÔNICOS
				new Produto
				{
					Id = 1,
					Nome = "iPhone 15 128GB",
					Descricao = "Smartphone Apple iPhone 15 com 128GB de armazenamento, câmera de 48MP e chip A16 Bionic",
					Preco = 7499.99m,
					EstoqueAtual = 25,
					Ativo = true,
					CategoriaId = 1
				},
				new Produto
				{
					Id = 2,
					Nome = "Samsung Galaxy S24 256GB",
					Descricao = "Smartphone Samsung Galaxy S24 com 256GB, tela Dynamic AMOLED e câmera tripla de 50MP",
					Preco = 4999.99m,
					EstoqueAtual = 40,
					Ativo = true,
					CategoriaId = 1
				},
				new Produto
				{
					Id = 3,
					Nome = "Notebook Dell Inspiron 15",
					Descricao = "Notebook Dell Inspiron 15.6\", Intel Core i5, 8GB RAM, SSD 256GB, Windows 11",
					Preco = 2899.99m,
					EstoqueAtual = 15,
					Ativo = true,
					CategoriaId = 1
				},
				new Produto
				{
					Id = 4,
					Nome = "iPad Air 5ª Geração",
					Descricao = "Tablet Apple iPad Air com chip M1, tela Liquid Retina de 10.9\", 64GB Wi-Fi",
					Preco = 4199.99m,
					EstoqueAtual = 20,
					Ativo = true,
					CategoriaId = 1
				},

				// VESTUÁRIO
				new Produto
				{
					Id = 5,
					Nome = "Camiseta Nike Dri-FIT",
					Descricao = "Camiseta esportiva Nike Dri-FIT, tecido respirável, disponível em várias cores",
					Preco = 129.99m,
					EstoqueAtual = 150,
					Ativo = true,
					CategoriaId = 2
				},
				new Produto
				{
					Id = 6,
					Nome = "Jeans Levi's 501 Original",
					Descricao = "Calça jeans Levi's 501 Original Fit, 100% algodão, corte clássico",
					Preco = 349.99m,
					EstoqueAtual = 80,
					Ativo = true,
					CategoriaId = 2
				},
				new Produto
				{
					Id = 7,
					Nome = "Tênis Adidas Ultraboost 22",
					Descricao = "Tênis de corrida Adidas Ultraboost 22, tecnologia BOOST, máximo conforto",
					Preco = 899.99m,
					EstoqueAtual = 60,
					Ativo = true,
					CategoriaId = 2
				},

				// LIVROS E MÍDIA
				new Produto
				{
					Id = 8,
					Nome = "Clean Code - Robert C. Martin",
					Descricao = "Livro sobre práticas de programação limpa e desenvolvimento de software profissional",
					Preco = 89.99m,
					EstoqueAtual = 45,
					Ativo = true,
					CategoriaId = 3
				},
				new Produto
				{
					Id = 9,
					Nome = "O Programador Pragmático",
					Descricao = "Guia essencial para desenvolvimento de software, de David Thomas e Andrew Hunt",
					Preco = 75.99m,
					EstoqueAtual = 30,
					Ativo = true,
					CategoriaId = 3
				},

				// CASA E DECORAÇÃO
				new Produto
				{
					Id = 10,
					Nome = "Cadeira de Escritório Ergonômica",
					Descricao = "Cadeira ergonômica para escritório, regulagem de altura, apoio lombar, suporte para braços",
					Preco = 1299.99m,
					EstoqueAtual = 12,
					Ativo = true,
					CategoriaId = 4
				},
				new Produto
				{
					Id = 11,
					Nome = "Mesa de Centro Madeira Maciça",
					Descricao = "Mesa de centro em madeira maciça, design moderno, 120x60cm",
					Preco = 899.99m,
					EstoqueAtual = 8,
					Ativo = true,
					CategoriaId = 4
				},

				// ESPORTES E FITNESS
				new Produto
				{
					Id = 12,
					Nome = "Halteres Ajustáveis 20kg",
					Descricao = "Par de halteres ajustáveis de 5 a 20kg cada, ideais para treino em casa",
					Preco = 459.99m,
					EstoqueAtual = 25,
					Ativo = true,
					CategoriaId = 5
				},
				new Produto
				{
					Id = 13,
					Nome = "Whey Protein Isolado 1kg",
					Descricao = "Suplemento Whey Protein Isolado, sabor chocolate, 1kg, alta pureza",
					Preco = 189.99m,
					EstoqueAtual = 100,
					Ativo = true,
					CategoriaId = 5
				},

				// BELEZA E CUIDADOS
				new Produto
				{
					Id = 14,
					Nome = "Perfume Natura Humor",
					Descricao = "Perfume feminino Natura Humor, fragrância floral frutal, 75ml",
					Preco = 159.99m,
					EstoqueAtual = 70,
					Ativo = true,
					CategoriaId = 6
				},
				new Produto
				{
					Id = 15,
					Nome = "Shampoo L'Oréal Elseve",
					Descricao = "Shampoo L'Oréal Elseve reparação total 5, 400ml",
					Preco = 24.99m,
					EstoqueAtual = 200,
					Ativo = true,
					CategoriaId = 6
				},

				// ALIMENTAÇÃO
				new Produto
				{
					Id = 16,
					Nome = "Café Especial Pilão Gourmet",
					Descricao = "Café torrado e moído especial, torra média, embalagem 500g",
					Preco = 32.99m,
					EstoqueAtual = 120,
					Ativo = true,
					CategoriaId = 7
				},
				new Produto
				{
					Id = 17,
					Nome = "Açaí Premium Congelado 1kg",
					Descricao = "Polpa de açaí premium congelada, sem açúcar, embalagem 1kg",
					Preco = 28.99m,
					EstoqueAtual = 80,
					Ativo = true,
					CategoriaId = 7
				},

				// AUTOMOTIVO
				new Produto
				{
					Id = 18,
					Nome = "Óleo Motor Castrol GTX 5W30",
					Descricao = "Óleo lubrificante para motor Castrol GTX 5W-30, embalagem 4 litros",
					Preco = 89.99m,
					EstoqueAtual = 50,
					Ativo = true,
					CategoriaId = 8
				},
				new Produto
				{
					Id = 19,
					Nome = "Pneu Michelin Primacy 4",
					Descricao = "Pneu para carro de passeio Michelin Primacy 4, medida 205/55 R16",
					Preco = 679.99m,
					EstoqueAtual = 30,
					Ativo = true,
					CategoriaId = 8
				},

				// PRODUTOS SAZONAIS/DESCONTINUADOS
				new Produto
				{
					Id = 20,
					Nome = "Smartphone Xiaomi Mi 11 Lite",
					Descricao = "Smartphone Xiaomi Mi 11 Lite 5G, 128GB, câmera tripla 64MP - DESCONTINUADO",
					Preco = 1899.99m,
					EstoqueAtual = 5,
					Ativo = false, // Produto descontinuado
					CategoriaId = 1
				}
			);

			// ========================================
			// VENDAS 
			// ========================================
			modelBuilder.Entity<Venda>().HasData(
				new Venda
				{
					Id = 1,
					ClienteId = 1,
					DataVenda = new DateTime(2024, 8, 15, 14, 30, 0),
					ValorTotal = 7629.98m,
					Status = StatusVenda.Entregue
				},
				new Venda
				{
					Id = 2,
					ClienteId = 2,
					DataVenda = new DateTime(2024, 8, 20, 10, 15, 0),
					ValorTotal = 259.98m,
					Status = StatusVenda.Entregue
				},
				new Venda
				{
					Id = 3,
					ClienteId = 3,
					DataVenda = new DateTime(2024, 9, 1, 16, 45, 0),
					ValorTotal = 2899.99m,
					Status = StatusVenda.Confirmada
				},
				new Venda
				{
					Id = 4,
					ClienteId = 4,
					DataVenda = new DateTime(2024, 9, 5, 9, 20, 0),
					ValorTotal = 1549.98m,
					Status = StatusVenda.Pendente
				}
			);

			// ========================================
			// ITENS DE VENDA 
			// ========================================
			modelBuilder.Entity<ItemVenda>().HasData(
				// Venda 1: Ana Carolina comprou iPhone + Camiseta Nike
				new ItemVenda
				{
					Id = 1,
					VendaId = 1,
					ProdutoId = 1, // iPhone 15
					Quantidade = 1,
					PrecoUnitario = 7499.99m
				},
				new ItemVenda
				{
					Id = 2,
					VendaId = 1,
					ProdutoId = 5, // Camiseta Nike
					Quantidade = 1,
					PrecoUnitario = 129.99m
				},

				// Venda 2: Carlos comprou 2 Camisetas Nike
				new ItemVenda
				{
					Id = 3,
					VendaId = 2,
					ProdutoId = 5, // Camiseta Nike
					Quantidade = 2,
					PrecoUnitario = 129.99m
				},

				// Venda 3: Maria José comprou Notebook Dell
				new ItemVenda
				{
					Id = 4,
					VendaId = 3,
					ProdutoId = 3, // Notebook Dell
					Quantidade = 1,
					PrecoUnitario = 2899.99m
				},

				// Venda 4: João Pedro comprou Cadeira + Whey Protein
				new ItemVenda
				{
					Id = 5,
					VendaId = 4,
					ProdutoId = 10, // Cadeira Ergonômica
					Quantidade = 1,
					PrecoUnitario = 1299.99m
				},
				new ItemVenda
				{
					Id = 6,
					VendaId = 4,
					ProdutoId = 13, // Whey Protein
					Quantidade = 1,
					PrecoUnitario = 189.99m
				},
				new ItemVenda
				{
					Id = 7,
					VendaId = 4,
					ProdutoId = 16, // Café Gourmet  
					Quantidade = 2,
					PrecoUnitario = 32.99m
				}
			);
		}
	}
}
