using CityInfoAPI.Dtos.Models;

namespace CityInfoAPI.Web.Controllers.ResponseHelpers
{
    #pragma warning disable CS1591

    // TO DO: Should this be a service?

    /// <summary>
    /// Used for building links in GET responses.
    /// </summary>
    public static class UriLinkHelper
    {
        public static LinkDto CreateLinkForCityWithinCollection(HttpRequest request, CityWithoutPointsOfInterestDto city)
        {
            string protocol = (request.IsHttps) ? "https" : "http";
            string version = "v1.0";    // we should probably look this up
            try
            {
                LinkDto link = new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{city.CityGuid}", "city", "GET");
                return link;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static CityDto CreateLinksForCityWithPointsOfInterest(HttpRequest request, CityDto city, int pageSize)
        {
            // NOTE: If we ever exceed 100 cities, we would need to calculate which page to send the user to in the collection.
            string protocol = (request.IsHttps) ? "https" : "http";
            string version = "v1.0";    // we should probably look this up
            try
            {
                // link to self
                city.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{city.CityGuid}", "self", "GET"));

                // link to collection
                city.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities?pageNumber=1&pageSize={pageSize}", "city-collection", "GET"));

                // if you wanted to expose this....then you could do this:
                // you could also wrap this in some custom logic...perhaps only show if authenticated...
                // link to create
                city.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities", "city-create", "POST"));

                // link to patch
                city.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{city.CityGuid}", "city-patch", "PATCH"));

            }
            catch (Exception)
            {
                throw;
            }
            return city;
        }

        public static CityWithoutPointsOfInterestDto CreateLinksForCity(HttpRequest request, CityWithoutPointsOfInterestDto city, int pageSize)
        {
            // NOTE: If we ever exceed 100 cities, we would need to calculate which page to send the user to in the collection.
            string protocol = (request.IsHttps) ? "https" : "http";
            string version = "v1.0";    // we should probably look this up
            try
            {
                // link to self
                city.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{city.CityGuid}", "self", "GET"));

                // link to collection
                city.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities?pageNumber=1&pageSize={pageSize}", "city-collection", "GET"));

                // if you wanted to expose this....then you could do this:
                // you could also wrap this in some custom logic...perhaps only show if authenticated...
                // link to create
                city.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities", "city-create", "POST"));

                // link to patch
                city.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{city.CityGuid}", "city-patch", "PATCH"));

            }
            catch (Exception)
            {
                throw;
            }
            return city;
        }

        public static LinkDto CreateLinkForPointOfInterestWithinCollection(HttpRequest request, PointOfInterestDto poi)
        {
            string protocol = (request.IsHttps) ? "https" : "http";
            string version = "v2.0";    // we should probably look this up
            try
            {
                LinkDto link = new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{poi.CityGuid}/pointsofinterest/{poi.PointGuid}", "point-of-interest", "GET");
                return link;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static PointOfInterestDto CreateLinksForPointOfInterest(HttpRequest request, PointOfInterestDto poi)
        {
            string protocol = (request.IsHttps) ? "https" : "http";
            string version = "v2.0";    // we should probably look this up
            //var defaultRequestParamaters = new RequestParameters();
            try
            {
                // link to self
                poi.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{poi.CityGuid}/pointsofinterest/{poi.PointGuid}", "self", "GET"));

                // link to collection
                poi.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{poi.CityGuid}/pointsofinterest", "points-of-interest-collection", "GET"));

                // if you wanted to expose this....then you could do this:
                // you could also wrap this in some custom logic...perhaps only show if authenticated...
                // link to create
                poi.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{poi.CityGuid}/pointsofinterest", "point-of-interest-create", "POST"));

                // link to update
                poi.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{poi.CityGuid}/pointsofinterest/{poi.PointGuid}", "point-of-interest-update", "PUT"));

                // link to patch
                poi.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{poi.CityGuid}/pointsofinterest/{poi.PointGuid}", "point-of-interest-patch", "PATCH"));

                // link to delete
                poi.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{poi.CityGuid}/pointsofinterest/{poi.PointGuid}", "point-of-interest-delete", "DELETE"));
            }
            catch (Exception)
            {
                throw;
            }
            return poi;
        }
    }

    #pragma warning restore CS1591
}
