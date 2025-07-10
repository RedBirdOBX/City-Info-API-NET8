using Asp.Versioning;
using AutoMapper;
using CityInfoAPI.Dtos;
using CityInfoAPI.Service;
using CityInfoAPI.Web.Controllers.ResponseHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace CityInfoAPI.Controllers;


/// <summary>
/// points of interest controller
/// </summary>
/// <response code="401">unauthorized request</response>
/// <response code="500">internal error</response>
[Route("api/v{version:apiVersion}")]
[ApiController]
[Authorize]
[ApiVersion(1.0)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class PointsOfInterestController : ControllerBase
{
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;
    private readonly IMapper _mapper;
    private readonly IPointsOfInterestService _service;
    private readonly ICityService _cityService;
    private readonly IConfiguration _configuration;
    private string _appVersion = "1.0";

    /// <summary>constructor</summary>
    /// <param name="logger"></param>
    /// <param name="mailService"></param>
    /// <param name="service"></param>
    /// <param name="cityService"></param>
    /// <param name="mapper"></param>
    /// <param name="configuration"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, IMapper mapper,
                                        IPointsOfInterestService service, ICityService cityService, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _cityService = cityService ?? throw new ArgumentNullException(nameof(service));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _appVersion = _configuration["AppVersion"] ?? string.Empty;
    }

    /// <summary>Gets all Points of Interest</summary>
    /// <returns>collection of points of interest</returns>
    /// <example>{baseUrl}/api/pointsofinterest</example>
    /// <response code="200">returns points of interest for city</response>
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("pointsofinterest", Name = "GetPointsOfInterest")]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest([FromQuery(Name = "name")] string? name = null,
        [FromQuery(Name = "search")] string? search = null)
    {
        try
        {
            var url = Url.Link("GetPointsOfInterest", new { name = name, search = search });
            _logger.LogInformation($"GetPointsOfInterest called. Url: {url}");

            var pointsOfInterest = await _service.GetPointsOfInterestAsync(name, search);

            foreach (var pointOfInterest in pointsOfInterest)
            {
                pointOfInterest.Links.Add(UriLinkHelper.CreateLinkForPointOfInterestWithinCollection(HttpContext.Request, pointOfInterest, _appVersion));
            }
            return Ok(pointsOfInterest);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while getting points of interest. {ex}");
            return StatusCode(500, "An error occurred while getting points of interest.");
        }
    }

    /// <summary>Gets all Points of Interest for City</summary>
    /// <returns>collection of points of interest</returns>
    /// <param name="cityGuid"></param>
    /// <example>{baseUrl}/api/cities/{cityGuid}/pointsofinterest</example>
    /// <response code="200">returns points of interest for city</response>
    /// <response code="404">city not found</response>
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("cities/{cityGuid}/pointsofinterest", Name = "GetPointsOfInterestForCity")]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterestForCity([FromRoute] Guid cityGuid)
    {
        try
        {
            var url = Url.Link("GetPointsOfInterestForCity", null);
            _logger.LogInformation($"GetPointsOfInterestForCity called. Url: {url}");

            var cityExists = await _cityService.CityExistsAsync(cityGuid);
            if (!cityExists)
            {
                _logger.LogWarning($"City with id {cityGuid} wasn't found when accessing points of interest.");
                return NotFound();
            }

            var pointsOfInterest = await _service.GetPointsOfInterestForCityAsync(cityGuid);

            foreach (var pointOfInterest in pointsOfInterest)
            {
                pointOfInterest.Links.Add(UriLinkHelper.CreateLinkForPointOfInterestWithinCollection(HttpContext.Request, pointOfInterest, _appVersion));
            }

            return Ok(pointsOfInterest);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while getting points of interest for city {cityGuid}. {ex}");
            return StatusCode(500, $"An error occurred while getting points of interest for city {cityGuid}.");
        }
    }

    /// <summary>gets point of interest by id</summary>
    /// <param name="cityGuid"></param>
    /// <param name="pointGuid"></param>
    /// <returns>point of interest</returns>
    /// <example>{baseUrl}/api/cities/{cityGuid}/pointsofinterest/{pointGuid}</example>
    /// <response code="200">returns point of interest by id for city</response>
    /// <response code="404">city or point of interest not found</response>
    //[Authorize(Policy = "MustBeFromRichmond")] - DEMO policy enforcement
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

            var url = Url.Link("GetPointOfInterestById", null);
            _logger.LogInformation($"GetPointOfInterestById called. Url: {url}");

            var cityExists = await _cityService.CityExistsAsync(cityGuid);
            if (!cityExists)
            {
                _logger.LogWarning($"City with id {cityGuid} wasn't found when accessing points of interest.");
                return NotFound();
            }

            var pointExists = await _service.PointOfInterestExistsAsync(pointGuid);
            if (!pointExists)
            {
                _logger.LogWarning($"Point of Interest with id {pointGuid} was not found.");
                return NotFound();
            }

            var pointOfInterest = await _service.GetPointOfInterestAsync(pointGuid);
            if (pointOfInterest == null)
            {
                _logger.LogWarning($"Point of interest with id {pointGuid}  for City {cityGuid} wasn't found.");
                return NotFound();
            }

            pointOfInterest = UriLinkHelper.CreateLinksForPointOfInterest(HttpContext.Request, pointOfInterest, _appVersion);

            return Ok(pointOfInterest);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while getting point of interest. {ex}");
            return StatusCode(500, "An error occurred while getting point of interest.");
        }
    }

    /// <summary>creates a point of interest</summary>
    /// <param name="cityGuid"></param>
    /// <param name="request"></param>
    /// <returns>new point of interest at route</returns>
    /// <example>{baseUrl}/api/cities/{cityGuid}/pointsofinterest</example>
    /// <response code="201">returns created at route for new point of interest</response>
    /// <response code="404">city not found</response>
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("cities/{cityGuid}/pointsofinterest/", Name = "CreatePointOfInterest")]
    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest([FromRoute] Guid cityGuid, [FromBody] PointOfInterestCreateDto request)
    {
        try
        {
            var url = Url.Link("CreatePointOfInterest", null);
            _logger.LogInformation($"CreatePointOfInterest called. Url: {url}. Request: {JsonConvert.SerializeObject(request)}");

            var cityExists = await _cityService.CityExistsAsync(cityGuid);
            if (!cityExists)
            {
                _logger.LogWarning($"City with id {cityGuid} wasn't found when creating point of interest.");
                return NotFound();
            }

            // check poi count
            var countOfPointsOfInterest = await _service.CountPointsOfInterestForCityAsync(cityGuid);
            if (int.TryParse(_configuration["PointsOfInterestCityLimit"], out var poiLimit))
            {
                if (countOfPointsOfInterest > poiLimit)
                {
                    return BadRequest($"City can only have {poiLimit} points of interest.");
                }
            }

            var newPointResults = await _service.CreatePointOfInterestAsync(cityGuid, request);

            if (newPointResults == null)
            {
                _logger.LogError("An error occurred while point of interest.");
                return StatusCode(500, "An error occurred while creating point of interest.");
            }

            return CreatedAtRoute("GetPointOfInterestById", new { cityGuid = cityGuid, pointGuid = newPointResults.PointGuid }, newPointResults);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while creating point of interest. {ex}");
            return StatusCode(500, "An error occurred while creating point of interest.");
        }
    }

		/// <summary>
    /// user should not be able to POST to an existing point of interest.
    /// </summary>
    /// <param name="cityGuid"></param>
    /// <param name="pointGuid"></param>
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("cities/{cityGuid}/pointsofinterest/{pointGuid}", Name = "BlockPostToExistingPointOfInterest")]
    public async Task<ActionResult> BlockPostToExistingPointOfInterest([FromRoute] Guid cityGuid, [FromRoute] Guid pointGuid)
    {
        // user should not be able to POST to an existing point of interest. anything with an id should
        // be done with a PUT or a PATCH.
        try
        {
            bool doesCityExist = await _cityService.CityExistsAsync(cityGuid);
            if (!doesCityExist)
            {
                return BadRequest("City does not exist.");
            }

            bool doesPointOfInterestExist = await _service.PointOfInterestExistsAsync(pointGuid);
            if (!doesPointOfInterestExist)
            {
                return BadRequest("You cannot post to points of interest like this.");
            }
            else
            {
                return StatusCode(409, "You cannot post to an existing point of interest!");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while creating city. {ex}");
            return StatusCode(500, "An error occurred while creating city.");
        }
    }

    /// <summary>updates point of interest through PUT</summary>
    /// <param name="cityGuid"></param>
    /// <param name="pointGuid"></param>
    /// <param name="updatePointOfInterest"></param>
    /// <returns>No Content</returns>
    /// <example>{baseUrl}/api/cities/{cityGuid}/pointsofinterest/{pointGuid}</example>
    /// <response code="204">updated point of interest</response>
    /// <response code="404">city or point of interest not found</response>
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("cities/{cityGuid}/pointsofinterest/{pointGuid}", Name = "UpdatePointOfInterest")]
    public async Task<ActionResult> UpdatePointOfInterest([FromRoute] Guid cityGuid, [FromRoute] Guid pointGuid, [FromBody] PointOfInterestUpdateDto updatePointOfInterest)
    {
        try
        {
            var url = Url.Link("UpdatePointOfInterest", null);
            _logger.LogInformation($"UpdatePointOfInterest called. Url: {url}. Request: {JsonConvert.SerializeObject(updatePointOfInterest)}");

            var cityExists = await _cityService.CityExistsAsync(cityGuid);
            if (!cityExists)
            {
                _logger.LogWarning($"City with id {cityGuid} wasn't found when creating point of interest.");
                return NotFound();
            }

            // find the point of interest
            var pointExists = await _service.PointOfInterestExistsAsync(pointGuid);
            if (!pointExists)
            {
                _logger.LogWarning($"Point of interest with id {pointGuid} wasn't found.");
                return NotFound();
            }

            // does the point of interest belong to the city?
            var existingPointOfInterest = await _service.GetPointOfInterestAsync(pointGuid);
            if (existingPointOfInterest == null || existingPointOfInterest.CityGuid != cityGuid)
            {
                _logger.LogWarning($"Point of interest with id {pointGuid} doesn't belong to city with id {cityGuid}.");
                return NotFound();
            }

            var results = await _service.UpdatePointOfInterestAsync(cityGuid, pointGuid, updatePointOfInterest);

            if (results == null)
            {
                _logger.LogError("An error occurred while updating point of interest.");
                return StatusCode(500, "An error occurred while updating point of interest.");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while updating point of interest. {ex}");
            return StatusCode(500, "An error occurred while updating point of interest.");
        }
    }

    /// <summary>patches point of interest</summary>
    /// <param name="cityGuid"></param>
    /// <param name="pointGuid"></param>
    /// <param name="patchDocument"></param>
    /// <returns>No Content</returns>
    /// <example>{baseUrl}/api/cities/{cityGuid}/pointsofinterest/{pointGuid}</example>
    /// <response code="204">updated point of interest</response>
    /// <response code="404">city or point of interest not found</response>
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPatch("cities/{cityGuid}/pointsofinterest/{pointGuid}", Name = "PatchPointOfInterest")]
    public async Task<ActionResult<PointOfInterestDto>> PatchPointOfInterest([FromRoute] Guid cityGuid, [FromRoute] Guid pointGuid, [FromBody] JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
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

                var url = Url.Link("PatchPointOfInterest", null);
                _logger.LogInformation($"PatchPointOfInterest called. Url: {url}. Request: {JsonConvert.SerializeObject(patchDocument)}");

                var cityExists = await _cityService.CityExistsAsync(cityGuid);
                if (!cityExists)
                {
                    _logger.LogWarning($"City with id {cityGuid} wasn't found when patching point of interest.");
                    return NotFound();
                }

                // find the point of interest
                var pointExists = await _service.PointOfInterestExistsAsync(pointGuid);
                if (!pointExists)
                {
                    _logger.LogWarning($"Point of interest with id {pointGuid} wasn't found.");
                    return NotFound();
                }

                // does the point of interest belong to the city?
                var existingPointOfInterest = await _service.GetPointOfInterestAsync(pointGuid);
                if (existingPointOfInterest == null || existingPointOfInterest.CityGuid != cityGuid)
                {
                    _logger.LogWarning($"Point of interest with id {pointGuid} doesn't belong to city with id {cityGuid}.");
                    return NotFound();
                }

                // map the request - override the values of the destination object w/ source
                var pointOfInterestToPatch = _mapper.Map<PointOfInterestUpdateDto>(existingPointOfInterest);

                // apply the patch - grab the updates and update the dto
                patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

                // see if updates are valid
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"Invalid model state for the patch.");
                    return BadRequest(ModelState);
                }

                // validate the final version
                if (!TryValidateModel(pointOfInterestToPatch))
                {
                    _logger.LogWarning($"Invalid model state for the patch.");
                    return BadRequest(ModelState);
                }

                // map changes back to the entity. source / destination
                _mapper.Map(pointOfInterestToPatch, existingPointOfInterest);

                // now that we have a updated entity, try to save it.
                var updatedPoint = await _service.UpdatePointOfInterestAsync(cityGuid, pointGuid, pointOfInterestToPatch);
                if (updatedPoint == null)
                {
                    _logger.LogError("An error occurred while patching point of interest.");
                    return StatusCode(500, "An error occurred while patching point of interest.");
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
            _logger.LogError($"An error occurred while patching point of interest. {ex}");
            return StatusCode(500, "An error occurred while patching point of interest.");
        }
    }

    /// <summary>deletes point of interest</summary>
    /// <param name="cityGuid"></param>
    /// <param name="pointGuid"></param>
    /// <returns>No Content</returns>
    /// <example>{baseUrl}/api/cities/{cityGuid}/pointsofinterest/{pointGuid}</example>
    /// <response code="204">deleted point of interest</response>
    /// <response code="404">city or point of interest not found</response>
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("cities/{cityGuid}/pointsofinterest/{pointGuid}", Name = "DeletePointOfInterest")]
    public async Task<ActionResult> DeletePointOfInterest([FromRoute] Guid cityGuid, [FromRoute] Guid pointGuid)
    {
        try
        {
            var url = Url.Link("DeletePointOfInterest", null);
            _logger.LogInformation($"DeletePointOfInterest called. Url: {url}.");

            // does the city exist?
            var cityExists = await _cityService.CityExistsAsync(cityGuid);
            if (!cityExists)
            {
                _logger.LogWarning($"City with id {cityGuid} wasn't found when deleting point of interest.");
                return NotFound();
            }

            // find the point of interest
            var pointExists = await _service.PointOfInterestExistsAsync(pointGuid);
            if (!pointExists)
            {
                _logger.LogWarning($"Point of interest with id {pointGuid} wasn't found.");
                return NotFound();
            }

            // does the point of interest belong to the city?
            var existingPointOfInterest = await _service.GetPointOfInterestAsync(pointGuid);
            if (existingPointOfInterest == null || existingPointOfInterest.CityGuid != cityGuid)
            {
                _logger.LogWarning($"Point of interest with id {pointGuid} doesn't belong to city with id {cityGuid}.");
                return NotFound();
            }

            // delete the point of interest
            var results = await _service.DeletePointOfInterestAsync(pointGuid);
            if (!results)
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
            _logger.LogError($"An error occurred while deleting point of interest. {ex}");
            return StatusCode(500, "An error occurred while deleting point of interest.");
        }
    }
}
