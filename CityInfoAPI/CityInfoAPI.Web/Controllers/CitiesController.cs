using Asp.Versioning;
using AutoMapper;
using CityInfoAPI.Data;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Web.Controllers.ResponseHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers
{
    [Route("api/v{version:apiVersion}/cities")]
    [ApiController]
    [Authorize]
    [ApiVersion(1.0)]
    [ApiVersion(2.0)]
    public class CitiesController : ControllerBase
    {
        private readonly ILogger<CitiesController> _logger;
        private readonly CityInfoMemoryDataStore _citiesDataStore;
        private readonly ICityInfoRepository _repo;
        private readonly IMapper _mapper;
        private readonly int _maxPageSize = 100;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CitiesController(ILogger<CitiesController> logger, CityInfoMemoryDataStore citiesDataStore, ICityInfoRepository repo, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(_citiesDataStore));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>Gets all Cities</summary>
        /// <returns>collection of CityDto</returns>
        /// <param name="includePointsOfInterest"></param>
        /// <param name="name"></param>
        /// <param name="search"></param>
        /// <example>{baseUrl}/api/cities</example>
        [HttpGet("", Name = "GetCities")]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities([FromQuery] bool? includePointsOfInterest = true,
            [FromQuery(Name = "name")] string? name = null, [FromQuery(Name = "search")] string? search = null,
            [FromQuery(Name = "pageNumber")] int pageNumber = 1, [FromQuery(Name = "pageSize")] int pageSize = 100)
        {
            try
            {
                if (pageSize > _maxPageSize)
                {
                    pageSize = _maxPageSize;
                }

                var cities = await _repo.GetCitiesAsync(name, search, pageNumber, pageSize);
                var results = _mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cities);

                _logger.LogInformation("Getting cities.");

                var totalCities = await _repo.GetCitiesCountAsync();
                var metaData = MetaDataHelper.BuildCitiesMetaData(totalCities, pageNumber, pageSize);

                // add as custom header
                Response.Headers.Append("X-CityParameters", Newtonsoft.Json.JsonConvert.SerializeObject(metaData));

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting cities.");
                return StatusCode(500, "An error occurred while getting cities.");
            }
        }

        /// <summary>Gets a City by Id</summary>
        /// <param name="cityGuid"></param>
        /// <param name="includePointsOfInterest"></param>
        /// <returns>CityDto</returns>
        /// <example>{baseUrl}/api/cities/{cityGuid}?includePointsOfInterest={bool}</example>
        [HttpGet("{cityGuid}", Name = "GetCityByCityId")]
        public async Task<IActionResult> GetCityByCityId([FromRoute] Guid cityGuid, [FromQuery] bool includePointsOfInterest = true)
        {
            try
            {
                var cityExists = await _repo.CityExistsAsync(cityGuid);
                if (!cityExists)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found.");
                    return NotFound();
                }

                var city = await _repo.GetCityByCityIdAsync(cityGuid, includePointsOfInterest);
                if (includePointsOfInterest)
                {
                    var results = _mapper.Map<CityDto>(city);
                    return Ok(results);
                }
                else
                {
                    var results = _mapper.Map<CityWithoutPointsOfInterestDto>(city);
                    return Ok(results);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting city.");
                return StatusCode(500, $"An error occurred while getting city. {ex}");
            }
        }

        /// <summary>creates a City</summary>
        /// <param name="request"></param>
        /// <returns>CityDto at details route</returns>
        /// <example>{baseUrl}/api/cities</example>
        [HttpPost("", Name = "CreateCity")]
        public async Task<ActionResult<CityWithoutPointsOfInterestDto>> CreateCity([FromBody] CityCreateDto request)
        {
            try
            {
                // guids are auto-generated and not provided by client. unlikely but just in case.
                var cityExists = await _repo.CityExistsAsync(request.CityGuid);
                if (cityExists)
                {
                    return Conflict($"City {request.CityGuid} already exists.");
                }

                // map the request
                var newCityRequest = _mapper.Map<City>(request);

                // create new call
                var newCity = await _repo.CreateCityAsync(newCityRequest);

                // save changes
                var success = await _repo.SaveChangesAsync();

                if (!success)
                {
                    _logger.LogError("An error occurred while creating city.");
                    return StatusCode(500, "An error occurred while creating city.");
                }

                // build proper results
                var results = _mapper.Map<CityWithoutPointsOfInterestDto>(newCity);

                return CreatedAtRoute("GetCityByCityId", new { cityGuid = results.CityGuid }, results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating city.");
                return StatusCode(500, "An error occurred while creating city.");
            }
        }

        /// <summary>updates city through PUT</summary>
        /// <param name="request"></param>
        /// <returns>No Content</returns>
        /// <example>{baseUrl}/api/cities/{cityGuid}</example>
        [HttpPut("{cityGuid}", Name = "UpdateCity")]
        public async Task<ActionResult> UpdateCity([FromRoute] Guid cityGuid, [FromBody] CityUpdateDto request)
        {
            try
            {
                var cityExists = await _repo.CityExistsAsync(cityGuid);
                if (!cityExists)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found.");
                    return NotFound();
                }

                var city = await _repo.GetCityByCityIdAsync(cityGuid, false);

                // map the request - override the values of the destination object w/ source
                _mapper.Map(request, city);

                var success = await _repo.SaveChangesAsync();
                if (!success)
                {
                    _logger.LogError("An error occurred while updating city.");
                    return StatusCode(500, "An error occurred while updating city.");
                }

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
        public async Task<ActionResult<CityDto>> PatchCity([FromRoute] Guid cityGuid, [FromBody] JsonPatchDocument<CityUpdateDto> patchDocument)
        {
            try
            {
                var cityExists = await _repo.CityExistsAsync(cityGuid);
                if (!cityExists)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found when patching city.");
                    return NotFound();
                }

                var existingCity = await _repo.GetCityByCityIdAsync(cityGuid, false);

                // map the request - override the values of the destination object w/ source
                var cityToPatch = _mapper.Map<CityUpdateDto>(existingCity);

                // apply the patch - grab the updates and update the dto
                patchDocument.ApplyTo(cityToPatch, ModelState);

                // validate the final version
                if (!TryValidateModel(cityToPatch))
                {
                    _logger.LogWarning($"Invalid model state for the patch.");
                    return BadRequest(ModelState);
                }

                // map changes back to the entity
                _mapper.Map(cityToPatch, existingCity);

                var success = await _repo.SaveChangesAsync();
                if (!success)
                {
                    _logger.LogError("An error occurred while patching city.");
                    return StatusCode(500, "An error occurred while patching city.");
                }

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
        public async Task<ActionResult> DeleteCity([FromRoute] Guid cityGuid)
        {
            try
            {
                // does the city exist?
                var cityExists = await _repo.CityExistsAsync(cityGuid);
                if (!cityExists)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found.");
                    return NotFound();
                }

                // delete the city
                await _repo.DeleteCityAsync(cityGuid);

                var success = await _repo.SaveChangesAsync();
                if (!success)
                {
                    _logger.LogError("An error occurred while deleting city.");
                    return StatusCode(500, "An error occurred while deleting city.");
                }

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
