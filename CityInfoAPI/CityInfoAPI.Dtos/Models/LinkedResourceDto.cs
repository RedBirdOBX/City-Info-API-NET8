using Newtonsoft.Json;

namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// links to add to response objs
    /// </summary>
    public class LinkedResourcesDto
    {
        /// <summary>
        /// links to be added to resource dtos
        /// </summary>
        [JsonProperty(PropertyName = "_links")]
        public List<LinkDto> Links { get; set; }

        /// <summary>
        ///  constructor
        /// </summary>
        public LinkedResourcesDto()
        {
            Links = new List<LinkDto>();
        }
    }
}
