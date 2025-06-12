using Asp.Versioning;
using AutoMapper;
using CityInfoAPI.Dtos;
using CityInfoAPI.Service;
using CityInfoAPI.Web.Controllers.ResponseHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers;

/// <summary>
/// States controller
/// </summary>
/// <response code="401">unauthorized request</response>
/// <response code="500">internal error</response>
[Route("api/v{version:apiVersion}/states")]
[ApiController]
[Authorize]
[ApiVersion(1.0)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class StatesController : ControllerBase
{
    private readonly ILogger<StatesController> _logger;
    private readonly IStateService _service;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    /// <param name="service"></param>
    public StatesController(ILogger<StatesController> logger, IMapper mapper, IStateService service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>Gets all States</summary>
    /// <returns>collection of StateDto</returns>
    /// <example>{baseUrl}/api/states</example>
    /// <response code="200">returns states</response>B
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("", Name = "GetStates")]
    public async Task<ActionResult<IEnumerable<StateDto>>> GetStates()
    {
        try
        {
            // record the request
            var url = Url.Link("GetStates", null);
            _logger.LogInformation($"Getting states URL: {url}");

            var states = await _service.GetStatesAsync();

            // add helper links
            foreach (var state in states)
            {
                state.Links.Add(UriLinkHelper.CreateLinkForStateWithinCollection(HttpContext.Request, state));
            }

            return Ok(states);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while getting states. {ex}");
            return StatusCode(500, "An error occurred while getting states.");
        }
    }

    /// <summary>returns state by state code</summary>
    /// <param name="stateCode"></param>
    /// <returns>StateDto</returns>
    /// <example>{baseUrl}/api/states/{stateAbbrev}</example>
    /// <response code="200">returns state by state code</response>
    /// <response code="400">bad request for getting state by state code</response>
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{stateCode}", Name = "GetStateByStateCode")]
    public async Task<ActionResult<StateDto>> GetStateByStateCode([FromRoute] string stateCode)
    {
        try
        {
            if (stateCode.Length != 2)
            {
                return BadRequest($"State code '{stateCode}' is not valid. It must be 2 characters long.");
            }

            var url = Url.Link("GetStateByStateCode", null);
            _logger.LogInformation($"Getting State by State Code URL: {url}");

            var stateExists = await _service.StateExistsAsync(stateCode);
            if (!stateExists)
            {
                _logger.LogWarning($"State with code {stateCode} wasn't found.");
                return NotFound();
            }

            var state = await _service.GetStateAsync(stateCode);
            state = UriLinkHelper.CreateLinksForState(HttpContext.Request, state ?? new StateDto());
            return Ok(state);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while getting state. {ex}");
            return StatusCode(500, "An error occurred while getting state.");
        }
    }
}
