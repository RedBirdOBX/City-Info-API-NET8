using CityInfoAPI.Data;
using CityInfoAPI.Dtos.Models;
using Microsoft.AspNetCore.JsonPatch;
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
        [HttpGet("{cityGuid}", Name = "GetCityById")]
        public ActionResult<CityDto> GetCity([FromRoute] Guid cityGuid)
        {
            var city = CityInfoMemoryDataStore.Current.Cities.FirstOrDefault(c => c.CityGuid == cityGuid);

            if (city == null)
            {
                return NotFound();
            }

            return Ok(city);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="createCity"></param>
        /// <returns></returns>
        [HttpPost("", Name = "CreateCity")]
        public ActionResult<PointOfInterestDto> CreateCity([FromBody] CityCreateDto createCity)
        {
            // temp
            var cities = CityInfoMemoryDataStore.Current.Cities;
            int max = cities.Max(c => c.Id);

            var finalCity = new CityDto
            {
                Id = ++max,
                Name = createCity.Name,
                Description = createCity.Description,
                CreatedOn = createCity.CreatedOn.Date,
                CityGuid = createCity.CityGuid
            };

            cities.Add(finalCity);

            return CreatedAtRoute("GetCityById", new { cityGuid = finalCity.CityGuid }, finalCity );
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="updateCity"></param>
        /// <returns></returns>
        [HttpPut("{cityGuid}", Name = "UpdateCity")]
        public ActionResult<PointOfInterestDto> UpdateCity([FromRoute] Guid cityGuid, [FromBody] CityUpdateDto updateCity)
        {
            var existingCity = CityInfoMemoryDataStore.Current.Cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
            if (existingCity == null)
            {
                return NotFound();
            }

            existingCity.Name = updateCity.Name;
            existingCity.Description = updateCity.Description;

            return NoContent();
        }

        [HttpPatch("{cityGuid}", Name = "PatchCity")]
        public ActionResult<CityDto> PatchCity([FromRoute] Guid cityGuid, [FromBody] JsonPatchDocument<CityUpdateDto> patchDocument)
        {
            var existingCity = CityInfoMemoryDataStore.Current.Cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
            if (existingCity == null)
            {
                return NotFound();
            }

            var cityToPatch = new CityUpdateDto
            {
                Name = existingCity.Name,
                Description = existingCity.Description
            };

            patchDocument.ApplyTo(cityToPatch, ModelState);

            // validate the final version
            if (!TryValidateModel(cityToPatch))
            {
                return BadRequest(ModelState);
            }

            existingCity.Name = cityToPatch.Name;
            existingCity.Description = cityToPatch.Description;

            return NoContent();
        }
    }
}
