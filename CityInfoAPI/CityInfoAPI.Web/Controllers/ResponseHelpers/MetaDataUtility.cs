using CityInfoAPI.Dtos;
using CityInfoAPI.Dtos.RequestModels;
using CityInfoAPI.Web.Controllers.ResponseHelpers;

namespace CityInfoAPI.Service;

#pragma warning disable CS1591
#pragma warning disable CS8603

public static class MetaDataUtility
{

    // TO DO: could this be a svc instead of a static class?
    public static PaginationMetaDataDto BuildCitiesMetaData(CityRequestParameters requestParams, int citiesCount, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
    {
        try
        {
            int totalPages = (int)Math.Ceiling(citiesCount / (double)requestParams.PageSize);
            bool hasNextPage = requestParams.PageNumber < totalPages;
            bool hasPrevPage = requestParams.PageNumber > 1;
            string nextUrl = (requestParams.PageNumber < totalPages) ? CreateCitiesResourceUri(requestParams, ResourceUriType.NextPage, httpContextAccessor, linkGenerator) : string.Empty;
            string prevUrl = (requestParams.PageNumber > 1) ? CreateCitiesResourceUri(requestParams, ResourceUriType.PreviousPage, httpContextAccessor, linkGenerator) : string.Empty;

            PaginationMetaDataDto results = new PaginationMetaDataDto
            {
                CurrentPage = requestParams.PageNumber,
                TotalPages = totalPages,
                PageSize = requestParams.PageSize,
                TotalCount = citiesCount,
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPrevPage,
                NextPageUrl = nextUrl,
                PreviousPageUrl = prevUrl,
            };

            return results;
        }
        catch (ArgumentNullException)
        {
            throw;
        }
    }

    public static string CreateCitiesResourceUri(CityRequestParameters requestParams, ResourceUriType type, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
    {
        try
        {
            switch (type)
            {
                case ResourceUriType.NextPage:
                    return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext,
                                                            action: "GetCities",
                                                            controller: "Cities",
                                                            values: new
                                                            {
                                                                pageNumber = requestParams.PageNumber + 1,
                                                                pageSize = requestParams.PageSize,
                                                                nameFilter = requestParams.Name,
                                                            });

                case ResourceUriType.PreviousPage:
                    return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext,
                                                            action: "GetCities",
                                                            controller: "Cities",
                                                            values: new
                                                            {
                                                                pageNumber = requestParams.PageNumber - 1,
                                                                pageSize = requestParams.PageSize,
                                                                nameFilter = requestParams.Name,
                                                            });
                default:
                    return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext,
                                                            action: "GetCities",
                                                            controller: "Cities",
                                                            values: new
                                                            {
                                                                pageNumber = requestParams.PageNumber,
                                                                pageSize = requestParams.PageSize,
                                                                nameFilter = requestParams.Name,
                                                            });
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
}

#pragma warning restore CS1591
#pragma warning restore CS8603