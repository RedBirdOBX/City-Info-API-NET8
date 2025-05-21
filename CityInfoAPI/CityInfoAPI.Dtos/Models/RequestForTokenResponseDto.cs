namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// holds response for request for token
    /// </summary>
    public class RequestForTokenResponseDto
    {
        /// <summary>
        /// success flag
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// token string
        /// </summary>
        public string? Token { get; set; }
    }
}
