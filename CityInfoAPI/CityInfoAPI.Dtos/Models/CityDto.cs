namespace CityInfoAPI.Dtos.Models
{
    public class CityDto
    {
        public int Id { get; set; }

        public Guid CityGuid { get; set; } = new Guid();

        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public int NumberOfPointsOfInterest { get { return PointsOfInterest.Count;  }  }

        public ICollection<PointOfInterestDto> PointsOfInterest { get; set; } = new List<PointOfInterestDto>();
    }
}
