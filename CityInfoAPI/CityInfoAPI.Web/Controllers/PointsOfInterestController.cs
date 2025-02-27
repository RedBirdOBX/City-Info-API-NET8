using Asp.Versioning;
using AutoMapper;
using CityInfoAPI.Data;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    [Authorize]
    [ApiVersion(1.0)]
    [ApiVersion(2.0)]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly CityInfoMemoryDataStore _citiesDataStore;
        private readonly ICityInfoRepository _repo;
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;

        /// <summary>constructor</summary>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PointsOfInterestController(CityInfoMemoryDataStore citiesDataStore, ICityInfoRepository repo, ILogger<PointsOfInterestController> logger, IMailService mailService, IMapper mapper)
        {
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>Gets all Points of Interest</summary>
        /// <returns>collection of points of interest</returns>
        /// <example>{baseUrl}/api/pointsofinterest</example>
        [HttpGet("pointsofinterest", Name = "GetPointsOfInterest")]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest([FromQuery(Name = "name")] string? name = null,
            [FromQuery(Name = "search")] string? search = null)
        {
            try
            {
                var pointsOfInterest = await _repo.GetPointsOfInterestAsync(name, search);
                var results = _mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest);

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting points of interest.");
                return StatusCode(500, "An error occurred while getting points of interest.");
            }
        }

        /// <summary>Gets all Points of Interest for City</summary>
        /// <returns>collection of points of interest</returns>
        /// <param name="cityGuid"></param>
        /// <example>{baseUrl}/api/cities/{cityGuid}/pointsofinterest</example>
        [HttpGet("cities/{cityGuid}/pointsofinterest", Name = "GetPointsOfInterestForCity")]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterestForCity([FromRoute] Guid cityGuid)
        {
            try
            {
                var cityExists = await _repo.CityExistsAsync(cityGuid);
                if (!cityExists)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found when accessing points of interest.");
                    return NotFound();
                }

                var pointsOfInterest = await _repo.GetPointsOfInterestForCityAsync(cityGuid);
                var results = _mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest);

                return Ok(results);
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
        //[Authorize(Policy = "MustBeFromRichmond")] - Demo policy enforcement
        [HttpGet("cities/{cityGuid}/pointsofinterest/{pointGuid}", Name = "GetPointOfInterestById")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterestById([FromRoute] Guid cityGuid, [FromRoute] Guid pointGuid)
        {
            try
            {
                // demo purposes ONLY. checking and validating a user's claims (in token).
                // Claim Based Authorization
                //var claimsCity = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;
                //if (!await _repo.CityNameMatchesCityIdAsync(claimsCity, cityGuid))
                //{
                //    return Forbid();
                //}
                // end of demo

                var cityExists = await _repo.CityExistsAsync(cityGuid);
                if (!cityExists)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found when accessing points of interest.");
                    return NotFound();
                }

                var pointExists = await _repo.PointOfInterestExistsAsync(pointGuid);
                if (!pointExists)
                {
                    _logger.LogWarning($"Point of Interest with id {pointGuid} was not found.");
                    return NotFound();
                }

                var city = await _repo.GetCityByCityIdAsync(cityGuid, true);
                var pointOfInterest = city.PointsOfInterest.Where(p => p.PointGuid == pointGuid).FirstOrDefault();
                if (pointOfInterest == null)
                {
                    _logger.LogWarning($"Point of interest with id {pointGuid}  for City {cityGuid} wasn't found.");
                    return NotFound();
                }

                var result = _mapper.Map<PointOfInterestDto>(pointOfInterest);
                return Ok(result);
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
        [HttpPost("cities/{cityGuid}/pointsofinterest/", Name = "CreatePointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest([FromRoute] Guid cityGuid, [FromBody] PointOfInterestCreateDto request)
        {
            try
            {
                var cityExists = await _repo.CityExistsAsync(cityGuid);
                if (!cityExists)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found when creating point of interest.");
                    return NotFound();
                }

                // map the request
                var newPointRequest = _mapper.Map<PointOfInterest>(request);

                // create new call
                var newPointResults = await _repo.CreatePointOfInterestAsync(newPointRequest);

                // save it
                var success = await _repo.SaveChangesAsync();

                if (!success)
                {
                    _logger.LogError("An error occurred while point of interest.");
                    return StatusCode(500, "An error occurred while creating point of interest.");
                }

                // map the results
                var results = _mapper.Map<PointOfInterestDto>(newPointResults);

                // new guid coming back empty.  Why does city work but not this....
                return CreatedAtRoute("GetPointOfInterestById", new { cityGuid = cityGuid, pointGuid = results.PointGuid }, results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating point of interest.");
                return StatusCode(500, "An error occurred while creating point of interest.");
            }
        }

        /// <summary>updates point of interest through PUT</summary>
        /// <param name="cityGuid"></param>
        /// <param name="pointGuid"></param>
        /// <param name="updatePointOfInterest"></param>
        /// <returns></returns>
        /// <example>{baseUrl}/api/cities/{cityGuid}/pointsofinterest/{pointGuid}</example>
        [HttpPut("cities/{cityGuid}/pointsofinterest/{pointGuid}", Name = "UpdatePointOfInterest")]
        public async Task<ActionResult> UpdatePointOfInterest([FromRoute] Guid cityGuid, [FromRoute] Guid pointGuid, [FromBody] PointOfInterestUpdateDto updatePointOfInterest)
        {
            try
            {
                var cityExists = await _repo.CityExistsAsync(cityGuid);
                if (!cityExists)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found when creating point of interest.");
                    return NotFound();
                }

                // find the point of interest
                var pointExists = await _repo.PointOfInterestExistsAsync(pointGuid);
                if (!pointExists)
                {
                    _logger.LogWarning($"Point of interest with id {pointGuid} wasn't found.");
                    return NotFound();
                }

                // does the point of interest belong to the city?
                var existingPointOfInterest = await _repo.GetPointOfInterestById(pointGuid);
                if (existingPointOfInterest.CityGuid != cityGuid)
                {
                    _logger.LogWarning($"Point of interest with id {pointGuid} doesn't belong to city with id {cityGuid}.");
                    return NotFound();
                }

                // map the request - override the values of the destination object w/ source
                _mapper.Map(updatePointOfInterest, existingPointOfInterest);

                var success = await _repo.SaveChangesAsync();
                if (!success)
                {
                    _logger.LogError("An error occurred while updating point of interest.");
                    return StatusCode(500, "An error occurred while updating point of interest.");
                }
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
        [HttpPatch("cities/{cityGuid}/pointsofinterest/{pointGuid}", Name = "PatchPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> PatchPointOfInterest([FromRoute] Guid cityGuid, [FromRoute] Guid pointGuid, [FromBody] JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {
            try
            {
                var cityExists = await _repo.CityExistsAsync(cityGuid);
                if (!cityExists)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found when patching point of interest.");
                    return NotFound();
                }

                // find the point of interest
                var pointExists = await _repo.PointOfInterestExistsAsync(pointGuid);
                if (!pointExists)
                {
                    _logger.LogWarning($"Point of interest with id {pointGuid} wasn't found.");
                    return NotFound();
                }

                // does the point of interest belong to the city?
                var existingPointOfInterest = await _repo.GetPointOfInterestById(pointGuid);
                if (existingPointOfInterest.CityGuid != cityGuid)
                {
                    _logger.LogWarning($"Point of interest with id {pointGuid} doesn't belong to city with id {cityGuid}.");
                    return NotFound();
                }

                // map the request - override the values of the destination object w/ source
                var pointOfInterestToPatch = _mapper.Map<PointOfInterestUpdateDto>(existingPointOfInterest);

                // apply the patch - grab the updates and update the dto
                patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

                // validate the final version
                if (!TryValidateModel(pointOfInterestToPatch))
                {
                    _logger.LogWarning($"Invalid model state for the patch.");
                    return BadRequest(ModelState);
                }

                // map changes back to the entity
                _mapper.Map(pointOfInterestToPatch, existingPointOfInterest);

                var success = await _repo.SaveChangesAsync();
                if (!success)
                {
                    _logger.LogError("An error occurred while patching point of interest.");
                    return StatusCode(500, "An error occurred while patching point of interest.");
                }

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
        [HttpDelete("cities/{cityGuid}/pointsofinterest/{pointGuid}", Name = "DeletePointOfInterest")]
        public async Task<ActionResult> DeletePointOfInterest([FromRoute] Guid cityGuid, [FromRoute] Guid pointGuid)
        {
            try
            {
                // does the city exist?
                var cityExists = await _repo.CityExistsAsync(cityGuid);
                if (!cityExists)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found when patching point of interest.");
                    return NotFound();
                }

                // find the point of interest
                var pointExists = await _repo.PointOfInterestExistsAsync(pointGuid);
                if (!pointExists)
                {
                    _logger.LogWarning($"Point of interest with id {pointGuid} wasn't found.");
                    return NotFound();
                }

                // does the point of interest belong to the city?
                var existingPointOfInterest = await _repo.GetPointOfInterestById(pointGuid);
                if (existingPointOfInterest.CityGuid != cityGuid)
                {
                    _logger.LogWarning($"Point of interest with id {pointGuid} doesn't belong to city with id {cityGuid}.");
                    return NotFound();
                }

                // delete the point of interest
                await _repo.DeletePointOfInterestAsync(pointGuid);

                var success = await _repo.SaveChangesAsync();
                if (!success)
                {
                    _logger.LogError("An error occurred while deleting point of interest.");
                    return StatusCode(500, "An error occurred while deleting point of interest.");
                }

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
