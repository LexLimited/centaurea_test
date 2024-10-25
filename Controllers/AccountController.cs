using System.Net.Quic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CentaureaTest.Models
{

    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public sealed class AccountController : Controller
    {
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
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

            return RedirectToAction("Index", "Home");
        }
    }

}