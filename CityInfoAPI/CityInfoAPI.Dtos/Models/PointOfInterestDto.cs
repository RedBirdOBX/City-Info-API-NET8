namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// point of interest object
    /// </summary>
    public class PointOfInterestDto
    {
        /// <summary>
        /// unique identifier for point of interest
        /// </summary>
        public Guid PointGuid { get; set; } = new Guid();

        /// <summary>
        /// unique indentifier for city
        /// </summary>
        public Guid CityGuid { get; set; } = new Guid();

        /// <summary>
        /// name of point of interest
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// description of point of interest
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// created on date for point of interest
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
