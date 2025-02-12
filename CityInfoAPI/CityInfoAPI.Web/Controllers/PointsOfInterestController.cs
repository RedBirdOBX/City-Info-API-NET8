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
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterestForCity()
        {
            return Ok(CityInfoMemoryDataStore.Current.Cities);
        }
    }
}
