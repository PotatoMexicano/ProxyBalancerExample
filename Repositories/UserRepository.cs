using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReverseProxyLoadBalance.Constants;
using ReverseProxyLoadBalance.Context;
using ReverseProxyLoadBalance.Entities;
using ReverseProxyLoadBalance.RequestsBody;
using System.Text.RegularExpressions;

namespace ReverseProxyLoadBalance.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserAsync(Int32 idUser, CancellationToken cancellation);
    Task<User?> GetUserByEmailAsync(String email, CancellationToken cancellation);
    Task<ICollection<User>> GetUsersAsync(CancellationToken cancellation);

    Task<ICollection<String>> GetRolesAsync(User user, CancellationToken cancellation);
    Task<Boolean> UpdateLastLoginAsync(User user);

    Task<Boolean> RegisterAsync(RegisterRequest requestUser, CancellationToken cancellation);
}

public class UserRepository(ApplicationDbContext context, UserManager<User> userManager, ILogger<UserRepository> logger) : IUserRepository
{
    public async Task<ICollection<String>> GetRolesAsync(User user, CancellationToken cancellation)
    {
        ICollection<String> roles = await userManager.GetRolesAsync(user);

        return roles;
    }

    public async Task<User?> GetUserAsync(Int32 idUser, CancellationToken cancellation)
    {
        return await userManager.FindByIdAsync(idUser.ToString());
    }

    public async Task<User?> GetUserByEmailAsync(String email, CancellationToken cancellation)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public async Task<ICollection<User>> GetUsersAsync(CancellationToken cancellation)
    {
        return await context.Users.Select(x => new User
        {
            Id = x.Id,
            Email = x.Email,
            LastLogin = x.LastLogin,
            UserName = x.UserName,
        }).ToArrayAsync(cancellation);
    }

    public async Task<Boolean> RegisterAsync(RegisterRequest requestUser, CancellationToken cancellation)
    {
        try
        {
            if (String.IsNullOrEmpty(requestUser.Email)) return false;

            Match username = Regex.Match(requestUser.Email, @"^[^@]+");

            User? user = new User
            {
                Email = requestUser.Email,
                UserName = username.Success ? username.Value : requestUser.Email,
            };

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash, 12);

            IdentityResult result = await userManager.CreateAsync(user, requestUser.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, RoleConstants.Undefined);
                return true;
            }

            return false;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao salvar usuário");
            return false;
        }
    }

    public async Task<Boolean> UpdateLastLoginAsync(User user)
    {
        user.LastLogin = DateTime.UtcNow;
        IdentityResult result = await userManager.UpdateAsync(user);
        return result.Succeeded;
    }
}
