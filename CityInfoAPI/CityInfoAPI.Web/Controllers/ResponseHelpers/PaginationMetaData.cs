namespace CityInfoAPI.Web.Controllers.ResponseHelpers
{
    /// <summary>
    /// obj used to provide metadata in response header
    /// </summary>
    public class PaginationMetaData
    {
        /// <summary>
        /// total items in the collection
        /// </summary>
        public int TotalItemCount { get; set; }

        /// <summary>
        /// total page count
        /// </summary>
        public int TotalPageCount { get; set; }

        /// <summary>
        /// page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// current page number
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="totalItemCount"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentPage"></param>
        public PaginationMetaData(int totalItemCount, int pageSize, int currentPage)
        {
            TotalItemCount = totalItemCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
        }
    }
}
