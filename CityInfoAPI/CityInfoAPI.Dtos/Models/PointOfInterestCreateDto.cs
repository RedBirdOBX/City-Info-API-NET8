using System.ComponentModel.DataAnnotations;

namespace CityInfoAPI.Dtos.Models
{
    public class PointOfInterestCreateDto
    {
        public int PointId { get; set; }

        public Guid PointGuid { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = $"{nameof(CityId)} is required.")]
        public int CityId { get; set; }

        [Required(ErrorMessage = $"{nameof(CityGuid)} is required.")]
        public Guid CityGuid { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = $"{nameof(Name)} is required.")]
        [MaxLength(ErrorMessage = $"Max length for {nameof(Name)} is 50 chars.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(ErrorMessage = $"Max length for {nameof(Description)} is 500 chars.")]
        public string? Description { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
