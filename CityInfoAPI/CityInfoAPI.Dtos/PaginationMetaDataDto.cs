namespace CityInfoAPI.Dtos
{
    /// <summary>
    /// Obj used for meta data in response headers
    /// </summary>
    public class PaginationMetaDataDto
    {
         /// <summary>
         /// current page counter, x of y.
         /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// count of total pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// number of records on page
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// total count of records
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// order by params
        /// </summary>
        public string OrderBy { get; set; } = string.Empty;

        /// <summary>
        /// bool - has prev page
        /// </summary>
        public bool HasPreviousPage { get; set; }

        /// <summary>
        /// bool - has next page
        /// </summary>
        public bool HasNextPage { get; set; }

        /// <summary>
        /// url for prev page
        /// </summary>
        public string PreviousPageUrl { get; set; } = string.Empty;

        /// <summary>
        /// url for next page
        /// </summary>
        public string NextPageUrl { get; set; } = string.Empty;
    }
}
