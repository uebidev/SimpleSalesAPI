using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.AI.Models
{
	public enum QueryFormat
	{
		LinqMethod,     // .Where(x => x.Id > 5).OrderBy(x => x.Nome)
		LinqQuery,      // from p in produtos where p.Id > 5 select p
		RawSQL          // SELECT * FROM Produtos WHERE Id > 5
	}
}
