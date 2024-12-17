using htdung.DataAccess.CacheRepository;
using htdung.DataAccess.EFRepository;
using htdung.DataAccess.Entities;
using htdung.DataAccess.MongoRepository;
using System.Linq.Expressions;

namespace htdung.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        IEFRepository<T> _ef;
        ICacheRepository<T> _cache;
        IMongoRepository<BaseActionLog<T>> _mongo;
        public GenericRepository(IEFRepository<T> ef, ICacheRepository<T> cache, IMongoRepository<BaseActionLog<T>> mongo)
        {
            _ef = ef;
            _cache = cache;
            _mongo = mongo;
        }
        public async Task AddAsync(T entity)
        {
            await _ef.AddAsync(entity);
            await _cache.SetAsync(entity, TimeSpan.FromMinutes(5));
            await _mongo.CreateAsync(new BaseActionLog<T>(entity, ActionType.Create, Guid.NewGuid()));
            await _cache.AddAllAsync(entity, TimeSpan.FromMinutes(5));
        }

        public async Task AddListAsync(IEnumerable<T> entities)
        {
            await _ef.AddListAsync(entities);
            await _cache.SetListAsync(entities, TimeSpan.FromMinutes(5));
            await _mongo.CreateListAsync(entities.Select(e => new BaseActionLog<T>(e, ActionType.Create, Guid.NewGuid())));
            await _cache.AddAllAsync(entities, TimeSpan.FromMinutes(5));
        }

        public async Task DeleteAsync(Guid id)
        {
            await _ef.DeleteAsync(id);
            await _cache.RemoveAsync(id);
            await _mongo.DeleteAsync(id);
            await _cache.RemoveAllAsync(id, TimeSpan.FromMinutes(5));
        }

        public async Task DeleteListAsync(IEnumerable<Guid> ids)
        {
            await _ef.DeleteListAsync(ids);
            await _cache.RemoveListAsync(ids);
            await _mongo.DeleteListAsync(ids);
            await _cache.RemoveAllAsync(ids, TimeSpan.FromMinutes(5));
        }


        public async Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> filter)
        {
            var existAll = await _cache.ExistAllAsync();
            if (existAll)
            {
                var all = await _cache.GetListAsync(null, true);
                return all.AsQueryable().Where(filter);
            }
            return await _ef.Filter(filter);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var existAll = await _cache.ExistAllAsync();
            if (existAll)
            {
                return await _cache.GetListAsync(null, true);
            }
            return await _ef.GetAllAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            var existAll = await _cache.ExistAllAsync();
            if (existAll)
            {
                var all = await _cache.GetListAsync(null, true);
                return all.FirstOrDefault(e => e.Id == id);
            }
            var entity = await _cache.GetAsync(id);
            if (entity != null)
            {
                return entity;
            }
            return await _ef.GetByIdAsync(id);
        }

        public async Task<PaginatedResult<T>> GetPagedAsync(PaginatedFilter<T> paginatedFilter)
        {
            var existAll = await _cache.ExistAllAsync();
            if (existAll)
            {
                return await _cache.GetAllPagedAsync(paginatedFilter);
            }
            return await _ef.GetPagedAsync(paginatedFilter);
        }

        public async Task UpdateAsync(T entity)
        {
            await _ef.UpdateAsync(entity);
            await _cache.UpdateAsync(entity, TimeSpan.FromMinutes(5));
            await _mongo.CreateAsync(new BaseActionLog<T>(entity, ActionType.Update, entity.Id));
            await _cache.UpdateAllAsync(entity, TimeSpan.FromMinutes(5));
        }

        public async Task UpdateListAsync(IEnumerable<T> entities)
        {
            await _ef.UpdateListAsync(entities);
            await _cache.UpdateListAsync(entities, TimeSpan.FromMinutes(5));
            await _mongo.CreateListAsync(entities.Select(e => new BaseActionLog<T>(e, ActionType.Update, e.Id)));
            await _cache.UpdateAllAsync(entities, TimeSpan.FromMinutes(5));
        }
    }
}
