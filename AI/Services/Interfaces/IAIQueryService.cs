using SimpleSalesAPI.AI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.AI.Services.Interfaces
{
	public interface IAIQueryService
	{
		Task<AIQueryResponse> ProcessNaturalQueryAsync(string naturalQuery, QueryFormat format = QueryFormat.LinqMethod);
		Task<AIQueryResponse> ProcessNaturalQueryWithCacheAsync(string naturalQuery, QueryFormat format = QueryFormat.LinqMethod);
	}
}
