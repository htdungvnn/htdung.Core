using System.Linq.Expressions;

namespace htdung.DataAccess.Entities
{
    public class PaginatedResult<T> where T : BaseEntity
    {
        public PaginatedResult(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        }

        public IEnumerable<T> Data { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class PaginatedFilter<T> where T : BaseEntity
    {
        public PaginatedFilter(int pageNumber, int pageSize, Expression<Func<T, bool>> filter = null)
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
