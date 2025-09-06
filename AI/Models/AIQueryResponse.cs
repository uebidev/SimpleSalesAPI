using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.AI.Models
{
	public class AIQueryResponse
	{
		public bool Success { get; set; }
		public string GeneratedQuery { get; set; } = string.Empty;
		public string Intent { get; set; } = string.Empty;
		public string ErrorMessage { get; set; } = string.Empty;
		public List<string> Warnings { get; set; } = new();
		public Dictionary<string, object> ExtractedParameters { get; set; } = new();
	}
}
