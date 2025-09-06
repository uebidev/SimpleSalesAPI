using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Dtos.Requests
{
	public class ItemVendaRequest
	{
		public int ProdutoId { get; set; }
		public int Quantidade { get; set; }
	}
}
