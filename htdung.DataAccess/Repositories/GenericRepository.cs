using htdung.DataAccess.CacheRepository;
using htdung.DataAccess.EFRepository;
using htdung.DataAccess.Entities;
using System.Linq.Expressions;

namespace htdung.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        IEFRepository<T> _ef;
        ICacheRepository<T> _cache;
        public GenericRepository(IEFRepository<T> ef, ICacheRepository<T> cache)
        {
            _ef = ef;
            _cache = cache;
        }
        public async Task AddAsync(T entity)
        {
            await _ef.AddAsync(entity);
            await _cache.SetAsync(entity, TimeSpan.FromMinutes(5));
        }

        public async Task AddListAsync(IEnumerable<T> entities)
        {
            await _ef.AddListAsync(entities);
            await _cache.SetListAsync(entities, TimeSpan.FromMinutes(5));
        }

        public async Task DeleteAsync(Guid id)
        {
            await _ef.DeleteAsync(id);
            await _cache.RemoveAsync(id);
        }

        public async Task DeleteListAsync(IEnumerable<Guid> ids)
        {
            await _ef.DeleteListAsync(ids);
            await _cache.RemoveListAsync(ids);
        }


        public async Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> filter)
        {
            return await _ef.Filter(filter);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _ef.GetAllAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            var entity = await _cache.GetAsync(id);
            if (entity != null)
            {
                return entity;
            }
            return await _ef.GetByIdAsync(id);
        }

        public async Task<PaginatedResult<T>> GetPagedAsync(PaginatedFilter<T> paginatedFilter)
        {
            return await _ef.GetPagedAsync(paginatedFilter);
        }

        public async Task UpdateAsync(T entity)
        {
            await _ef.UpdateAsync(entity);
            await _cache.UpdateAsync(entity, TimeSpan.FromMinutes(5));
        }

        public async Task UpdateListAsync(IEnumerable<T> entities)
        {
            await _ef.UpdateListAsync(entities);
            await _cache.UpdateListAsync(entities, TimeSpan.FromMinutes(5));
        }
    }
}
