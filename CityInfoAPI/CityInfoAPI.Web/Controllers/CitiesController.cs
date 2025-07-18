using Asp.Versioning;
using AutoMapper;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Data.PropertyMapping;
using CityInfoAPI.Dtos;
using CityInfoAPI.Dtos.RequestModels;
using CityInfoAPI.Service;
using CityInfoAPI.Web.Controllers.Constants;
using CityInfoAPI.Web.Controllers.ResponseHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CityInfoAPI.Controllers;

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
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly IPropertyMappingProcessor _propProcessor;
    private IHttpContextAccessor _httpContextAccessor;
    private LinkGenerator _linkGenerator;
    private string _appVersion = "1.0";

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    /// <param name="service"></param>
    /// <param name="configuration"></param>
    /// <param name="httpContextAccessor"></param>
    /// <param name="linkGenerator"></param>
    public CitiesController(ILogger<CitiesController> logger, IMapper mapper, ICityService service, IConfiguration configuration,
                            IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator, IPropertyMappingProcessor propProcessor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
        _propProcessor = propProcessor ?? throw new ArgumentNullException(nameof(propProcessor));
        _appVersion = _configuration["AppVersion"] ?? string.Empty;
    }

    /// <summary>Gets all Cities</summary>
    /// <returns>collection of CityDto</returns>
    /// <example>{baseUrl}/api/cities?pageNumber=1&pageSize=100&includePointsOfInterest=true&name=foo&search=bar</example>
    /// <response code="200">returns cities</response>
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("", Name = "GetCities")]
    [HttpHead("", Name = "GetCities")]
    public async Task<ActionResult<IEnumerable<CityDto>>> GetCities([FromQuery] CityRequestParameters requestParams)
    {
        try
        {
            // check the orderby param in querystring
            if (!_propProcessor.ValidMappingExistsFor<CityDto, City>(requestParams.OrderBy))
            {
                _logger.LogWarning($"Invalid orderby parameter: {requestParams.OrderBy}");
                return BadRequest($"Invalid OrderBy parameter: {requestParams.OrderBy}");
            }

            // record the request
            // Url.Link will build the url with non-null values
            var url = Url.Link("GetCities", requestParams);
            _logger.LogInformation($"Getting cities URL: {url}");

            // META DATA. building meta data. correct page size if needed
            if (requestParams.PageSize > RequestConstants.MAX_PAGE_SIZE)
            {
                requestParams.PageSize = RequestConstants.MAX_PAGE_SIZE;
            }

            // how many total pages do we have?
            int citiesCount = await _service.CountCitiesAsync(requestParams);
            int totalPages = (int)Math.Ceiling(citiesCount / (double)requestParams.PageSize);

            if (requestParams.PageNumber > totalPages)
            {
                return BadRequest("You have requested more pages that are available.");
            }

            var metaData = MetaDataUtility.BuildCitiesMetaData(requestParams, citiesCount, _httpContextAccessor, _linkGenerator);
            Response.Headers.Append("X-CityParameters", JsonConvert.SerializeObject(metaData));
            // end of META DATA

            var results = await _service.GetCitiesAsync(requestParams);

            // add helper links
            foreach (var city in results)
            {
                city.Links.Add(UriLinkHelper.CreateLinkForCityWithinCollection(HttpContext.Request, city, _appVersion));
                city.State?.Links.Add(UriLinkHelper.CreateLinkForStateWithinCollection(HttpContext.Request, city.State, _appVersion));
            }

            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while getting cities. {ex}");
            return StatusCode(500, "An error occurred while getting cities.");
        }
    }

    /// <summary>
    /// Gets cities with requested fields based on the provided comma-separated field names.
    /// </summary>
    /// <param name="requested"></param>
    /// <returns>dynamic version of cities</returns>
    /// <example>{baseUrl}/api/cities/fields?requested=name,description</example>
    /// <response code="200">returns cities</response>
    [HttpGet("fields", Name = "GetCitiesWithRequestFields")]
    public async Task<ActionResult<IEnumerable<dynamic>>> GetCitiesWithRequestFields(string requested)
    {
        try
        {
            if (string.IsNullOrEmpty(requested))
            {
                _logger.LogWarning("Fields parameter is null or empty.");
                return BadRequest("Fields parameter cannot be null or empty.");
            }

            var results = await _service.GetCitiesWithRequestedFields(requested);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while getting cities with requested fields. {ex}");
            return StatusCode(500, "An error occurred while getting cities with requested fields.");
        }
    }

    /// <summary>mostly for demo purposes. returns options available for city by id requests</summary>
    /// <param name="cityGuid"></param>
    /// <returns>CityDto</returns>
    /// <example>{baseUrl}/api/cities/{cityGuid}?includePointsOfInterest={bool}</example>
    /// <response code="200">returns city by id</response>
    /// <response code="400">bad request for getting city by id</response>
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpOptions("{cityGuid}", Name = "GetCityByCityIdOptions")]
    public ActionResult GetCityByCityIdOptions([FromRoute] Guid cityGuid)
    {
        Response.Headers.Append("Allow", "GET, POST, PUT, DELETE, PATCH, OPTIONS, HEAD");
        return Ok();
    }

    /// <summary>returns city by id</summary>
    /// <param name="cityGuid"></param>
    /// <param name="includePointsOfInterest"></param>
    /// <returns>CityDto</returns>
    /// <example>{baseUrl}/api/cities/{cityGuid}?includePointsOfInterest={bool}</example>
    /// <response code="200">returns city by id</response>
    /// <response code="400">bad request for getting city by id</response>
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{cityGuid}", Name = "GetCityByCityId")]
    [HttpHead("{cityGuid}", Name = "GetCityByCityId")]
    public async Task<ActionResult<CityDto>> GetCityByCityId([FromRoute] Guid cityGuid, [FromQuery] bool includePointsOfInterest = true)
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

            var city = await _service.GetCityAsync(cityGuid, includePointsOfInterest);
            if (includePointsOfInterest)
            {
                city = UriLinkHelper.CreateLinksForCityWithPointsOfInterest(HttpContext.Request, city ?? new CityDto(), RequestConstants.MAX_PAGE_SIZE, _appVersion);

                // if points of interest were included
                foreach (PointOfInterestDto poi in city.PointsOfInterest)
                {
                    poi.Links.Add(UriLinkHelper.CreateLinkForPointOfInterestWithinCollection(HttpContext.Request, poi, _appVersion));
                }

                return Ok(city);
            }
            else
            {
                city = UriLinkHelper.CreateLinksForCity(HttpContext.Request, city ?? new CityDto(), RequestConstants.MAX_PAGE_SIZE, _appVersion);
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
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("", Name = "CreateCity")]
    public async Task<ActionResult<CityDto>> CreateCity([FromBody] CityCreateDto request)
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

            // check poi count
            if (int.TryParse(_configuration["PointsOfInterestCityLimit"], out var poiLimit))
            {
                if (request.PointsOfInterest.Count() > poiLimit)
                {
                    return BadRequest($"City can only have {poiLimit} points of interest.");
                }
            }

            // create the city first...
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

    /// <summary>
    /// prevents posts to existing cities
    /// </summary>
    /// <param name="cityGuid"></param>
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("{cityGuid}", Name = "BlockPostToExistingCity")]
    public async Task<ActionResult> BlockPostToExistingCity(Guid cityGuid)
    {
        // user should not be able to POST to an existing city. anything with an id should
        // be done with a PUT or a PATCH.
        try
        {
            bool doesCityExist = await _service.CityExistsAsync(cityGuid);
            if (!doesCityExist)
            {
                return BadRequest("You cannot post to cities like this.");
            }
            else
            {
                return StatusCode(409, "You cannot post to an existing city!");
            }
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
    [ProducesDefaultResponseType]
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
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPatch("{cityGuid}", Name = "PatchCity")]
    public async Task<ActionResult<CityDto>> PatchCity([FromRoute] Guid cityGuid, [FromBody] JsonPatchDocument<CityUpdateDto> patchDocument)
    {
        try
        {
            if (patchDocument.Operations.Count > 0)
            {
                Operation operation = patchDocument.Operations[0];
                if (operation.op == null)
                {
                    ModelState.AddModelError("Description", "The operation is missing (replace, add, remove, etc).");
                    return BadRequest(ModelState);
                }

                if (operation.path == null)
                {
                    ModelState.AddModelError("Description", "The path is missing. What do you want to update?");
                    return BadRequest(ModelState);
                }

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

                // If we include the optional ModelState argument, it will send back any potential errors.
                // This is where we map new values to the properties.
                // ModelState was created here when the Model Binding was applied to the input model...the JSONPatchDocument.
                // Since the framework has no way of knowing what was required and valid in the document, it will usually have
                // no errors and be valid.
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
            else
            {
                ModelState.AddModelError("Description", "The patch document is not correct.");
                return BadRequest(ModelState);
            }
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
    [ProducesDefaultResponseType]
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
