using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.AI.Models
{
	public class ClaudeApiRequest
	{
		public string Model { get; set; } = "claude-sonnet-4-20250514";
		public int Max_tokens { get; set; } = 1000;
		public List<ClaudeMessage> Messages { get; set; } = new();
	}
}
