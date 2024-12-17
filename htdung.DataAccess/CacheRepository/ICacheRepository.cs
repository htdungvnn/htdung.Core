using htdung.DataAccess.Entities;

namespace htdung.DataAccess.CacheRepository
{
    public interface ICacheRepository<T> where T : BaseEntity
    {
        Task SetAsync(T value, TimeSpan? expiry = null);
        Task SetListAsync(IEnumerable<T> values, TimeSpan? expiry = null);
        Task<T?> GetAsync(Guid key);
        Task<IEnumerable<T?>> GetListAsync(IEnumerable<Guid> key);
        Task<bool> ExistsAsync(Guid key);
        Task RemoveAsync(Guid key);
        Task RemoveListAsync(IEnumerable<Guid> key);
        Task UpdateAsync(T value, TimeSpan? expiry = null);
        Task UpdateListAsync(IEnumerable<T> values, TimeSpan? expiry = null);
        Task<bool> ExistAsync(Guid id);
    }
}
