using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SimpleSalesAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Endereco = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    EstoqueAtual = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produtos_Categoria",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Vendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    DataVenda = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0.00m),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendas_Cliente",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItensVenda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VendaId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensVenda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensVenda_Produto",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensVenda_Venda",
                        column: x => x.VendaId,
                        principalTable: "Vendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Descricao", "Nome" },
                values: new object[,]
                {
                    { 1, "Smartphones, tablets, notebooks e acessórios tecnológicos", "Eletrônicos" },
                    { 2, "Roupas, calçados e acessórios de moda", "Vestuário" },
                    { 3, "Livros, e-books, filmes e materiais educativos", "Livros e Mídia" },
                    { 4, "Móveis, decoração e utensílios domésticos", "Casa e Decoração" },
                    { 5, "Equipamentos esportivos e suplementos", "Esportes e Fitness" },
                    { 6, "Cosméticos, perfumes e produtos de higiene", "Beleza e Cuidados" },
                    { 7, "Alimentos, bebidas e produtos gourmet", "Alimentação" },
                    { 8, "Peças, acessórios e ferramentas automotivas", "Automotivo" }
                });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Email", "Endereco", "Nome", "Telefone" },
                values: new object[,]
                {
                    { 1, "ana.carolina@gmail.com", "Rua das Flores, 234 - Vila Madalena, São Paulo - SP", "Ana Carolina Silva", "(11) 98765-4321" },
                    { 2, "carlos.eduardo@hotmail.com", "Avenida Atlântica, 1500 - Copacabana, Rio de Janeiro - RJ", "Carlos Eduardo Santos", "(21) 97654-3210" },
                    { 3, "mariajose@outlook.com", "Rua da Liberdade, 789 - Centro, Belo Horizonte - MG", "Maria José Oliveira", "(31) 99876-5432" },
                    { 4, "joaopedro@yahoo.com.br", "Rua XV de Novembro, 456 - Centro, Blumenau - SC", "João Pedro Rodrigues", "(47) 98123-4567" },
                    { 5, "fernanda.lima@gmail.com", "Avenida Beira Mar, 2100 - Meireles, Fortaleza - CE", "Fernanda Costa Lima", "(85) 97456-1234" },
                    { 6, "ricardo.almeida@uol.com.br", "SQN 308, Bloco A - Asa Norte, Brasília - DF", "Ricardo Almeida Pereira", "(61) 99345-6789" },
                    { 7, "juliana.ferreira@terra.com.br", "Rua da Praia, 1200 - Centro Histórico, Porto Alegre - RS", "Juliana Ferreira Martins", "(51) 98567-2341" },
                    { 8, "gabriel.souza@gmail.com", "Avenida T-4, 890 - Setor Bueno, Goiânia - GO", "Gabriel Henrique Souza", "(62) 97234-5678" }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Ativo", "CategoriaId", "Descricao", "EstoqueAtual", "Nome", "Preco" },
                values: new object[,]
                {
                    { 1, true, 1, "Smartphone Apple iPhone 15 com 128GB de armazenamento, câmera de 48MP e chip A16 Bionic", 25, "iPhone 15 128GB", 7499.99m },
                    { 2, true, 1, "Smartphone Samsung Galaxy S24 com 256GB, tela Dynamic AMOLED e câmera tripla de 50MP", 40, "Samsung Galaxy S24 256GB", 4999.99m },
                    { 3, true, 1, "Notebook Dell Inspiron 15.6\", Intel Core i5, 8GB RAM, SSD 256GB, Windows 11", 15, "Notebook Dell Inspiron 15", 2899.99m },
                    { 4, true, 1, "Tablet Apple iPad Air com chip M1, tela Liquid Retina de 10.9\", 64GB Wi-Fi", 20, "iPad Air 5ª Geração", 4199.99m },
                    { 5, true, 2, "Camiseta esportiva Nike Dri-FIT, tecido respirável, disponível em várias cores", 150, "Camiseta Nike Dri-FIT", 129.99m },
                    { 6, true, 2, "Calça jeans Levi's 501 Original Fit, 100% algodão, corte clássico", 80, "Jeans Levi's 501 Original", 349.99m },
                    { 7, true, 2, "Tênis de corrida Adidas Ultraboost 22, tecnologia BOOST, máximo conforto", 60, "Tênis Adidas Ultraboost 22", 899.99m },
                    { 8, true, 3, "Livro sobre práticas de programação limpa e desenvolvimento de software profissional", 45, "Clean Code - Robert C. Martin", 89.99m },
                    { 9, true, 3, "Guia essencial para desenvolvimento de software, de David Thomas e Andrew Hunt", 30, "O Programador Pragmático", 75.99m },
                    { 10, true, 4, "Cadeira ergonômica para escritório, regulagem de altura, apoio lombar, suporte para braços", 12, "Cadeira de Escritório Ergonômica", 1299.99m },
                    { 11, true, 4, "Mesa de centro em madeira maciça, design moderno, 120x60cm", 8, "Mesa de Centro Madeira Maciça", 899.99m },
                    { 12, true, 5, "Par de halteres ajustáveis de 5 a 20kg cada, ideais para treino em casa", 25, "Halteres Ajustáveis 20kg", 459.99m },
                    { 13, true, 5, "Suplemento Whey Protein Isolado, sabor chocolate, 1kg, alta pureza", 100, "Whey Protein Isolado 1kg", 189.99m },
                    { 14, true, 6, "Perfume feminino Natura Humor, fragrância floral frutal, 75ml", 70, "Perfume Natura Humor", 159.99m },
                    { 15, true, 6, "Shampoo L'Oréal Elseve reparação total 5, 400ml", 200, "Shampoo L'Oréal Elseve", 24.99m },
                    { 16, true, 7, "Café torrado e moído especial, torra média, embalagem 500g", 120, "Café Especial Pilão Gourmet", 32.99m },
                    { 17, true, 7, "Polpa de açaí premium congelada, sem açúcar, embalagem 1kg", 80, "Açaí Premium Congelado 1kg", 28.99m },
                    { 18, true, 8, "Óleo lubrificante para motor Castrol GTX 5W-30, embalagem 4 litros", 50, "Óleo Motor Castrol GTX 5W30", 89.99m },
                    { 19, true, 8, "Pneu para carro de passeio Michelin Primacy 4, medida 205/55 R16", 30, "Pneu Michelin Primacy 4", 679.99m }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "CategoriaId", "Descricao", "EstoqueAtual", "Nome", "Preco" },
                values: new object[] { 20, 1, "Smartphone Xiaomi Mi 11 Lite 5G, 128GB, câmera tripla 64MP - DESCONTINUADO", 5, "Smartphone Xiaomi Mi 11 Lite", 1899.99m });

            migrationBuilder.InsertData(
                table: "Vendas",
                columns: new[] { "Id", "ClienteId", "DataVenda", "Status", "ValorTotal" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 8, 15, 14, 30, 0, 0, DateTimeKind.Unspecified), "Entregue", 7629.98m },
                    { 2, 2, new DateTime(2024, 8, 20, 10, 15, 0, 0, DateTimeKind.Unspecified), "Entregue", 259.98m },
                    { 3, 3, new DateTime(2024, 9, 1, 16, 45, 0, 0, DateTimeKind.Unspecified), "Confirmada", 2899.99m },
                    { 4, 4, new DateTime(2024, 9, 5, 9, 20, 0, 0, DateTimeKind.Unspecified), "Pendente", 1549.98m }
                });

            migrationBuilder.InsertData(
                table: "ItensVenda",
                columns: new[] { "Id", "PrecoUnitario", "ProdutoId", "Quantidade", "VendaId" },
                values: new object[,]
                {
                    { 1, 7499.99m, 1, 1, 1 },
                    { 2, 129.99m, 5, 1, 1 },
                    { 3, 129.99m, 5, 2, 2 },
                    { 4, 2899.99m, 3, 1, 3 },
                    { 5, 1299.99m, 10, 1, 4 },
                    { 6, 189.99m, 13, 1, 4 },
                    { 7, 32.99m, 16, 2, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nome",
                table: "Categorias",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Email",
                table: "Clientes",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Nome",
                table: "Clientes",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_ItensVenda_Produto",
                table: "ItensVenda",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensVenda_Venda",
                table: "ItensVenda",
                column: "VendaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensVenda_Venda_Produto",
                table: "ItensVenda",
                columns: new[] { "VendaId", "ProdutoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_Ativo",
                table: "Produtos",
                column: "Ativo");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_Categoria",
                table: "Produtos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_Categoria_Ativo",
                table: "Produtos",
                columns: new[] { "CategoriaId", "Ativo" });

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_Nome",
                table: "Produtos",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_Cliente",
                table: "Vendas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_Data",
                table: "Vendas",
                column: "DataVenda");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_Data_Status",
                table: "Vendas",
                columns: new[] { "DataVenda", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_Status",
                table: "Vendas",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensVenda");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Vendas");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
