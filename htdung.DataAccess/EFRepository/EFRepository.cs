using htdung.DataAccess.Data;
using htdung.DataAccess.Entities;
using htdung.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace htdung.DataAccess.EFRepository
{
    public class EFRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public EFRepository(ApplicationDbContext dbCotext)
        {
            _context = dbCotext;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            var exist = await Exist(entity.Id);
            if (!exist)
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddListAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {

            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }

        }



        public async Task DeleteListAsync(IEnumerable<Guid> ids)
        {
            var entities = await _dbSet.Where(e => ids.Contains(e.Id)).ToListAsync();
            if (entities.Any())
            {
                _dbSet.RemoveRange(entities);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exist(Guid id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.Where(filter).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<PaginatedResult<T>> GetPagedAsync(PaginatedFilter<T> paginatedFilter)
        {
            // Start with the queryable
            var query = _context.GetDbSet<T>().AsQueryable();

            // Apply filter if provided
            if (paginatedFilter.Filter != null)
            {
                query = query.Where(paginatedFilter.Filter);
            }

            // Calculate the total count of records (filtered or unfiltered)
            var totalCount = await query.CountAsync();

            // Fetch the paginated data
            var data = await query
                .Skip((paginatedFilter.PageNumber - 1) * paginatedFilter.PageSize)  // Skip items for the current page
                .Take(paginatedFilter.PageSize)                     // Take only the number of items for the current page
                .ToListAsync();

            // Return the paginated result
            return new PaginatedResult<T>(data, paginatedFilter.PageNumber, paginatedFilter.PageSize, totalCount);
        }


        public async Task UpdateAsync(T model)
        {
            var entity = await _dbSet.FindAsync(model.Id);
            if (entity != null)
            {
                _dbSet.Update(model);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateListAsync(IEnumerable<T> models)
        {
            var entities = await _dbSet.Where(e => models.Select(m => m.Id).Contains(e.Id)).ToListAsync();
            if (entities.Any())
            {
                _dbSet.UpdateRange(models);
                await _context.SaveChangesAsync();
            }
        }
    }
}
