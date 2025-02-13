using CityInfoAPI.Data;
using CityInfoAPI.Dtos.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        /// <summary>
        /// Gets all Points of Interest for City
        /// </summary>
        /// <returns></returns>
        [HttpGet("", Name = "GetPointsOfInterestForCity")]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterestForCity(int cityId)
        {
            var cities = CityInfoMemoryDataStore.Current.Cities;

            var city = cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }

        /// <summary>
        /// Gets single Point of Interest for City
        /// </summary>
        /// <returns></returns>
        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterestById")]
        public ActionResult<PointOfInterestDto> GetPointOfInterestById(int cityId, int pointOfInterestId)
        {
            var cities = CityInfoMemoryDataStore.Current.Cities;

            var city = cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }
    }
}
