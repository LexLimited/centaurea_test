using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CentaureaTest.Models.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CentaureaTest.Models
{

    [ApiController]
    [Route("auth")]
    public sealed class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController
        (
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AuthController> logger
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] SignUp signUp, [FromQuery] string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _userManager.FindByNameAsync(signUp.Username) is not null)
            {
                return BadRequest($"User with email {signUp.Username} already exists");
            }

            var user = new ApplicationUser
            {
                UserName = signUp.Username,
                Password = signUp.Password,
                Role = Role.User,   
            };

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
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(logIn.Username);
            if (user is null)
            {
                return BadRequest($"User {logIn.Username} does not exist");
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

                return BadRequest("Username or password is incorrect");
            }

            return Ok(user);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("logged out");
        }

        [HttpPost("create_role")]
        public async Task<IActionResult> CreateRole([FromBody, Required] string roleName)
        {
            if (!CentaureaRoles.RoleNameExists(roleName))
            {
                return BadRequest($"{roleName} is not a valid role name");
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                return Conflict($"Role {roleName} already exists");
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                return Ok($"Created role {roleName}");
            }

            return BadRequest(new
            {
                Message = $"Failed to create role {roleName}",
                result.Errors,    
            });
        }

        [HttpPost("assign_role")]
        [AllowAnonymous]
        // [Authorize(Roles = "Superuser")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRole assignRole)
        {
            var user = await _userManager.FindByNameAsync(assignRole.Username);
            if (user is null)
            {
                return NotFound($"User {assignRole.Username} not found");
            }

            var result = await _userManager.AddToRoleAsync(user, assignRole.RoleName);
            if (result.Succeeded)
            {
                return Ok($"User {user.UserName} assigned to role {assignRole.RoleName}");
            }

            return BadRequest(new
            {
                Message = $"Failed to assign {user.UserName} to role {assignRole.RoleName}",
                result.Errors,
            });
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin, Superuser")]
        public async Task<IActionResult> GetUsers()
        {
            var userRole = await _roleManager.FindByNameAsync("User");
            if (userRole is null || userRole.Name is null)
            {
                return Problem("Role 'User' not found");
            }

            var userUsers = await _userManager.GetUsersInRoleAsync(userRole.Name);
            return Ok(userUsers);
        }

        [HttpGet("whoami")]
        [AllowAnonymous]
        public IActionResult WhoAmI()
        {
            if (User.Identity is null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized("User is not authenticated");
            }

            var username = User.Identity.Name;
            var roles = User.Claims
                .Where(claim => claim.Type == ClaimTypes.Role)
                .Select(claim => claim.Value)
                .ToList();

            return Ok(new
            {
                Username = username,
                Roles = roles,
                IsPrivileged = roles.Where(role => role == "Admin" || role == "Superuser").Any(),
            });
        }
    }

}