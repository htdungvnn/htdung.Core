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
    public class MongoRepository<T> : IMongoRepository<T>
        where T: class
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(MongoDbContext context, string collectionName)
        {
            _collection = context.GetCollection<T>(collectionName);
        }

        public async Task CreateAsync(T action)
        {
            await _collection.InsertOneAsync(action);
        }

        public Task DeleteAsync(Guid id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            await _collection.DeleteOneAsync(filter);
        }

        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id); // Assuming the entity has "Id" field
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task< bool> Exist(Guid id)
        {
         return await _collection.Find(x => x.Id == id).AnyAsync();
        }

        public Task UpdateAsync(Guid id, T action)
        {
            throw new NotImplementedException();
        }
    }
}
