using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Selu383.SP25.P02.Api.Features.Users;
using Selu383.SP25.P02.Api.Features.Roles;
using Selu383.SP25.P02.Api.Features.Theaters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Selu383.SP25.P02.Api.Data
{
    public static class SeedTheaters
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                await context.Database.MigrateAsync();

                // ✅ Seed Roles
                var roles = new[] { "Admin", "User" };
                foreach (var roleName in roles)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new Role { Name = roleName });
                    }
                }

                // ✅ Seed Users (After ensuring roles are created)
                await CreateUser(userManager, "galkadi", "Password123!", "Admin");
                await CreateUser(userManager, "bob", "Password123!", "User");
                await CreateUser(userManager, "sue", "Password123!", "User");

                // ✅ Seed Theaters (if not already seeded)
                if (!context.Theaters.Any())
                {
                    context.Theaters.AddRange(
                        new Theater
                        {
                            Name = "AMC Palace 10",
                            Address = "123 Main St, Springfield",
                            SeatCount = 150
                        },
                        new Theater
                        {
                            Name = "Regal Cinema",
                            Address = "456 Elm St, Shelbyville",
                            SeatCount = 200
                        },
                        new Theater
                        {
                            Name = "Grand Theater",
                            Address = "789 Broadway Ave, Metropolis",
                            SeatCount = 300
                        },
                        new Theater
                        {
                            Name = "Vintage Drive-In",
                            Address = "101 Retro Rd, Smallville",
                            SeatCount = 75
                        }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }

        

        private static async Task CreateUser(UserManager<User> userManager, string username, string password, string role)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                user = new User { UserName = username };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
