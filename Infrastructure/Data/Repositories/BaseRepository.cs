using Microsoft.EntityFrameworkCore;
using SimpleSalesAPI.Domain.Repositories;
using SimpleSalesAPI.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SimpleSalesAPI.Infrastructure.Data.Repositories
{
        public class BaseRepository<T> : IBaseRepository<T> where T : class
        {
                protected readonly AppDbContext _context;
                protected readonly DbSet<T> _dbSet;
                public BaseRepository(AppDbContext context)
                {
                        _context = context;
                        _dbSet = _context.Set<T>();
                }
                public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

                public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

                public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);


                public async Task<IEnumerable<T>> GetFilterAsync(Expression<Func<T, bool>> predicate, bool tracking = false, params Expression<Func<T, object>>[] includes)
                {
                        IQueryable<T> query = _dbSet;

                        if (!tracking)
                                query = query.AsNoTracking();

                        if (includes != null)
                                foreach (var include in includes)
                                        query = query.Include(include);
                        if (predicate != null)
                                query = query.Where(predicate);

                        return await query.ToListAsync();
                }

                public void Remove(T entity) => _dbSet.Remove(entity);

                public void Update(T entity) => _dbSet.Update(entity);

                public async Task<List<TResult>> QueryAsync<TResult>(Func<IQueryable<T>, IQueryable<TResult>> query)
                {
                        if (query is null)
                                throw new ArgumentNullException(nameof(query));

                        return await query(_dbSet).ToListAsync();
                }
        }
}
