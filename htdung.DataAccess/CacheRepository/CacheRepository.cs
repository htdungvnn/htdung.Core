using htdung.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Linq;
using System.Text.Json;

namespace htdung.DataAccess.CacheRepository
{
    public class CacheRepository<T> : ICacheRepository<T> where T : BaseEntity
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public CacheRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _database = _connectionMultiplexer.GetDatabase();
        }

        // Set a value in Redis cache with an optional expiration
        public async Task SetAsync(T value, TimeSpan? expiry = null)
        {
            var exist = await ExistAsync(value.Id);
            if (!exist)
            {
                var serializedValue = JsonSerializer.Serialize(value);
                await _database.StringSetAsync(value.Id.ToString(), serializedValue, expiry);
            }
        }

        public async Task SetListAsync(IEnumerable<T> values, TimeSpan? expiry = null, bool isAll = false)
        {
            if (isAll)
            {
                await SetAllAsync(values, expiry);
                return;
            }
            foreach (var value in values)
            {
                await SetAsync(value, expiry);
            }
        }

        // Get a value from Redis cache
        public async Task<T?> GetAsync(Guid key)
        {
            var exist = await ExistAsync(key);
            if (exist)
            {
                var value = await _database.StringGetAsync(key.ToString());
                return JsonSerializer.Deserialize<T>(value);
            }
            return null;
        }

        // Check if a key exists in the cache
        public async Task<bool> ExistsAsync(Guid key)
        {
            return await _database.KeyExistsAsync(key.ToString());
        }

        // Remove a key from Redis cache
        public async Task RemoveAsync(Guid key)
        {
            var exist = await ExistAsync(key);
            if (exist)
            {
                await _database.KeyDeleteAsync(key.ToString());
            }
        }

        public async Task<IEnumerable<T?>> GetListAsync(IEnumerable<Guid>? keys, bool isAll = false)
        {
            if (isAll)
            {
                return await GetAllAsync();
            }
            var result = new List<T?>();
            foreach (var k in keys)
            {
                var value = await GetAsync(k); // Fetch asynchronously

                result.Add(value); // Return result lazily

            }
            return result;
        }

        public async Task RemoveListAsync(IEnumerable<Guid> key)
        {
            foreach (var k in key)
            {
                await RemoveAsync(k);
            }
        }

        public async Task UpdateAsync(T value, TimeSpan? expiry = null)
        {
            await RemoveAsync(value.Id);
            await SetAsync(value, expiry);
        }

        public async Task UpdateListAsync(IEnumerable<T> values, TimeSpan? expiry = null)
        {
            await RemoveListAsync(values.Select(v => v.Id));
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
            var exist = await ExistAllAsync();
            if (!exist)
            {
                await _database.StringSetAsync("All" + typeof(T).Name, JsonSerializer.Serialize(values), expiry);
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var all = await _database.StringGetAsync("All" + typeof(T).Name);
            return JsonSerializer.Deserialize<IEnumerable<T>>(all);
        }

        public virtual async Task AddAllAsync(T value, TimeSpan? expiry = null)
        {
            var exist = await ExistAllAsync();
            if (exist)
            {
                var all = await GetAllAsync();
                all.Append(value);
                await _database.KeyDeleteAsync("All" + typeof(T).Name);
                await _database.StringSetAsync("All" + typeof(T).Name, JsonSerializer.Serialize(all), expiry);
            }
        }
        public async Task AddAllAsync(IEnumerable<T> values, TimeSpan? expiry = null)
        {
            var exist = await ExistAllAsync();
            if (exist)
            {
                var all = await GetAllAsync();
                all.Concat(values);
                await _database.KeyDeleteAsync("All" + typeof(T).Name);
                await _database.StringSetAsync("All" + typeof(T).Name, JsonSerializer.Serialize(all), expiry);
            }
        }
        public async Task UpdateAllAsync(T value, TimeSpan? expiry = null)
        {
            var exist = await ExistAllAsync();

            if (exist)
            {
                var all = await GetAllAsync();
                var itemToRemove = all.FirstOrDefault(v => v.Id == value.Id);
                all.ToList().Remove(itemToRemove);
                all.Append(value);
                await _database.KeyDeleteAsync("All" + typeof(T).Name);
                await _database.StringSetAsync("All" + typeof(T).Name, JsonSerializer.Serialize(all), expiry);
            }
        }

        public async Task RemoveAllAsync(Guid id, TimeSpan? expiry = null)
        {
            var exist = await ExistAllAsync();
            if (exist)
            {
                var all = await GetAllAsync();
                var itemToRemove = all.FirstOrDefault(v => v.Id == id);
                if (itemToRemove != null)
                {
                    all.ToList().Remove(itemToRemove);
                    await _database.KeyDeleteAsync("All" + typeof(T).Name);
                    await _database.StringSetAsync("All" + typeof(T).Name, JsonSerializer.Serialize(all));
                }
            }
        }

        public async Task RemoveAllAsync(IEnumerable<Guid> ids, TimeSpan? expiry = null)
        {
            var exist = await ExistAllAsync();
            if (exist)
            {
                var all = await GetAllAsync();
                var itemToUpdate = all.Where(v => !ids.Contains(v.Id));
                if (itemToUpdate != null)
                {
                    await _database.KeyDeleteAsync("All" + typeof(T).Name);
                    await _database.StringSetAsync("All" + typeof(T).Name, JsonSerializer.Serialize(itemToUpdate));
                }
            }
        }

        public async Task UpdateAllAsync(IEnumerable<T> entities, TimeSpan? expiry = null)
        {
            var exist = await ExistAllAsync();

            if (exist)
            {
                var all = await GetAllAsync();
                var allList = all.ToList();
                var itemsToRemove = allList.Where(v => entities.Select(x => x.Id).Contains(v.Id)).ToList();

                foreach (var item in itemsToRemove)
                {
                    allList.Remove(item);
                }

                allList.AddRange(entities);

                await _database.KeyDeleteAsync("All" + typeof(T).Name);
                await _database.StringSetAsync("All" + typeof(T).Name, JsonSerializer.Serialize(allList), expiry);
            }
        }

        public async Task<PaginatedResult<T>> GetAllPagedAsync(PaginatedFilter<T> paginatedFilter)
        {
            var exist = await ExistAllAsync();
            if (exist)
            {
                var all = await GetAllAsync();
                var dataFilter = new List<T>().AsQueryable();

                if (paginatedFilter.Filter != null)
                {
                    dataFilter = all.AsQueryable().Where(paginatedFilter.Filter);
                }

                var totalCount = await dataFilter.CountAsync();

                // Fetch the paginated data
                var data = await dataFilter
                    .Skip((paginatedFilter.PageNumber - 1) * paginatedFilter.PageSize)  // Skip items for the current page
                    .Take(paginatedFilter.PageSize)                     // Take only the number of items for the current page
                    .ToListAsync();

                // Return the paginated result
                return new PaginatedResult<T>(data, paginatedFilter.PageNumber, paginatedFilter.PageSize, totalCount);
            }
            return null;
        }


    }
}
