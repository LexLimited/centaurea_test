using Microsoft.AspNetCore.Identity;

namespace CentaureaTest.Auth
{

    public sealed class ApplicationUser : IdentityUser
    {
        public string Username { get; set; }
        public Role Role { get; set; }

        public ApplicationUser(string username, Role role) : base(username)
        {
            Username = username;
            Role = role;
        }
    }

}