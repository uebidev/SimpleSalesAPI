using System;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Domain.Repositories;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IBaseRepository<T> Repository<T>() where T : class;
    Task<int> CommitAsync();
}
