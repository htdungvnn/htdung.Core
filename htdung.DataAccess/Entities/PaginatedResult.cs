using System.Linq.Expressions;

namespace htdung.DataAccess.Entities
{
    public class PaginatedResult<T>(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
        where T : BaseEntity
    {
        public IEnumerable<T> Data { get; set; } = data;
        public int PageNumber { get; set; } = pageNumber;
        public int PageSize { get; set; } = pageSize;
        public int TotalCount { get; set; } = totalCount;
        public int TotalPages { get; set; } = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public class PaginatedFilter<T> where T : BaseEntity
    {
        public PaginatedFilter(int pageNumber, int pageSize, Expression<Func<T, bool>>? filter = null)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Filter = filter;
        }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Expression<Func<T, bool>>? Filter { get; set; }
    }
}
