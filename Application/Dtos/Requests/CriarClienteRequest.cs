using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Dtos.Requests
{
	public class CriarClienteRequest
	{
		public string Nome { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Telefone { get; set; } = string.Empty;
		public string Endereco { get; set; } = string.Empty;
	}
}
