using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimpleSalesAPI.AI.Models;
using SimpleSalesAPI.AI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.AI.Services
{
	public class ClaudeQueryService : IAIQueryService
	{
		private readonly HttpClient _httpClient;
		private readonly IMemoryCache _cache;
		private readonly ILogger<ClaudeQueryService> _logger;
		private readonly string _basePrompt;

		public ClaudeQueryService(HttpClient httpClient, IMemoryCache cache, ILogger<ClaudeQueryService> logger)
		{
			_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
			_cache = cache ?? throw new ArgumentNullException(nameof(cache));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			_basePrompt = CreateSystemPrompt();
		}

		public async Task<AIQueryResponse> ProcessNaturalQueryAsync(string naturalQuery, QueryFormat format = QueryFormat.LinqMethod)
		{
			try
			{
				_logger.LogInformation("Processando query natural: {Query}", naturalQuery);

				var prompt = BuildPrompt(naturalQuery, format);
				var claudeResponse = await CallClaudeApiAsync(prompt);

				return ParseClaudeResponse(claudeResponse, naturalQuery);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao processar query natural: {Query}", naturalQuery);

				return new AIQueryResponse
				{
					Success = false,
					ErrorMessage = "Erro interno ao processar query. Claudinha teve um bugzinho! 🤖💔"
				};
			}
		}

		public async Task<AIQueryResponse> ProcessNaturalQueryWithCacheAsync(string naturalQuery, QueryFormat format = QueryFormat.LinqMethod)
		{
			var cacheKey = $"ai_query_{naturalQuery}_{format}".ToLowerInvariant();

			if (_cache.TryGetValue(cacheKey, out AIQueryResponse? cachedResult))
			{
				_logger.LogInformation("Cache hit para query: {Query}", naturalQuery);
				return cachedResult!;
			}

			var result = await ProcessNaturalQueryAsync(naturalQuery, format);

			if (result.Success)
			{
				_cache.Set(cacheKey, result, TimeSpan.FromMinutes(30)); // Cache por 30 minutos
				_logger.LogInformation("Query cacheada: {Query}", naturalQuery);
			}

			return result;
		}

		private string CreateSystemPrompt()
		{
			return @"
				Você é um especialista em C# e Entity Framework que gera consultas LINQ baseadas em linguagem natural.

				ENTIDADES DO SISTEMA SimpleSalesAPI:

				```csharp
				public class Cliente
				{
					public int Id { get; set; }
					public string Nome { get; set; }
					public string Email { get; set; }
					public string Telefone { get; set; }
					public DateTime DataCadastro { get; set; }
				}

				public class Produto  
				{
					public int Id { get; set; }
					public string Nome { get; set; }
					public string Descricao { get; set; }
					public decimal Preco { get; set; }
					public int QuantidadeEstoque { get; set; }
					public DateTime DataCriacao { get; set; }
				}

				public class Venda
				{
					public int Id { get; set; }
					public int ClienteId { get; set; }
					public DateTime DataVenda { get; set; }
					public decimal ValorTotal { get; set; }
					public StatusVenda Status { get; set; }
					public Cliente Cliente { get; set; }
					public List<ItemVenda> Itens { get; set; }
				}

				public class ItemVenda
				{
					public int Id { get; set; }
					public int VendaId { get; set; }
					public int ProdutoId { get; set; }
					public int Quantidade { get; set; }
					public decimal PrecoUnitario { get; set; }
					public Venda Venda { get; set; }
					public Produto Produto { get; set; }
				}

				public enum StatusVenda { Pendente, Aprovada, Cancelada, Entregue }
				```

				RELACIONAMENTOS:
				- Venda.ClienteId → Cliente.Id (Many-to-One)
				- ItemVenda.VendaId → Venda.Id (Many-to-One)  
				- ItemVenda.ProdutoId → Produto.Id (Many-to-One)
				- Venda.Itens → List<ItemVenda> (One-to-Many)

				INSTRUÇÕES:
				1. Responda APENAS com a query LINQ solicitada
				2. Use nomes de DbSet: context.Clientes, context.Produtos, context.Vendas, context.ItensVenda
				3. Para relacionamentos, use Include() quando necessário
				4. Não inclua using statements ou declarações de variáveis
				5. Seja preciso com os tipos de dados e nomes das propriedades
				6. Se a query for ambígua, escolha a interpretação mais provável

				EXEMPLOS:
				Input: 'produtos mais vendidos'
				Output: context.ItensVenda.GroupBy(i => i.Produto).OrderByDescending(g => g.Sum(i => i.Quantidade)).Select(g => g.Key)

				Input: 'vendas do cliente João'  
				Output: context.Vendas.Include(v => v.Cliente).Where(v => v.Cliente.Nome.Contains(""João""))

				Input: 'produtos com estoque baixo'
				Output: context.Produtos.Where(p => p.QuantidadeEstoque < 10)
				";
		}

		private string BuildPrompt(string naturalQuery, QueryFormat format)
		{
			var formatInstruction = format switch
			{
				QueryFormat.LinqMethod => "Gere uma consulta LINQ usando sintaxe de método (method syntax)",
				QueryFormat.LinqQuery => "Gere uma consulta LINQ usando sintaxe de query (query syntax)",
				QueryFormat.RawSQL => "Gere uma consulta SQL raw",
				_ => "Gere uma consulta LINQ usando sintaxe de método"
			};

			return $@"{_basePrompt}

			FORMATO SOLICITADO: {formatInstruction}

			QUERY NATURAL DO USUÁRIO: ""{naturalQuery}""

			Responda apenas com a consulta gerada:";
		}

		private async Task<string> CallClaudeApiAsync(string prompt)
		{
			var request = new ClaudeApiRequest
			{
				Model = "claude-sonnet-4-20250514",
				Max_tokens = 1000,
				Messages = new List<ClaudeMessage>
				{
					new ClaudeMessage { Role = "user", Content = prompt }
				}
			};

			var json = JsonConvert.SerializeObject(request);
			var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

			var response = await _httpClient.PostAsync("https://api.anthropic.com/v1/messages", content);

			if (!response.IsSuccessStatusCode)
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				_logger.LogError("Erro na API Claude: {StatusCode} - {Content}", response.StatusCode, errorContent);
				throw new HttpRequestException($"Claude API error: {response.StatusCode}");
			}

			var responseContent = await response.Content.ReadAsStringAsync();
			return responseContent;
		}

		private AIQueryResponse ParseClaudeResponse(string claudeResponseJson, string originalQuery)
		{
			try
			{
				var claudeResponse = JsonConvert.DeserializeObject<ClaudeApiResponse>(claudeResponseJson);

				if (claudeResponse?.Content?.FirstOrDefault()?.Text == null)
				{
					return new AIQueryResponse
					{
						Success = false,
						ErrorMessage = "Claudinha não conseguiu gerar uma resposta válida! 🤖❌"
					};
				}

				var generatedQuery = claudeResponse.Content.First().Text.Trim();

				// Validações básicas de segurança
				var warnings = ValidateGeneratedQuery(generatedQuery);

				return new AIQueryResponse
				{
					Success = true,
					GeneratedQuery = generatedQuery,
					Intent = ExtractIntent(originalQuery),
					Warnings = warnings,
					ExtractedParameters = ExtractParameters(originalQuery)
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao parse da resposta do Claude");

				return new AIQueryResponse
				{
					Success = false,
					ErrorMessage = "Erro ao interpretar resposta da Claudinha! 💔"
				};
			}
		}

		private List<string> ValidateGeneratedQuery(string query)
		{
			var warnings = new List<string>();

			// Validações de segurança básicas
			var dangerousPatterns = new[] { "DROP", "DELETE", "UPDATE", "INSERT", "ALTER", "EXEC", "EXECUTE" };

			foreach (var pattern in dangerousPatterns)
			{
				if (query.ToUpperInvariant().Contains(pattern))
				{
					warnings.Add($"⚠️ Query contém padrão potencialmente perigoso: {pattern}");
				}
			}

			// Validação de entidades conhecidas
			var validEntities = new[] { "Clientes", "Produtos", "Vendas", "ItensVenda" };
			var hasValidEntity = validEntities.Any(entity => query.Contains(entity));

			if (!hasValidEntity)
			{
				warnings.Add("⚠️ Query não referencia nenhuma entidade conhecida");
			}

			return warnings;
		}

		private string ExtractIntent(string naturalQuery)
		{
			var query = naturalQuery.ToLowerInvariant();

			if (query.Contains("mais vendido") || query.Contains("top") || query.Contains("melhor"))
				return "top_selling_products";

			if (query.Contains("cliente") && (query.Contains("pedido") || query.Contains("venda")))
				return "customer_orders";

			if (query.Contains("estoque"))
				return "stock_query";

			if (query.Contains("vendas") && query.Contains("data"))
				return "sales_by_date";

			return "general_query";
		}

		private Dictionary<string, object> ExtractParameters(string naturalQuery)
		{
			var parameters = new Dictionary<string, object>();

			// Extração básica de parâmetros (pode ser expandida)
			var query = naturalQuery.ToLowerInvariant();

			if (query.Contains("últimos") || query.Contains("ultimo"))
				parameters["time_range"] = "recent";

			if (query.Contains("hoje"))
				parameters["date"] = "today";

			return parameters;
		}
	}
}
