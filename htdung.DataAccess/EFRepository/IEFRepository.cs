using htdung.DataAccess.Entities;
using htdung.DataAccess.Repositories;

namespace htdung.DataAccess.EFRepository
{
    public interface IEFRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        Task<bool> Exist(Guid id);
    }
}
