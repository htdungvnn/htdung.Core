using htdung.DataAccess.CacheRepository;
using htdung.DataAccess.EFRepository;
using htdung.DataAccess.Entities;
using htdung.DataAccess.MongoRepository;
using System.Linq.Expressions;

namespace htdung.DataAccess.Repositories
{
    public class GenericRepository<T>(
        IEfRepository<T> ef,
        ICacheRepository<T> cache,
        IMongoRepository<BaseActionLog<T>> mongo)
        : IGenericRepository<T>
        where T : BaseEntity
    {
        public async Task AddAsync(T entity)
        {
            await Task.WhenAll(
                ef.AddAsync(entity),
                cache.SetAsync(entity, TimeSpan.FromMinutes(5)),
                mongo.CreateAsync(new BaseActionLog<T>(entity, ActionType.Create, Guid.NewGuid())),
                cache.AddAllAsync(entity, TimeSpan.FromMinutes(5))
            );
        }

        public async Task AddListAsync(IEnumerable<T> entities)
        {
            var baseEntities = entities as T[] ?? entities.ToArray();
            await Task.WhenAll(
                ef.AddListAsync(baseEntities),
                cache.SetListAsync(baseEntities, TimeSpan.FromMinutes(5)),
                mongo.CreateListAsync(baseEntities.Select(e => new BaseActionLog<T>(e, ActionType.Create, Guid.NewGuid()))),
                cache.AddAllAsync(baseEntities, TimeSpan.FromMinutes(5))
            );
        }

        public async Task DeleteAsync(Guid id)
        {
            await Task.WhenAll(
                ef.DeleteAsync(id),
                cache.RemoveAsync(id),
                mongo.DeleteAsync(id),
                cache.RemoveAllAsync(id, TimeSpan.FromMinutes(5))
            );
        }

        public async Task DeleteListAsync(IEnumerable<Guid> ids)
        {
            var enumerable = ids as Guid[] ?? ids.ToArray();
            await Task.WhenAll(
                ef.DeleteListAsync(enumerable),
                cache.RemoveListAsync(enumerable),
                mongo.DeleteListAsync(enumerable),
                cache.RemoveAllAsync(enumerable, TimeSpan.FromMinutes(5))
            );
        }

        public async Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> filter)
        {
            if (!await cache.ExistAllAsync()) return await ef.Filter(filter);
            var all = await cache.GetListAsync(null, true);
            return all.AsQueryable().Where(filter);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            if (await cache.ExistAllAsync())
            {
                return await cache.GetListAsync(null, true);
            }
            return await ef.GetAllAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            if (!await cache.ExistAllAsync()) return await cache.GetAsync(id) ?? await ef.GetByIdAsync(id);
            var all = await cache.GetListAsync(null, true);
            return all.FirstOrDefault(e => e.Id == id);
        }

        public async Task<PaginatedResult<T>> GetPagedAsync(PaginatedFilter<T> paginatedFilter)
        {
            if (await cache.ExistAllAsync())
            {
                return await cache.GetAllPagedAsync(paginatedFilter);
            }
            return await ef.GetPagedAsync(paginatedFilter);
        }

        public async Task UpdateAsync(T entity)
        {
            await Task.WhenAll(
                ef.UpdateAsync(entity),
                cache.UpdateAsync(entity, TimeSpan.FromMinutes(5)),
                mongo.CreateAsync(new BaseActionLog<T>(entity, ActionType.Update, entity.Id)),
                cache.UpdateAllAsync(entity, TimeSpan.FromMinutes(5))
            );
        }

        public async Task UpdateListAsync(IEnumerable<T> entities)
        {
            var baseEntities = entities as T[] ?? entities.ToArray();
            await Task.WhenAll(
                ef.UpdateListAsync(baseEntities),
                cache.UpdateListAsync(baseEntities, TimeSpan.FromMinutes(5)),
                mongo.CreateListAsync(baseEntities.Select(e => new BaseActionLog<T>(e, ActionType.Update, e.Id))),
                cache.UpdateAllAsync(baseEntities, TimeSpan.FromMinutes(5))
            );
        }
    }
}