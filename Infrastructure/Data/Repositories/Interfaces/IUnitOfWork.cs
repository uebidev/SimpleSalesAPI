using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Infrastructure.Data.Repositories.Interfaces
{
	public interface IUnitOfWork : IDisposable, IAsyncDisposable
	{
		IBaseRepository<T> Repository<T>() where T : class;
		Task<int> CommitAsync();
	}
}
