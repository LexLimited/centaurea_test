using System.Security.Claims;
using CentaureaTest.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CentaureaTest.Models
{

    [ApiController]
    [Route("auth")]
    [Authorize]
    public sealed class AuthenticationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthenticationController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] SignUp signUp, [FromQuery] string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser(signUp.Username, Role.User);
            var result = await _userManager.CreateAsync(user, signUp.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(err => err.Description));
            }
            else
            {
                return returnUrl is null ? Ok(user) : Ok(Redirect(returnUrl));
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn([FromBody] LogIn logIn, [FromQuery] string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(logIn.Username, logIn.Password, false, false);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return BadRequest("You're locked out");
                }

                if (result.IsNotAllowed)
                {
                    return BadRequest("You're not allowed to log in ???");
                }

                if (result.RequiresTwoFactor)
                {
                    return BadRequest("Login requires 2FA");
                }
            }

            return returnUrl is null ? Ok() : Ok(Redirect(returnUrl));
        }

        // [HttpPost("login")]
        // public async Task<IActionResult> LogIn(string username, string password, [FromQuery] string? returnUrl)
        // {
        //     // TODO! Very strange and clearly malfunctioning pice of code
        //     if (username == "admin" && password == "admin")
        //     {
        //         var claims = new List<Claim>()
        //         {
        //             new Claim(ClaimTypes.Name, username),
        //             new Claim("Role", "Admin"),
        //         };

        //         var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

        //         await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));
        //     }

        //     return BadRequest("Every request is bad");
        // }
    }

}