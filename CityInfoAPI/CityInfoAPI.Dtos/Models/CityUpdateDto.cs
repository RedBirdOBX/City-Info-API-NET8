using System.ComponentModel.DataAnnotations;

namespace CityInfoAPI.Dtos.Models
{
    public class CityUpdateDto
    {
        [Required(ErrorMessage = $"{nameof(Name)} is required.")]
        [MaxLength(ErrorMessage = $"Max length for {nameof(Name)} is 50 chars.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(ErrorMessage = $"Max length for {nameof(Description)} is 500 chars.")]
        public string? Description { get; set; }
    }
}
