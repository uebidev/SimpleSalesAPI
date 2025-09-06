using SimpleSalesAPI.Infrastructure.Data.Context;
using SimpleSalesAPI.Infrastructure.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Infrastructure.Data.Repositories
{
	public class UnitOfWork(AppDbContext context) : IUnitOfWork
	{
		private readonly Dictionary<Type, object> _repositories = [];
		private AppDbContext _context = context;

		public IBaseRepository<T> Repository<T>() where T : class
		{
			if (_repositories.ContainsKey(typeof(T)))
				return (IBaseRepository<T>)_repositories[typeof(T)];

			var repository = new BaseRepository<T>(_context);

			_repositories[typeof(T)] = repository;

			return repository;
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
