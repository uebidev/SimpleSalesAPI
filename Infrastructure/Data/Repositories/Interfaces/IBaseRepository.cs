using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalesAPI.Infrastructure.Data.Repositories.Interfaces
{
	public interface IBaseRepository<T> where T : class
	{
		Task<T> GetByIdAsync(object id); //já que é genérico, vamos prever caso uma entidade tenha id de algum outro tipo (pode acontecer cara )
		Task<IEnumerable<T>> GetAllAsync();
		Task<IEnumerable<T>> GetFilterAsync(Expression<Func<T, bool>> predicate, bool tracking = false, params Expression<Func<T, object>>[] includes);
		Task AddAsync(T entity);
		void Update(T entity);
		void Remove(T entity);
	}
}
