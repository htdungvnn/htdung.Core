using htdung.DataAccess.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace htdung.DataAccess.CacheRepository
{
    public class CacheRepository<T>(IConnectionMultiplexer connectionMultiplexer) : ICacheRepository<T>
        where T : BaseEntity
    {
        private readonly IDatabase _database = connectionMultiplexer.GetDatabase();

        public async Task SetAsync(T value, TimeSpan? expiry = null)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(value.Id.ToString(), serializedValue, expiry);
        }

        public async Task SetListAsync(IEnumerable<T> values, TimeSpan? expiry = null, bool isAll = false)
        {
            if (isAll)
            {
                await SetAllAsync(values, expiry);
                return;
            }

            var batch = _database.CreateBatch();
            foreach (var value in values)
            {
                var serializedValue = JsonSerializer.Serialize(value);
                await batch.StringSetAsync(value.Id.ToString(), serializedValue, expiry);
            }
            batch.Execute();
        }

        public async Task<T?> GetAsync(Guid key)
        {
            var value = await _database.StringGetAsync(key.ToString());
            return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : null;
        }

        public async Task<bool> ExistsAsync(Guid key)
        {
            return await _database.KeyExistsAsync(key.ToString());
        }

        public async Task RemoveAsync(Guid key)
        {
            await _database.KeyDeleteAsync(key.ToString());
        }

        public async Task<IEnumerable<T>> GetListAsync(IEnumerable<Guid>? keys, bool isAll = false)
        {   
            if (isAll)
            {
                return await GetAllAsync();
            }

            if (keys != null)
            {
                var tasks = keys.Select(GetAsync);
                return (await Task.WhenAll(tasks))!;
            }

            return null;
        }

        public Task RemoveListAsync(IEnumerable<Guid> keys)
        {
            var batch = _database.CreateBatch();
            foreach (var key in keys)
            {
                batch.KeyDeleteAsync(key.ToString());
            }
            batch.Execute();
            return Task.CompletedTask;
        }

        public async Task UpdateAsync(T value, TimeSpan? expiry = null)
        {
            await SetAsync(value, expiry);
        }

        public async Task UpdateListAsync(IEnumerable<T> values, TimeSpan? expiry = null)
        {
            await SetListAsync(values, expiry);
        }

        public async Task<bool> ExistAsync(Guid id)
        {
            return await _database.KeyExistsAsync(id.ToString());
        }

        public async Task<bool> ExistAllAsync()
        {
            return await _database.KeyExistsAsync("All" + typeof(T).Name);
        }

        public async Task SetAllAsync(IEnumerable<T> values, TimeSpan? expiry = null)
        {
            var serializedValues = JsonSerializer.Serialize(values);
            await _database.StringSetAsync("All" + typeof(T).Name, serializedValues, expiry);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var all = await _database.StringGetAsync("All" + typeof(T).Name);
            return all.HasValue ? JsonSerializer.Deserialize<IEnumerable<T>>(all) : Enumerable.Empty<T>();
        }

        public async Task AddAllAsync(T value, TimeSpan? expiry = null)
        {
            var all = (await GetAllAsync()).ToList();
            all.Add(value);
            await SetAllAsync(all, expiry);
        }

        public async Task AddAllAsync(IEnumerable<T> values, TimeSpan? expiry = null)
        {
            var all = (await GetAllAsync()).ToList();
            all.AddRange(values);
            await SetAllAsync(all, expiry);
        }

        public async Task UpdateAllAsync(T value, TimeSpan? expiry = null)
        {
            var all = (await GetAllAsync()).ToList();
            var index = all.FindIndex(v => v.Id == value.Id);
            if (index >= 0)
            {
                all[index] = value;
                await SetAllAsync(all, expiry);
            }
        }

        public async Task RemoveAllAsync(Guid id, TimeSpan? expiry = null)
        {
            var all = (await GetAllAsync()).ToList();
            all.RemoveAll(v => v.Id == id);
            await SetAllAsync(all, expiry);
        }

        public async Task RemoveAllAsync(IEnumerable<Guid> ids, TimeSpan? expiry = null)
        {
            var all = (await GetAllAsync()).ToList();
            all.RemoveAll(v => ids.Contains(v.Id));
            await SetAllAsync(all, expiry);
        }

        public async Task UpdateAllAsync(IEnumerable<T> entities, TimeSpan? expiry = null)
        {
            var all = (await GetAllAsync()).ToList();
            var baseEntities = entities as T[] ?? entities.ToArray();
            var ids = baseEntities.Select(e => e.Id).ToHashSet();
            all.RemoveAll(v => ids.Contains(v.Id));
            all.AddRange(baseEntities);
            await SetAllAsync(all, expiry);
        }

        public async Task<PaginatedResult<T>> GetAllPagedAsync(PaginatedFilter<T> paginatedFilter)
        {
            var all = (await GetAllAsync()).AsQueryable();

            if (paginatedFilter.Filter != null)
            {
                all = all.Where(paginatedFilter.Filter);
            }

            var totalCount = all.Count();
            var data = all.Skip((paginatedFilter.PageNumber - 1) * paginatedFilter.PageSize)
                          .Take(paginatedFilter.PageSize)
                          .ToList();

            return new PaginatedResult<T>(data, paginatedFilter.PageNumber, paginatedFilter.PageSize, totalCount);
        }
    }
}