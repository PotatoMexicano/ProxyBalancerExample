using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReverseProxyLoadBalance.Constants;
using ReverseProxyLoadBalance.Entities;

namespace ReverseProxyLoadBalance.Context;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions options) : base(options) { }
}

public static class MappingEntities
{
    public static async void SeedDatabase(this WebApplication app)
    {
        IServiceScope scope = app.Services.CreateScope();

        RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        String[] rolesName = { RoleConstants.Admin, RoleConstants.Employee, RoleConstants.Client, RoleConstants.Undefined };
        foreach (String role in rolesName)
        {
            Boolean exist = await roleManager.RoleExistsAsync(role);
            if (!exist) await roleManager.CreateAsync(new IdentityRole(role));
        }

        User[] adminUsers = new[]
        {
            new User
            {
                Email = "admin@email.com",
                UserName = "admin",
                LastLogin = DateTime.UtcNow,
            }
        };

        User[] defaultUsers = new[]
        {
            new User
            {
                Email = "gabriel@email.com",
                UserName = "gabriel",
                LastLogin = DateTime.UtcNow,
            }
        };

        foreach (User admin in adminUsers)
        {
            if (String.IsNullOrEmpty(admin.Email) || String.IsNullOrEmpty(admin.UserName)) return;

            User? adminEntity = await userManager.FindByEmailAsync(admin.Email);

            if (adminEntity == null)
            {
                IdentityResult result = await userManager.CreateAsync(admin, $"Passw0rd-{admin.UserName}");
                if (result.Succeeded) adminEntity = await userManager.FindByEmailAsync(admin.Email);
            }

            if (adminEntity == null) return;

            if (!await userManager.IsInRoleAsync(adminEntity, RoleConstants.Admin))
            {
                await userManager.AddToRoleAsync(adminEntity, RoleConstants.Admin);
            }
        }

        foreach (User user in defaultUsers)
        {
            if (String.IsNullOrEmpty(user.Email) || String.IsNullOrEmpty(user.UserName)) return;

            User? userEntity = await userManager.FindByEmailAsync(user.Email);

            if (userEntity == null)
            {
                IdentityResult result = await userManager.CreateAsync(user, $"Passw0rd-{user.UserName}");
                if (result.Succeeded) userEntity = await userManager.FindByEmailAsync(user.Email);
            }

            if (userEntity == null) return;

            if (!await userManager.IsInRoleAsync(userEntity, RoleConstants.Employee))
            {
                await userManager.AddToRoleAsync(userEntity, RoleConstants.Employee);
            }
        }
    }
}
