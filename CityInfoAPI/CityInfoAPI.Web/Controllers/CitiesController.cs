using CityInfoAPI.Data;
using CityInfoAPI.Dtos.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers
{
    [Route("api/cities")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        /// <summary>
        /// Gets all Cities
        /// </summary>
        /// <returns></returns>
        [HttpGet("", Name = "GetCities")]
        public ActionResult<IEnumerable<CityDto>> GetCities()
        {
            return Ok(CityInfoMemoryDataStore.Current.Cities);
        }

        /// <summary>
        /// Gets a City by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}", Name = "GetCity")]
        public ActionResult<CityDto> GetCity(int id)
        {
            var city = CityInfoMemoryDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);

            if (city == null)
            {
                return NotFound();
            }

            return Ok(city);
        }
    }
}
