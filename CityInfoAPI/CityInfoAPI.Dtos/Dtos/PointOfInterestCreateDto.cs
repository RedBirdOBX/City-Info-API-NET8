using System.ComponentModel.DataAnnotations;

namespace CityInfoAPI.Dtos;

/// <summary>
/// object for creating a point of interest
/// </summary>
public class PointOfInterestCreateDto
{
    /// <summary>
    /// unique identifier for the point of interest
    /// </summary>
    public Guid PointGuid { get; set; } = Guid.NewGuid();

    /// <summary>
    /// unique identifier for the city
    /// </summary>
    [Required(ErrorMessage = $"{nameof(CityGuid)} is required.")]
    public Guid CityGuid { get; set; }

    /// <summary>
    /// name of the point of interest
    /// </summary>
    [Required(ErrorMessage = $"{nameof(Name)} is required.")]
    [MaxLength(ErrorMessage = $"Max length for {nameof(Name)} is 50 chars.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// description of the point of interest
    /// </summary>
    [MaxLength(ErrorMessage = $"Max length for {nameof(Description)} is 500 chars.")]
    public string? Description { get; set; }
}
