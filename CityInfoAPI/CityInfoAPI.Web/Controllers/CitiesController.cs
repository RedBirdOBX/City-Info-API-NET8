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
        private readonly ILogger<CitiesController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CitiesController(ILogger<CitiesController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>Gets all Cities</summary>
        /// <returns>collection of CityDto</returns>
        /// <example>{baseUrl}/api/cities</example>
        [HttpGet("", Name = "GetCities")]
        public ActionResult<IEnumerable<CityDto>> GetCities()
        {
            try
            {
                _logger.LogInformation("Getting cities.");
                return Ok(CityInfoMemoryDataStore.Current.Cities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting cities.");
                return StatusCode(500, "An error occurred while getting cities.");
            }
        }

        /// <summary>Gets a City by Id</summary>
        /// <param name="cityGuid"></param>
        /// <returns>CityDto</returns>
        /// <example>{baseUrl}/api/cities/{cityGuid}</example>
         [HttpGet("{cityGuid}", Name = "GetCityById")]
        public ActionResult<CityDto> GetCity([FromRoute] Guid cityGuid)
        {
            try
            {
                var city = CityInfoMemoryDataStore.Current.Cities.FirstOrDefault(c => c.CityGuid == cityGuid);
                if (city == null)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found.");
                    return NotFound();
                }

                return Ok(city);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting city.");
                return StatusCode(500, "An error occurred while getting city.");
            }
        }

        /// <summary>creates a City</summary>
        /// <param name="createCity"></param>
        /// <returns>new city at details route</returns>
        /// <example>{baseUrl}/api/cities</example>
        [HttpPost("", Name = "CreateCity")]
        public ActionResult<PointOfInterestDto> CreateCity([FromBody] CityCreateDto createCity)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating city.");
                return StatusCode(500, "An error occurred while creating city.");
            }
        }

        /// <summary>updates city through PUT</summary>
        /// <param name="updateCity"></param>
        /// <returns>No Content</returns>
        /// <example>{baseUrl}/api/cities/{cityGuid}</example>
        [HttpPut("{cityGuid}", Name = "UpdateCity")]
        public ActionResult<PointOfInterestDto> UpdateCity([FromRoute] Guid cityGuid, [FromBody] CityUpdateDto updateCity)
        {
            try
            {
                var existingCity = CityInfoMemoryDataStore.Current.Cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
                if (existingCity == null)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found.");
                    return NotFound();
                }

                existingCity.Name = updateCity.Name;
                existingCity.Description = updateCity.Description;

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating city.");
                return StatusCode(500, "An error occurred while updating city.");
            }
        }

        /// <summary>patches city object</summary>
        /// <param name="cityGuid"></param>
        /// <param name="patchDocument"></param>
        /// <returns>No Content</returns>
        /// <example>{baseUrl}/api/cities/{cityGuid}</example>
        [HttpPatch("{cityGuid}", Name = "PatchCity")]
        public ActionResult<CityDto> PatchCity([FromRoute] Guid cityGuid, [FromBody] JsonPatchDocument<CityUpdateDto> patchDocument)
        {
            try
            {
                var existingCity = CityInfoMemoryDataStore.Current.Cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
                if (existingCity == null)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found.");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while patching city.");
                return StatusCode(500, "An error occurred while patching city.");
            }
        }

        /// <summary>deletes city object</summary>
        /// <param name="cityGuid"></param>
        /// <returns>no content</returns>
        /// <example>{baseUrl}/api/cities/{cityGuid}</example>
        [HttpDelete("{cityGuid}", Name = "DeleteCity")]
        public ActionResult<PointOfInterestDto> DeleteCity([FromRoute] Guid cityGuid)
        {
            try
            {
                var existingCity = CityInfoMemoryDataStore.Current.Cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
                if (existingCity == null)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found.");
                    return NotFound();
                }

                CityInfoMemoryDataStore.Current.Cities.Remove(existingCity);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting city.");
                return StatusCode(500, "An error occurred while deleting city.");
            }
        }
    }
}
