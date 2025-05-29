namespace CityInfoAPI.Dtos.RequestModels
{
    /// <summary>
    /// obj for mapping a user requesting to get token
    /// </summary>
    public class AuthenticationUserRequest
    {
        /// <summary>
        /// username
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// password
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
