using htdung.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace htdung.DataAccess.MongoRepository
{
    public interface IMongoRepository<T> where T : class
    {
        // Insert a new entity
        Task CreateAsync(T action);

        // Get an entity by its ID
        Task<T> GetByIdAsync(Guid id);

        // Get all entities
        Task<IEnumerable<T>> GetAllAsync();

        // Update an entity
        Task UpdateAsync(Guid id, T action);

        // Delete an entity by its ID
        Task DeleteAsync(Guid id);

        // Find entities using a filter expression
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter);

    }
}
