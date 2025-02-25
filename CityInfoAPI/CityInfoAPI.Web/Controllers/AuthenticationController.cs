using Azure.Core;
using CityInfoAPI.Web.Controllers.RequestHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CityInfoAPI.Web.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("Authenticate", Name = "Authenticate")]
        public ActionResult<string> Authenticate([FromBody] AuthenticationUserRequest userRequest)
        {
            // 1) validate the user
            var user = ValidateUserCredentials(userRequest.UserName, userRequest.Password);
            if (user == null)
            {
                return Unauthorized("User not authenticated");
            }

            // 2) create the token.  create key from secret.  secret only this api knows.
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Authentication:SecretForKey"] ?? string.Empty));

            // get the security key and create a signature to ensure the data wasn't tampered with.
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // claims to be used in token
            var claimsForToken = new List<Claim>();

            // custom policy - will generate claim. client must provide proper resources of "all".
            // industry standards:
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("given_name", user.FirstName));
            claimsForToken.Add(new Claim("family_name", user.LastName));
            claimsForToken.Add(new Claim("city", user.City));

            // here's our token. https://jwt.io/
            var jwtSecurityToken = new JwtSecurityToken(
                                                    _configuration["Authentication:Issuer"],
                                                    _configuration["Authentication:Audience"],
                                                    claimsForToken,
                                                    DateTime.UtcNow,
                                                    DateTime.UtcNow.AddMinutes(30),
                                                    signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }

        /// <summary>
        /// This typically might include a call to a database or a service to
        /// validate the user credentials.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private CityInfoUser ValidateUserCredentials(string username, string password)
        {
            // typically, this is where we look up the credentials passed in.
            // for now, we'll just return a hard-coded user and assume creds are valid.

            return new CityInfoUser(1, username, "John", "Doe", "New York");
        }
    }
}
