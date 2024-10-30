using Microsoft.AspNetCore.Identity;

namespace CentaureaTest.Models.Auth
{

    public sealed class ApplicationUser : IdentityUser
    {
        public Role Role { get; set; }
        public string? Password { get; set; }
    }

}