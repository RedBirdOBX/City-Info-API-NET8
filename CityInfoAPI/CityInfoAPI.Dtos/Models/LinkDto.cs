// Ignore Spelling: Href

using System.Runtime.Serialization;

namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// link type to be included in responses
    /// </summary>
    [DataContractAttribute]
    public class LinkDto
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public LinkDto()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="href">uri to return</param>
        /// <param name="rel">responsibility of uri</param>
        /// <param name="method">type of request</param>
        public LinkDto(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }

        /// <summary>
        /// the uri of the resource
        /// </summary>
        [DataMemberAttribute]
        public string Href { get; set; }

        /// <summary>
        /// the responsibility of the uri
        /// </summary>
        [DataMemberAttribute]
        public string Rel { get; set; }

        /// <summary>
        /// the type of request to be made
        /// </summary>
        [DataMemberAttribute]
        public string Method { get; set; }
    }
}
