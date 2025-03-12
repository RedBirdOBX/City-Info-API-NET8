using Asp.Versioning;
using AutoMapper;
using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Service;
using CityInfoAPI.Web.Controllers.RequestHelpers;
using CityInfoAPI.Web.Controllers.RequestHelpers.Models;
using CityInfoAPI.Web.Controllers.ResponseHelpers;
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class CitiesController : ControllerBase
    {
        private readonly ILogger<CitiesController> _logger;
        private readonly ICityService _service;
        private readonly IMapper _mapper;
        private IHttpContextAccessor _httpContextAccessor;
        private LinkGenerator _linkGenerator;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="linkGenerator"></param>
        public CitiesController(ILogger<CitiesController> logger, IMapper mapper, ICityService service,
            IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(service));
            _linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
        }

        /// <summary>Gets all Cities</summary>
        /// <returns>collection of CityDto</returns>
        /// <example>{baseUrl}/api/cities</example>
        /// <response code="200">returns city by id</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("", Name = "GetCities")]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities([FromQuery] CityRequestParameters requestParams)
        {
            try
            {
                // record the request
                var url = Url.Link("GetCities", new { requestParams.IncludePointsOfInterest, requestParams.Name, requestParams.Search, requestParams.PageNumber, requestParams.PageSize });
                _logger.LogInformation($"Getting cities URL: {url}");

                // META DATA. building meta data. correct page size if needed
                if (requestParams.PageSize > RequestConstants.MAX_PAGE_SIZE)
                {
                    requestParams.PageSize = RequestConstants.MAX_PAGE_SIZE;
                }

                // how many total pages do we have?
                int citiesCount = 0;

                // did they use a name filter? count all possible results
                if (!string.IsNullOrEmpty(requestParams.Name))
                {
                    var allCities = await _service.GetAllCitiesAsync();
                    var filteredCities = allCities.Where(c => c.Name.ToLower().Contains(requestParams.Name.ToLower()));
                    citiesCount = filteredCities.Count();

                    // bad filter was used
                    if (citiesCount == 0)
                    {
                        return Ok($"No cities found with the name containing {requestParams.Name}.");
                    }
                }
                else
                {
                    // count all
                    citiesCount = (await _service.GetAllCitiesAsync()).Count();
                }

                int totalPages = (int)Math.Ceiling(citiesCount / (double)requestParams.PageSize);

                if (requestParams.PageNumber > totalPages)
                {
                    return BadRequest("You have requested more pages that are available.");
                }

                var metaData = MetaDataUtility.BuildCitiesMetaData(requestParams, citiesCount, _httpContextAccessor, _linkGenerator);
                Response.Headers.Append("X-CityParameters", JsonConvert.SerializeObject(metaData));
                // end of META DATA

                var results = await _service.GetCitiesAsync(requestParams.Name ?? string.Empty, requestParams.Search ?? string.Empty, requestParams.PageNumber, requestParams.PageSize);

                // add helper links
                foreach (var city in results)
                {
                    city.Links.Add(UriLinkHelper.CreateLinkForCityWithinCollection(HttpContext.Request, city));
                }

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

                var cityExists = await _service.CityExistsAsync(cityGuid);
                if (!cityExists)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found.");
                    return NotFound();
                }

                if (includePointsOfInterest)
                {
                    var city = await _service.GetCityAsync(cityGuid, includePointsOfInterest);
                    city = UriLinkHelper.CreateLinksForCityWithPointsOfInterest(HttpContext.Request, city ?? new CityDto(), RequestConstants.MAX_PAGE_SIZE);

                    // if points of interest were included
                    foreach (PointOfInterestDto poi in city.PointsOfInterest)
                    {
                        poi.Links.Add(UriLinkHelper.CreateLinkForPointOfInterestWithinCollection(HttpContext.Request, poi));
                    }

                    return Ok(city);
                }
                else
                {
                    var city = await _service.GetCityWithoutPointsOfInterestAsync(cityGuid, includePointsOfInterest);
                    city = UriLinkHelper.CreateLinksForCity(HttpContext.Request, city ?? new CityWithoutPointsOfInterestDto(), RequestConstants.MAX_PAGE_SIZE);
                    return Ok(city);
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

                // guids are auto-generated and not provided by client. unlikely but just in case.
                var cityExists = await _service.CityExistsAsync(request.CityGuid);
                if (cityExists)
                {
                    return Conflict($"City {request.CityGuid} already exists.");
                }

                var newCity = await _service.CreateCityAsync(request);
                if (newCity == null)
                {
                    _logger.LogError("An error occurred while creating city.");
                    return StatusCode(500, "An error occurred while creating city.");
                }

                return CreatedAtRoute("GetCityByCityId", new { cityGuid = newCity.CityGuid }, newCity);
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

                var cityExists = await _service.CityExistsAsync(cityGuid);
                if (!cityExists)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found.");
                    return NotFound();
                }

                var updatedCity = await _service.UpdateCityAsync(request, cityGuid);
                if (updatedCity == null)
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

                var cityExists = await _service.CityExistsAsync(cityGuid);
                if (!cityExists)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found when patching city.");
                    return NotFound();
                }

                var existingCity = await _service.GetCityAsync(cityGuid, false);

                // map the request - override the values of the destination object w/ source
                var cityToPatch = _mapper.Map<CityUpdateDto>(existingCity);

                // apply the patch - grab the updates and update the dto
                patchDocument.ApplyTo(cityToPatch, ModelState);

                // see if updates are valid
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"Invalid model state for the patch.");
                    return BadRequest(ModelState);
                }

                // validate the final version
                if (!TryValidateModel(cityToPatch))
                {
                    _logger.LogWarning($"Invalid model state for the patch.");
                    return BadRequest(ModelState);
                }

                // map changes back to the entity. source / destination
                _mapper.Map(cityToPatch, existingCity);

                // now that we have a updated entity, try to save it.
                var updatedCity = await _service.UpdateCityAsync(cityToPatch, cityGuid);
                if (updatedCity == null)
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
                var cityExists = await _service.CityExistsAsync(cityGuid);
                if (!cityExists)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found.");
                    return NotFound();
                }

                // delete the city
                var success = await _service.DeleteCityAsync(cityGuid);
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
