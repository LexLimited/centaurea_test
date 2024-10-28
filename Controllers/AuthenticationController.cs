using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentaureaTest.Models
{

    [ApiController]
    [Route("auth")]
    [Authorize]
    public sealed class AuthenticationController : Controller
    {
        [HttpGet("signup")]
        public IActionResult SignUp([FromBody] string? returnUrl)
        {
            return BadRequest("Every request is bad");
        }

        [HttpGet("login")]
        public async Task<IActionResult> LogIn([FromBody] string? returnUrl)
        {
            await Task.Delay(500);
            return BadRequest("Every request is bad");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn(string username, string password, [FromQuery] string? returnUrl)
        {
            // TODO! Very strange and clearly malfunctioning pice of code
            if (username == "admin" && password == "admin")
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim("Role", "Admin"),
                };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));
            }

            return BadRequest("Every request is bad");
        }
    }

}