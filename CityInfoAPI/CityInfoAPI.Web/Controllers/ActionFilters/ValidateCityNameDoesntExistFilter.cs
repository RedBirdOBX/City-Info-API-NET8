using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CityInfoAPI.Dtos;
using CityInfoAPI.Service;
using CityInfoAPI.Dtos.RequestModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CityInfoAPI.Controllers.Filters;

/// <summary>
/// ActionFilter to ensure a city with the same name does not already exist.
/// </summary>
public class ValidateCityNameDoesNotExistFilter : ActionFilterAttribute
{

    private readonly ICityService _service;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="service"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ValidateCityNameDoesNotExistFilter(ICityService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(_service));
    }

    /// <summary>
    /// execution of action filter.
    /// </summary>
    /// <param name="context"></param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionArguments.TryGetValue("request", out var value) && value is CityCreateDto dto)
        {
            var requestParams = new CityRequestParameters
            {
                Name = dto.Name
            };

            // TO DO: Check state as well....
            var matchingCity = _service.GetCitiesAsync(requestParams).Result;
            if (matchingCity.Any())
            {
                var modelState = new ModelStateDictionary();
                modelState.AddModelError("Name", $"A city with the name {requestParams.Name.ToLower()} already exists.");

                var problemDetails = new ValidationProblemDetails(modelState)
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "One or more validation errors occurred.",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                };

                 context.Result = new ConflictObjectResult(problemDetails);
            }
        }
        base.OnActionExecuting(context);
    }
}