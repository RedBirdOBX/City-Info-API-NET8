using CityInfoAPI.Dtos;
using CityInfoAPI.Web.Controllers.ResponseHelpers;
using Microsoft.AspNetCore.Mvc;


namespace CityInfoAPI.Web.Controllers;

/// <summary>
/// Root Controller
/// </summary>
[Route("api")]
[ApiController]
public class RootController : ControllerBase
{

    private string _appVersion = "1.0";
    private readonly IConfiguration _configuration;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="configuration"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public RootController(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _appVersion = _configuration["AppVersion"] ?? string.Empty;
    }


    /// <summary>
    /// Root of Application
    /// </summary>
    /// <returns>IActionResult</returns>
    [HttpGet(Name = "GetRoot")]
    public IActionResult GetRoot()
    {
        // create links for root
        var links = new List<LinkDto>();

        var rootLink = Url.Link("GetRoot", null);
        if (rootLink != null)
        {
            links.Add(new LinkDto(rootLink, "self", "GET"));
        }

        var citiesLink = UriLinkHelper.CreateLinkForCitiesAtRoot(HttpContext.Request, _appVersion);
        if (!string.IsNullOrEmpty(citiesLink))
        {
            links.Add(new LinkDto(citiesLink, "cities", "GET"));
        }

        var authLink = UriLinkHelper.CreateLinkForAuthenticateAtRoot(HttpContext.Request, _appVersion);
        if (!string.IsNullOrEmpty(authLink))
        {
            links.Add(new LinkDto(authLink, "authenticate", "POST"));
        }
        return Ok(links);
    }
}