using System.ComponentModel.DataAnnotations;

namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// object for creating a city
    /// </summary>
    public class CityCreateDto
    {
        /// <summary>
        /// unique identifier for the city
        /// </summary>
        public Guid CityGuid { get; set; } = Guid.NewGuid();

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

        /// <summary>
        /// created on date for the city
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
