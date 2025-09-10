using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using dotnet.Contracts;
using dotnet.Services.Interfaces;

namespace dotnet.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [OpenApiTag("Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">Email, password, and optional display name.</param>
        /// <returns>JWT token and user info.</returns>
        [HttpPost("register")]
        [OpenApiOperation("Register", "Create a new user account and receive a JWT token.")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var resp = await _auth.RegisterAsync(request);
            return Ok(resp);
        }

        // PUBLIC_INTERFACE
        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="request">Email and password.</param>
        /// <returns>JWT token and user info.</returns>
        [HttpPost("login")]
        [OpenApiOperation("Login", "Authenticate an existing user and receive a JWT token.")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var resp = await _auth.LoginAsync(request);
            return Ok(resp);
        }
    }
}
