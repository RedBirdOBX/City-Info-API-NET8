using CityInfoAPI.Data;
using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Web.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers
{
    [Route("api/cities/{cityGuid}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly CityInfoMemoryDataStore _citiesDataStore;
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;

        /// <summary>constructor</summary>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PointsOfInterestController(CityInfoMemoryDataStore citiesDataStore, ILogger<PointsOfInterestController> logger, IMailService mailService)
        {
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }

        /// <summary>Gets all Points of Interest for City</summary>
        /// <returns>collection of points of interest</returns>
        /// <param name="cityGuid"></param>
        /// <example>{baseUrl}/api/cities/{cityGuid}/pointsofinterest</example>
        [HttpGet("", Name = "GetPointsOfInterestForCity")]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterestForCity([FromRoute] Guid cityGuid)
        {
            try
            {
                var cities = _citiesDataStore.Cities;

                var city = cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
                if (city == null)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found when accessing points of interest.");
                    return NotFound();
                }

                return Ok(city.PointsOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting points of interest.");
                return StatusCode(500, "An error occurred while getting points of interest.");
            }
        }

        /// <summary>gets point of interest by id</summary>
        /// <param name="cityGuid"></param>
        /// <param name="pointGuid"></param>
        /// <returns>point of interest</returns>
        /// <example>{baseUrl}/api/cities/{cityGuid}/pointsofinterest/{pointGuid}</example>
        [HttpGet("{pointGuid}", Name = "GetPointOfInterestById")]
        public ActionResult<PointOfInterestDto> GetPointOfInterestById([FromRoute] Guid cityGuid, [FromRoute] Guid pointGuid)
        {
            try
            {
                var cities = _citiesDataStore.Cities;

                var city = cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
                if (city == null)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found when accessing points of interest.");
                    return NotFound();
                }

                var pointOfInterest = city.PointsOfInterest.Where(p => p.PointGuid == pointGuid).FirstOrDefault();
                if (pointOfInterest == null)
                {
                    _logger.LogWarning($"Point of interest with id {pointGuid} wasn't found.");
                    return NotFound();
                }

                return Ok(pointOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting point of interest.");
                return StatusCode(500, "An error occurred while getting point of interest.");
            }
        }

        /// <summary>creates a point of interest</summary>
        /// <param name="cityGuid"></param>
        /// <param name="createPointOfInterest"></param>
        /// <returns>new point of interest at route</returns>
        /// <example>{baseUrl}/api/cities/{cityGuid}/pointsofinterest</example>
        [HttpPost("", Name = "CreatePointOfInterest")]
        public ActionResult CreatePointOfInterest([FromRoute] Guid cityGuid, [FromBody] PointOfInterestCreateDto createPointOfInterest)
        {
            try
            {
                var city = _citiesDataStore.Cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
                if (city == null)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found when creating point of interest.");
                    return NotFound();
                }

                // temp
                int max = _citiesDataStore.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id) + 1;

                var finalPointOfInterest = new PointOfInterestDto
                {
                    Id = max,
                    PointGuid = createPointOfInterest.PointGuid,
                    CityId = createPointOfInterest.CityId,
                    CityGuid = cityGuid,
                    Name = createPointOfInterest.Name,
                    Description = createPointOfInterest.Description,
                    CreatedOn = createPointOfInterest.CreatedOn.Date
                };

                city.PointsOfInterest.Add(finalPointOfInterest);

                return CreatedAtRoute("GetPointOfInterestById", new { cityGuid = cityGuid, pointOfInterestGuid = finalPointOfInterest.PointGuid }, finalPointOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating point of interest.");
                return StatusCode(500, "An error occurred while creating point of interest.");
            }
        }

        /// <summary>updates point of interest through PUT</summary>
        /// <param name="cityGuid"></param>
        /// <param name="pointOfInterestGuid"></param>
        /// <param name="updatePointOfInterest"></param>
        /// <returns></returns>
        /// <example>{baseUrl}/api/cities/{cityGuid}/pointsofinterest/{pointGuid}</example>
        [HttpPut("{pointGuid}", Name = "UpdatePointOfInterest")]
        public ActionResult<PointOfInterestDto> UpdatePointOfInterest([FromRoute] Guid cityGuid, [FromRoute] Guid pointGuid, [FromBody] PointOfInterestUpdateDto updatePointOfInterest)
        {
            try
            {
                var city = _citiesDataStore.Cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
                if (city == null)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found when updating point of interest.");
                    return NotFound();
                }

                // find the point of interest
                var existingPointOfInterest = city.PointsOfInterest.Where(p => p.PointGuid == pointGuid).FirstOrDefault();
                if (existingPointOfInterest == null)
                {
                    _logger.LogWarning($"Point of interest with id {pointGuid} wasn't found.");
                    return NotFound();
                }

                // does the point of interest belong to the city?
                if (existingPointOfInterest.CityGuid != cityGuid)
                {
                    _logger.LogWarning($"Point of interest with id {pointGuid} doesn't belong to city with id {cityGuid}.");
                    return NotFound();
                }

                existingPointOfInterest.Name = updatePointOfInterest.Name;
                existingPointOfInterest.Description = updatePointOfInterest.Description;

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating point of interest.");
                return StatusCode(500, "An error occurred while updating point of interest.");
            }
        }

        /// <summary>patches point of interest</summary>
        /// <param name="cityGuid"></param>
        /// <param name="pointGuid"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        /// <example>{baseUrl}/api/cities/{cityGuid}/pointsofinterest/{pointGuid}</example>
        [HttpPatch("{pointGuid}", Name = "PatchPointOfInterest")]
        public ActionResult<PointOfInterestDto> PatchPointOfInterest([FromRoute] Guid cityGuid, [FromRoute] Guid pointGuid, [FromBody] JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {
            try
            {
                var city = _citiesDataStore.Cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
                if (city == null)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found when patching point of interest.");
                    return NotFound();
                }

                // find the point of interest
                var existingPointOfInterest = city.PointsOfInterest.Where(p => p.PointGuid == pointGuid).FirstOrDefault();
                if (existingPointOfInterest == null)
                {
                    _logger.LogWarning($"Point of interest with id {pointGuid} wasn't found.");
                    return NotFound();
                }

                // does the point of interest belong to the city?
                if (existingPointOfInterest.CityGuid != cityGuid)
                {
                    _logger.LogWarning($"Point of interest with id {pointGuid} doesn't belong to city with id {cityGuid}.");
                    return NotFound();
                }

                var pointOfInterestToPatch = new PointOfInterestUpdateDto
                {
                    Name = existingPointOfInterest.Name,
                    Description = existingPointOfInterest.Description
                };

                patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

                // validate the final version
                if (!TryValidateModel(pointOfInterestToPatch))
                {
                    _logger.LogWarning($"Invalid model state for the patch.");
                    return BadRequest(ModelState);
                }

                existingPointOfInterest.Name = pointOfInterestToPatch.Name;
                existingPointOfInterest.Description = pointOfInterestToPatch.Description;

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while patching point of interest.");
                return StatusCode(500, "An error occurred while patching point of interest.");
            }
        }

        /// <summary>deletes point of interest</summary>
        /// <param name="cityGuid"></param>
        /// <param name="pointGuid"></param>
        /// <returns></returns>
        /// <example>{baseUrl}/api/cities/{cityGuid}/pointsofinterest/{pointGuid}</example>
        [HttpDelete("{pointGuid}", Name = "DeletePointOfInterest")]
        public ActionResult DeletePointOfInterest([FromRoute] Guid cityGuid, [FromRoute] Guid pointGuid)
        {
            try
            {
                var existingCity = _citiesDataStore.Cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
                if (existingCity == null)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found when deleting point of interest.");
                    return NotFound();
                }

                // find the point of interest
                var existingPointOfInterest = existingCity.PointsOfInterest.Where(p => p.PointGuid == pointGuid).FirstOrDefault();
                if (existingPointOfInterest == null)
                {
                    _logger.LogWarning($"Point of interest with id {pointGuid} wasn't found.");
                    return NotFound();
                }

                // does the point of interest belong to the city?
                if (existingPointOfInterest.CityGuid != cityGuid)
                {
                    _logger.LogWarning($"Point of interest with id {pointGuid} doesn't belong to city with id {cityGuid}.");
                    return NotFound();
                }

                existingCity.PointsOfInterest.Remove(existingPointOfInterest);

                // test / demo svc
                _mailService.Send("Point of interest deleted.", $"Point of interest {existingPointOfInterest.Name} with id {existingPointOfInterest.PointGuid} was deleted.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting point of interest.");
                return StatusCode(500, "An error occurred while deleting point of interest.");
            }
        }
    }
}
