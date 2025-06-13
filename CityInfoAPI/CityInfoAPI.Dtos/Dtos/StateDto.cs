namespace CityInfoAPI.Dtos;

/// <summary>
/// state object
/// </summary>
public class StateDto : LinkedResourcesDto
{
    /// <summary>
    /// unique identifier for the state
    /// </summary>
    public Guid StateGuid { get; set; } = new Guid();

    /// <summary>
    /// state abbreviation
    /// </summary>
    public string StateCode { get; set; } = string.Empty;
}
