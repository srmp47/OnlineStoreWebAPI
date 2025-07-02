
namespace OnlineStoreWebAPI.Pagination
{
    public class PaginationParameters
    {
        private const int maxPageSize = 5;
        private int pageSize = 1;

        public int PageId { get; set; } = 1;

        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = Math.Min(maxPageSize, value);
            }
        }
    }
}
