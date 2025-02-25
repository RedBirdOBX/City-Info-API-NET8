namespace CityInfoAPI.Web.Controllers.ResponseHelpers
{
    public static class MetaDataHelper
    {
        // refactor; accept an obj, not primitive types
        public static PaginationMetaDataDto BuildCitiesMetaData(int citiesCount, int pageNumber, int pageSize)
        {
            try
            {
                int totalPages = (int)Math.Ceiling(citiesCount / (double)pageSize);
                bool hasNextPage = pageNumber < totalPages;
                bool hasPrevPage = pageNumber > 1;

                //string nextUrl = (requestParameters.PageNumber < totalPages) ? CreateCitiesResourceUri(requestParameters, ResourceUriType.NextPage, httpContextAccessor, linkGenerator) : string.Empty;
                //string prevUrl = (requestParameters.PageNumber > 1) ? CreateCitiesResourceUri(requestParameters, ResourceUriType.PreviousPage, httpContextAccessor, linkGenerator) : string.Empty;
                //string orderNameBy = (!string.IsNullOrEmpty(requestParameters.OrderNameBy)) ? requestParameters.OrderNameBy : "asc";
                //orderNameBy = orderNameBy.Equals("desc", StringComparison.CurrentCultureIgnoreCase) ? "desc" : "asc";

                PaginationMetaDataDto results = new PaginationMetaDataDto
                {
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    TotalCount = citiesCount,
                    HasNextPage = hasNextPage,
                    HasPreviousPage = hasPrevPage,
                    //NextPageUrl = nextUrl,
                    //PreviousPageUrl = prevUrl,
                    //OrderNameBy = orderNameBy
                };

                return results;
            }
            catch (ArgumentNullException exception)
            {
                throw exception;
            }
        }

        //private static string CreateCitiesResourceUri(RequestParameters requestParameters, ResourceUriType type, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        //{
        //    try
        //    {
        //        // i'm sensing a DRY violation here...
        //        switch (type)
        //        {
        //            case ResourceUriType.NextPage:
        //                return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext,
        //                                                        action: "GetPagedCities",
        //                                                        controller: "Cities",
        //                                                        values: new {
        //                                                                        pageNumber = requestParameters.PageNumber + 1,
        //                                                                        pageSize = requestParameters.PageSize ,
        //                                                                        nameFilter = requestParameters.NameFilter,
        //                                                        });

        //            case ResourceUriType.PreviousPage:
        //                return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext,
        //                                                        action: "GetPagedCities",
        //                                                        controller: "Cities",
        //                                                        values: new {
        //                                                                        pageNumber = requestParameters.PageNumber - 1,
        //                                                                        pageSize = requestParameters.PageSize,
        //                                                                        nameFilter = requestParameters.NameFilter,
        //                                                                    });
        //            default:
        //                return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext,
        //                                                        action: "GetPagedCities",
        //                                                        controller: "Cities",
        //                                                        values: new {
        //                                                                        pageNumber = requestParameters.PageNumber,
        //                                                                        pageSize = requestParameters.PageSize,
        //                                                                        nameFilter = requestParameters.NameFilter,
        //                                                                    });
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }
        //}
    }
}
