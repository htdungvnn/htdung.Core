using htdung.DataAccess.Entities;
using StackExchange.Redis;
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

        public async Task SetListAsync(IEnumerable<T> values, TimeSpan? expiry = null)
        {
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

        public async Task<IEnumerable<T?>> GetListAsync(IEnumerable<Guid> keys)
        {
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
    }
}
