using IdentityService.Interfaces;
using IdentityService.Models.Login;
using IdentityService.Models.Register;
using IdentityService.Models.Revoke;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.Login(request);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            await _authService.Register(request);
            return Ok("User registered successfully.");
        }

        [Authorize]
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RevokeRequest request)
        {
            await _authService.Revoke(request);
            return Ok("User refresh token revoked.");
        }

        [Authorize(Roles = "admin")]
        [HttpPost("revoke-all")]
        public async Task<IActionResult> RevokeAll()
        {
            await _authService.RevokeAll();
            return Ok("All refresh tokens revoked.");
        }

        [Authorize]
        [HttpPost("revoke-access-token")]
        public async Task<IActionResult> RevokeAccessToken([FromBody] RevokeAccessTokenRequest request)
        {
            await _authService.RevokeAccessToken(request);
            return Ok("Access token revoked.");
        }
    }
}
