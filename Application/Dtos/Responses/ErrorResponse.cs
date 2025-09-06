using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Application.Dtos.Responses
{
	public class ErrorResponse
	{
		public string Type { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public int Status { get; set; }
		public string Detail { get; set; } = string.Empty;
		public string Instance { get; set; } = string.Empty;
		public string TraceId { get; set; } = string.Empty;
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;
		public Dictionary<string, object>? Extensions { get; set; }
	}
}
