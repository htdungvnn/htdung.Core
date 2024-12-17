using htdung.DataAccess.Entities;

namespace htdung.DataAccess.CacheRepository
{
    public interface ICacheRepository<T> where T : BaseEntity
    {
        Task SetAsync(T value, TimeSpan? expiry = null);
        Task SetListAsync(IEnumerable<T> values, TimeSpan? expiry = null, bool isAll = false);
        Task<T?> GetAsync(Guid key);
        Task<IEnumerable<T>> GetListAsync(IEnumerable<Guid>? key, bool isAll = false);
        Task<bool> ExistsAsync(Guid key);
        Task RemoveAsync(Guid key);
        Task RemoveListAsync(IEnumerable<Guid> key);
        Task UpdateAsync(T value, TimeSpan? expiry = null);
        Task UpdateListAsync(IEnumerable<T> values, TimeSpan? expiry = null);
        Task<bool> ExistAsync(Guid id);
        Task<bool> ExistAllAsync();
        Task AddAllAsync(T value, TimeSpan? expiry = null);
        Task AddAllAsync(IEnumerable<T> values, TimeSpan? expiry = null);
        Task RemoveAllAsync(Guid id, TimeSpan? expiry = null);
        Task RemoveAllAsync(IEnumerable<Guid> ids, TimeSpan? expiry = null);
        Task UpdateAllAsync(T entity, TimeSpan? expiry = null);
        Task UpdateAllAsync(IEnumerable<T> entities, TimeSpan? expiry = null);
        Task<PaginatedResult<T>> GetAllPagedAsync(PaginatedFilter<T> paginatedFilter);
    }
}
