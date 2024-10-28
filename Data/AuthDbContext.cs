using System.Text;
using CentaureaTest.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CentaureaTest.Data
{

    public sealed class AuthDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public DbSet<IdentityRole> IdentityRole { get; set; }

        public AuthDbContext(DbContextOptions<AuthDbContext> options, ILogger<AuthDbContext> logger) : base(options) {}

        public static async Task SeedRoleAndSuperuser(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            IdentityResult roleResult;

            var roleNames = new List<string>
            {
                Role.User.ToString(),
                Role.Admin.ToString(),
                Role.Superuser.ToString(),
            };

            foreach (var role in roleNames)
            {
                var roleName = role.ToString();

                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // foreach (var role in (Role[]) Enum.GetValues(typeof(Role)))
            // {
            //     var roleName = role.ToString();

            //     if (!await roleManager.RoleExistsAsync(roleName))
            //     {
            //         roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            //     }
            // }

            var superUser = await userManager.FindByNameAsync(SuperUser.USERNAME);
            if (superUser is null)
            {
                superUser = SuperUser.ToApplciationUser();

                var createUserResult = await userManager.CreateAsync(superUser, SuperUser.PASSWORD);
                if (!createUserResult.Succeeded)
                {
                    var builder = new StringBuilder();
                    foreach (var error in createUserResult.Errors)
                    {
                        builder.AppendLine(error.Description);   
                    }

                    throw new Exception($"Failed to create a user:\r\n {builder}");
                }

                await userManager.AddToRoleAsync(superUser, Role.Superuser.ToString());
            }
        }
    }

}