using Dynamics.Models.Models;
using Dynamics.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dynamics.DataAccess
{
    public class InitDb
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            try
            {
                // Because application db context is scoped, we need to make it scoped to
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                    // Setup roles
                    string[] roles = { RoleConstants.User, RoleConstants.Admin, RoleConstants.ProjectLeader,
                        RoleConstants.HeadOfOrganization, RoleConstants.Banned };

                    foreach (string role in roles)
                    {
                        var roleStore = new RoleStore<IdentityRole<Guid>, ApplicationDbContext, Guid>(context); // Create a role store that accepts GUID

                        if (!context.Roles.Any(r => r.Name == role))
                        {
                            roleStore.CreateAsync(new IdentityRole<Guid>(role)).GetAwaiter().GetResult();
                        }
                    }

                    // Seed a new user data
                    var user = new User
                    {
                        UserName = "admin",
                        Email = "admin@gmail.com",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString("D")
                    };

                    if (!context.Users.Any(u => u.UserName == user.UserName))
                    {
                        var result = await userManager.CreateAsync(user, "123123");
                        if (result.Succeeded)
                        {
                            result = await userManager.AddToRolesAsync(user, new[] { RoleConstants.Admin });
                        }
                    }
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
