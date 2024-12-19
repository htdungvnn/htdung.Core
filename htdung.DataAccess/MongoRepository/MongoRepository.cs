using htdung.DataAccess.Data;
using htdung.DataAccess.Entities;
using MongoDB.Driver;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace htdung.DataAccess.MongoRepository
{
    public class MongoRepository<T>(MongoDbContext context, string collectionName) : IMongoRepository<T>
        where T : BaseActionLog<BaseEntity>
    {
        private readonly IMongoCollection<T> _collection = context.GetCollection<T>(collectionName);

        public async Task CreateAsync(T action)
        {
            await _collection.InsertOneAsync(action);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _collection.DeleteOneAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
        {
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> Exist(Guid id)
        {
            return await _collection.Find(x => x.Id == id).AnyAsync();
        }

        public async Task UpdateAsync(Guid id, T action)
        {
            await _collection.ReplaceOneAsync(e => e.Id == id, action);
        }

        public async Task CreateListAsync(IEnumerable<T> action)
        {
            await _collection.InsertManyAsync(action);
        }

        public async Task UpdateListAsync(IEnumerable<T> actions)
        {
            var baseActionLogs = actions as T[] ?? actions.ToArray();
            await _collection.UpdateManyAsync(Builders<T>.Filter.In(e => e.Id, baseActionLogs.Select(a => a.Id)), Builders<T>.Update.Set(e => e, baseActionLogs.First()));
        }

        public Task DeleteListAsync(IEnumerable<Guid> ids)
        {
            var filter = Builders<T>.Filter.In(e => e.Id, ids);
            return _collection.DeleteManyAsync(filter);
        }
    }
}
