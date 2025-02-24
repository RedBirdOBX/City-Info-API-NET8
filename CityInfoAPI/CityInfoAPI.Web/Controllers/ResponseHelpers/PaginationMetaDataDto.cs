﻿namespace CityInfoAPI.Web.Controllers.ResponseHelpers
{
    public class PaginationMetaDataDto
    {
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

        public string PreviousPageUrl { get; set; } = string.Empty;

        public string NextPageUrl { get; set; } = string.Empty;

        public string OrderNameBy { get; set; } = string.Empty;
    }
}
