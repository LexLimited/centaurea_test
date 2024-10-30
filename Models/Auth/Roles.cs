namespace CentaureaTest.Models.Auth
{

    public enum Role
    {
        User,
        Admin,
        Superuser,
    }

    public static class CentaureaRoles
    {
        public static Role Roles { get; }
        public static string[] RoleNames { get => new string[]{ "User", "Admin", "Superuser" }; }
        
        public static string RoleName(Role role)
        {
            switch (role)
            {
                case Role.User: return "User";
                case Role.Admin: return "Admin";
                case Role.Superuser: return "Superuser";
            }

            throw new NotImplementedException($"RoleName for {role} is not implemented");
        }

        public static bool RoleNameExists(string roleName)
        {
            return RoleNames.Contains(roleName);
        }
    }

}