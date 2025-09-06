using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Dtos.Responses
{
	public class VendaResponse
	{
		public int Id { get; set; }
		public ClienteResumoResponse Cliente { get; set; } = new();
		public DateTime DataVenda { get; set; }
		public decimal ValorTotal { get; set; }
		public string Status { get; set; } = string.Empty;
		public List<ItemVendaResponse> Itens { get; set; } = new();
	}
}
