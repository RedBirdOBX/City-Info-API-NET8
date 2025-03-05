using Asp.Versioning;
using AutoMapper;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CityInfoAPI.Controllers
{
    /// <summary>
    /// Cities controller
    /// </summary>
    /// <response code="401">unauthorized request</response>
    /// <response code="500">internal error</response>
    [Route("api/v{version:apiVersion}/cities")]
    [ApiController]
    [Authorize]
    [ApiVersion(1.0)]
    [ApiVersion(2.0)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class CitiesController : ControllerBase
    {
        private readonly ILogger<CitiesController> _logger;
        private readonly ICityInfoRepository _repo;
        private readonly ICityInfoService _service;
        private readonly IResponseHeaderService _headerService;
        private readonly IMapper _mapper;
        private readonly int _maxPageSize = 100;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="repo"></param>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        /// <param name="headerService"></param>
        public CitiesController(ILogger<CitiesController> logger, ICityInfoRepository repo, IMapper mapper, ICityInfoService service, IResponseHeaderService headerService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _headerService = headerService ?? throw new ArgumentNullException(nameof(headerService));
        }

        /// <summary>Gets all Cities</summary>
        /// <returns>collection of CityDto</returns>
        /// <param name="includePointsOfInterest"></param>
        /// <param name="name"></param>
        /// <param name="search"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <example>{baseUrl}/api/cities</example>
        /// <response code="200">returns city by id</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("", Name = "GetCities")]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities([FromQuery] bool? includePointsOfInterest = true,
            [FromQuery(Name = "name")] string? name = null, [FromQuery(Name = "search")] string? search = null,
            [FromQuery(Name = "pageNumber")] int pageNumber = 1, [FromQuery(Name = "pageSize")] int pageSize = 100)
        {
            try
            {
                var url = Url.Link("GetCities", new { includePointsOfInterest, name, search, pageNumber, pageSize });
                _logger.LogInformation($"Getting cities URL: {url}");

                if (pageSize > _maxPageSize)
                {
                    pageSize = _maxPageSize;
                }

                var metaData = _headerService.BuildCitiesHeaderMetaData(pageNumber, pageSize);
                Response.Headers.Append("X-CityParameters", JsonConvert.SerializeObject(metaData));

                var results = await _service.GetCitiesAsync(name, search, pageNumber, pageSize);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting cities. {ex}");
                return StatusCode(500, "An error occurred while getting cities.");
            }
        }

        /// <summary>returns city by id</summary>
        /// <param name="cityGuid"></param>
        /// <param name="includePointsOfInterest"></param>
        /// <returns>CityDto</returns>
        /// <example>{baseUrl}/api/cities/{cityGuid}?includePointsOfInterest={bool}</example>
        /// <response code="200">returns city by id</response>
        /// <response code="400">bad request for getting city by id</response>
        [HttpGet("{cityGuid}", Name = "GetCityByCityId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CityWithoutPointsOfInterestDto>> GetCityByCityId([FromRoute] Guid cityGuid, [FromQuery] bool includePointsOfInterest = true)
        {
            try
            {
                var url = Url.Link("GetCityByCityId", new { cityGuid, includePointsOfInterest });
                _logger.LogInformation($"Getting City By Id URL: {url}");

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
                _logger.LogError($"An error occurred while getting city. {ex}");
                return StatusCode(500, "An error occurred while getting city.");
            }
        }

        /// <summary>creates a City</summary>
        /// <param name="request"></param>
        /// <returns>CityDto at details route</returns>
        /// <example>{baseUrl}/api/cities</example>
        /// <response code="201">city created</response>
        /// <response code="409">conflict of data - city already exists</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost("", Name = "CreateCity")]
        public async Task<ActionResult<CityWithoutPointsOfInterestDto>> CreateCity([FromBody] CityCreateDto request)
        {
            try
            {
                var url = Url.Link("CreateCity", null);
                _logger.LogInformation($"CreateCity URL: {url}. Request: {JsonConvert.SerializeObject(request)}");

                // guids are auto-generated and not provided by client. unlikely but just incase.
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
                _logger.LogError($"An error occurred while creating city. {ex}");
                return StatusCode(500, "An error occurred while creating city.");
            }
        }

        /// <summary>updates city through PUT</summary>
        /// <param name="cityGuid"></param>
        /// <param name="request"></param>
        /// <returns>No Content</returns>
        /// <example>{baseUrl}/api/cities/{cityGuid}</example>
        /// <response code="204">city updated</response>
        /// <response code="404">city not found</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{cityGuid}", Name = "UpdateCity")]
        public async Task<ActionResult> UpdateCity([FromRoute] Guid cityGuid, [FromBody] CityUpdateDto request)
        {
            try
            {
                var url = Url.Link("UpdateCity", null);
                _logger.LogInformation($"UpdateCity URL: {url}. Request: {JsonConvert.SerializeObject(request)}");

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
                _logger.LogError($"An error occurred while updating city. {ex}");
                return StatusCode(500, "An error occurred while updating city.");
            }
        }

        /// <summary>patches city object</summary>
        /// <param name="cityGuid"></param>
        /// <param name="patchDocument"></param>
        /// <returns>No Content</returns>
        /// <example>{baseUrl}/api/cities/{cityGuid}</example>
        /// <response code="204">city updated</response>
        /// <response code="400">city has bad data</response>
        /// <response code="404">city not found</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{cityGuid}", Name = "PatchCity")]
        public async Task<ActionResult<CityDto>> PatchCity([FromRoute] Guid cityGuid, [FromBody] JsonPatchDocument<CityUpdateDto> patchDocument)
        {
            try
            {
                var url = Url.Link("PatchCity", null);
                _logger.LogInformation($"PatchCity URL: {url}. Request: {JsonConvert.SerializeObject(patchDocument)}");

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
                _logger.LogError($"An error occurred while patching city. {ex}");
                return StatusCode(500, "An error occurred while patching city.");
            }
        }

        /// <summary>deletes city object</summary>
        /// <param name="cityGuid"></param>
        /// <returns>no content</returns>
        /// <example>{baseUrl}/api/cities/{cityGuid}</example>
        /// <response code="204">city deleted</response>
        /// <response code="404">city not found</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{cityGuid}", Name = "DeleteCity")]
        public async Task<ActionResult> DeleteCity([FromRoute] Guid cityGuid)
        {
            try
            {
                var url = Url.Link("DeleteCity", null);
                _logger.LogInformation($"DeleteCity URL: {url}.");

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
                _logger.LogError($"An error occurred while deleting city. {ex}");
                return StatusCode(500, "An error occurred while deleting city.");
            }
        }
    }
}
