using SimpleSalesAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Domain.Models
{
	public class Venda
	{
		public int Id { get; set; }
		public int ClienteId { get; set; }
		public Cliente Cliente { get; set; } 
		public DateTime DataVenda { get; set; }
		public decimal ValorTotal { get; set; }
		public StatusVenda Status { get; set; }
		public List<ItemVenda> Itens { get; set; } = new();
	}
}
