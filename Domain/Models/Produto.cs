using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Domain.Models
{
	public class Produto
	{
		public int Id { get; set; }
		public string Nome { get; set; }
		public string Descricao { get; set; }
		public int CategoriaId { get; set; }       
		public Categoria Categoria { get; set; }
		public decimal Preco { get; set; }
		public int EstoqueAtual { get; set; }
		public bool Ativo { get; set; }
	}
}
