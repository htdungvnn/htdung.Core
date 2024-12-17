using htdung.DataAccess.Entities;
using System.Linq.Expressions;

namespace htdung.DataAccess.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task AddListAsync(IEnumerable<T> entities);
        Task UpdateListAsync(IEnumerable<T> entities);
        Task DeleteListAsync(IEnumerable<Guid> ids);
        Task<PaginatedResult<T>> GetPagedAsync(PaginatedFilter<T> pageFilter);
        Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> filter);

    }
}
