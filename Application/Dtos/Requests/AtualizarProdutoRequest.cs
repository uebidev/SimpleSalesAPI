using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Dtos.Requests
{
	public class AtualizarProdutoRequest
	{
		public string Nome { get; set; } = string.Empty;
		public string Descricao { get; set; } = string.Empty;
		public decimal Preco { get; set; }
		public int EstoqueAtual { get; set; }
		public int CategoriaId { get; set; }
	}

}
