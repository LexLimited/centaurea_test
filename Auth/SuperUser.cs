namespace CentaureaTest.Auth
{

    /// <summary>
    /// Totally safe class (not suspicious)
    /// </summary>
    public static class SuperUser
    {
        public static readonly string PASSWORD = "_Lex123";
        public static readonly string EMAIL = "pechalno@mylo.net";
        public static readonly string USERNAME = "lex";

        public static ApplicationUser ToApplciationUser()
        {
            return new ApplicationUser(USERNAME, PASSWORD, Role.Superuser);
        }
    }

}