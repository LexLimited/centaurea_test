using CentaureaTest.Models.Auth;

namespace CentaureaTest.Models
{

    public sealed class AssignRole
    {
        public string Username { get; set; } = string.Empty;
        public string RoleName { get; set; } = CentaureaRoles.RoleName(Role.User);
    }

}