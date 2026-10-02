using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using CityInfoAPI.Dtos;
using CityInfoAPI.Service;
using CityInfoAPI.Dtos.RequestModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CityInfoAPI.Controllers.Filters;

/// <summary>
/// ActionFilter to ensure a city with the same name does not already exist.
/// </summary>
public class CheckForExistingCityNameFilter : ActionFilterAttribute
{

    private readonly ICityService _service;
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="service"></param>
    /// <param name="problemDetailsFactory"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CheckForExistingCityNameFilter(ICityService service, ProblemDetailsFactory problemDetailsFactory)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
    }

    /// <summary>
    /// execution of action filter.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("request", out var value) && value is CityCreateDto dto)
        {
            var requestParams = new CityRequestParameters
            {
                Name = dto.Name
            };

            var matchingCity = await _service.GetCitiesAsync(requestParams);
            if (matchingCity.Any())
            {
                var modelState = new ModelStateDictionary();
                modelState.AddModelError("Name", $"A city with the name {requestParams.Name.ToLower()} already exists.");

                // the factory applies the same defaults and customizations (type, traceId, MachineName) as the framework's own errors
                var problemDetails = _problemDetailsFactory.CreateValidationProblemDetails(
                    context.HttpContext,
                    modelState,
                    statusCode: StatusCodes.Status409Conflict,
                    title: "One or more validation errors occurred.");

                context.Result = new ConflictObjectResult(problemDetails);
                return;
            }
        }

        await next();
    }
}