using Microsoft.AspNetCore.Identity;

namespace CentaureaTest.Auth
{

    public sealed class ApplicationUser : IdentityUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }

        public ApplicationUser(string username, string password, Role role) : base(username)
        {
            Username = username;
            Password = password;
            Role = role;
        }
    }

}