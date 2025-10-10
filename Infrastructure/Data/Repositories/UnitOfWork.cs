using SimpleSalesAPI.Domain.Repositories;
using SimpleSalesAPI.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Infrastructure.Data.Repositories
{
        public class UnitOfWork(AppDbContext context) : IUnitOfWork
        {
                private readonly Dictionary<Type, object> _repositories = [];
                private AppDbContext _context = context;

                public IBaseRepository<T> Repository<T>() where T : class
                {
                        if (_repositories.TryGetValue(typeof(T), out var repository))
                                return (IBaseRepository<T>)repository;

                        var newRepository = new BaseRepository<T>(_context);

                        _repositories[typeof(T)] = newRepository;

                        return newRepository;
                }

                public async Task<int> CommitAsync() => await _context.SaveChangesAsync();
                public void Dispose() => _context.Dispose();
                public async ValueTask DisposeAsync()
                {
                        if (_context != null)
                        {
                                await _context.DisposeAsync();
                                _context = null;
                        }
                }
        }
}
