using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Dtos.Responses
{

	public class ValidationErrorResponse : ErrorResponse
	{
		public Dictionary<string, string[]> Errors { get; set; } = new();
	}
}
