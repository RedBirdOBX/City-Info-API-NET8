namespace CityInfoAPI.Dtos;

/// <summary>
/// city object
/// </summary>
public class CityTempDto
{

    /// <summary>
    /// name of city
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// description of city
    /// </summary>
    public string? Description { get; set; }
}
