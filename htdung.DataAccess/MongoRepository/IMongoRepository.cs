using htdung.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace htdung.DataAccess.MongoRepository
{
    public interface IMongoRepository<T> where T : class
    {
        Task CreateAsync(T action);
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task UpdateAsync(Guid id, T action);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter);
        Task CreateListAsync(IEnumerable<T> actions);
        Task UpdateListAsync(IEnumerable<T> actions);
        Task DeleteListAsync(IEnumerable<Guid> ids);


    }
}
