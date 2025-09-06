using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Dtos.Requests
{
	public class CriarVendaRequest
	{
		public int ClienteId { get; set; }
		public List<ItemVendaRequest> Itens { get; set; }
	}
}
