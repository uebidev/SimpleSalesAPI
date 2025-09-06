using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Dtos.Responses
{
	public class ClienteResumoResponse
	{
		public int Id { get; set; }
		public string Nome { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
	}
}
