using System.ComponentModel.DataAnnotations;

namespace CityInfoAPI.Dtos;

/// <summary>
/// object for updating a point of interest
/// </summary>
public class PointOfInterestUpdateDto
{
    /// <summary>
    /// name of point of interest
    /// </summary>
    [Required(ErrorMessage = $"{nameof(Name)} is required.")]
    [MaxLength(ErrorMessage = $"Max length for {nameof(Name)} is 50 chars.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// description of point of interest
    /// </summary>
    [MaxLength(ErrorMessage = $"Max length for {nameof(Description)} is 500 chars.")]
    public string? Description { get; set; }
}
