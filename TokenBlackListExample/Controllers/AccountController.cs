using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MasterNeverDown.LoginLimit.Filters;
using MasterNeverDown.LoginLimit.Middlewares;
using MasterNeverDown.TokenBlackList.Filters;
using Microsoft.AspNetCore.Authorization;

namespace TokenBlackListExample.Controllers
{
    /// <summary>
    /// Controller for handling account-related actions such as login and logout.
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="configuration">The configuration settings.</param>
        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticates the user and generates a JWT token if the credentials are valid.
        /// </summary>
        /// <param name="login">The login model containing username and password.</param>
        /// <returns>Returns an Ok result with the token if successful, otherwise Unauthorized.</returns>
        [HttpPost("login")]
        [Tags("Middleware Demo")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            if (IsValidUser(login))
            {
                var token = GenerateJwtToken();
                return Ok(new { token });
            }

            return Unauthorized();
        }

        /// <summary>
        /// Logs out the user by invalidating the provided JWT token.
        /// </summary>
        /// <param name="authorization">The authorization header containing the JWT token.</param>
        /// <returns>Returns Ok if successful, otherwise BadRequest.</returns>
        [Authorize]
        [HttpPost("logout")]
        [Tags("Middleware Demo")]
        public IActionResult Logout([FromHeader]string authorization)
        {
            var token = authorization.Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required");
            }

            return Ok("Logged out successfully");
        }

        /// <summary>
        /// Authenticates the user and generates a JWT token if the credentials are valid.
        /// This action uses a custom login limit filter.
        /// </summary>
        /// <param name="login">The login model containing username and password.</param>
        /// <returns>Returns an Ok result with the token if successful, otherwise Unauthorized.</returns>
        [AllowAnonymous]
        [TypeFilter(typeof(LoginLimitFilter))]
        [HttpPost("custom/login")]
        [Tags("Filter Demo")]
        public IActionResult Login2([FromBody] LoginModel login)
        {
            if (IsValidUser(login))
            {
                var token = GenerateJwtToken();
                return Ok(new { token });
            }

            return Unauthorized();
        }

        /// <summary>
        /// Logs out the user by invalidating the provided JWT token.
        /// This action uses a custom logout filter.
        /// </summary>
        /// <param name="authorization">The authorization header containing the JWT token.</param>
        /// <returns>Returns Ok if successful, otherwise BadRequest.</returns>
        [Authorize]
        [TypeFilter(typeof(LogoutFilter))]
        [HttpPost("custom/logout")]
        [Tags("Filter Demo")]
        public IActionResult Logout2([FromHeader]string authorization)
        {
            var token = authorization.Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required");
            }

            return Ok("Logged out successfully");
        }

        /// <summary>
        /// Validates the user credentials.
        /// </summary>
        /// <param name="login">The login model containing username and password.</param>
        /// <returns>Returns true if the credentials are valid, otherwise false.</returns>
        private bool IsValidUser(LoginModel login)
        {
            // Validate the user credentials
            return login.Username == "test" && login.Password == "password";
        }

        /// <summary>
        /// Generates a JWT token for the authenticated user.
        /// </summary>
        /// <returns>Returns the generated JWT token as a string.</returns>
        private string GenerateJwtToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "test"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    /// <summary>
    /// Model representing the login credentials.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <example>test</example>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <example>password</example>
        public string Password { get; set; }
    }
}