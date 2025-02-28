namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// City object without points of interest
    /// </summary>
    public class CityWithoutPointsOfInterestDto
    {
        /// <summary>
        ///  unique identifier for city
        /// </summary>
        public Guid CityGuid { get; set; } = new Guid();

        /// <summary>
        ///  name of city
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// description of city
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// created on date
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
