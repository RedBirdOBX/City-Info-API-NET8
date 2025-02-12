namespace CityInfoAPI.Dtos.Models
{
    public class PointOfInterestDto
    {
        public int Id { get; set; }

        public System.Guid PointId { get; set; } = new Guid();

        public System.Guid CityId { get; set; } = new Guid();

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
