using System.ComponentModel.DataAnnotations;

namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// onject for updating a city
    /// </summary>
    public class CityUpdateDto
    {
        /// <summary>
        /// name of city
        /// </summary>
        [Required(ErrorMessage = $"{nameof(Name)} is required.")]
        [MaxLength(ErrorMessage = $"Max length for {nameof(Name)} is 50 chars.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// description of city
        /// </summary>
        [MaxLength(ErrorMessage = $"Max length for {nameof(Description)} is 500 chars.")]
        public string? Description { get; set; }
    }
}
