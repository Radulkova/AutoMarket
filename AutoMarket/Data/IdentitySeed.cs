using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AutoMarket.Data
{
    public static class IdentitySeed
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var logger = scope.ServiceProvider
                .GetService<ILoggerFactory>()
                ?.CreateLogger("IdentitySeed");

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "Client" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                    {
                        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                        logger?.LogError("Failed to create role {Role}: {Errors}", role, errors);
                    }
                }
            }

            await EnsureUserWithRole(userManager, logger, "admin@automarket.bg", "Admin123!", "Admin");
            // По желание тест клиент:
            // await EnsureUserWithRole(userManager, logger, "client@automarket.bg", "Client123!", "Client");
        }

        private static async Task EnsureUserWithRole(
            UserManager<IdentityUser> userManager,
            ILogger? logger,
            string email,
            string password,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    logger?.LogError("Failed to create user {Email}: {Errors}", email, errors);
                    return; // важно: не спираме приложението
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                var addRoleResult = await userManager.AddToRoleAsync(user, role);
                if (!addRoleResult.Succeeded)
                {
                    var errors = string.Join("; ", addRoleResult.Errors.Select(e => e.Description));
                    logger?.LogError("Failed to add role {Role} to {Email}: {Errors}", role, email, errors);
                }
            }
        }
    }
}

