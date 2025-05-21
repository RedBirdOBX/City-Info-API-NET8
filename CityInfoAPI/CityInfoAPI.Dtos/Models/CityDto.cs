namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// city object
    /// </summary>
    public class CityDto : LinkedResourcesDto
    {
        /// <summary>
        /// unique identifier for the city
        /// </summary>
        public Guid CityGuid { get; set; } = new Guid();

        /// <summary>
        /// name of city
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// description of city
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// created on date for city
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// count of points of interest for city
        /// </summary>
        public int NumberOfPointsOfInterest { get { return PointsOfInterest.Count;  }  }

        /// <summary>
        /// state
        /// </summary>
        public StateDto? State { get; set; }

        /// <summary>
        /// points of interest for city
        /// </summary>
        public ICollection<PointOfInterestDto> PointsOfInterest { get; set; } = new List<PointOfInterestDto>();
    }
}
