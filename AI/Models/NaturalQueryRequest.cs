using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.AI.Models
{
	public class NaturalQueryRequest
	{
		[Required(ErrorMessage = "Query é obrigatória!")]
		[StringLength(500, ErrorMessage = "Query muito longa! Seja mais conciso.")]
		public string Query { get; set; } = string.Empty;

		public QueryFormat Format { get; set; } = QueryFormat.LinqMethod;
	}
}
