using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfoAPI.Data.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = $"{nameof(PointGuid)} is required.")]
        [MaxLength(50, ErrorMessage = $"{nameof(PointGuid)} cannot exceed 50 characters.")]
        public Guid PointGuid { get; set; } = new Guid();

        [Required(ErrorMessage = $"{nameof(Name)} is required.")]
        [MaxLength(50, ErrorMessage = $"{nameof(Name)} cannot exceed 50 characters.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = $"{nameof(Description)} cannot exceed 200 characters.")]
        public string? Description { get; set; }

        public int CityId { get; set; }

        public Guid CityGuid { get; set; } = new Guid();

        [ForeignKey("CityId")]
        public City? City { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
