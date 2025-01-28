using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ReverseProxyLoadBalance.Constants;
using ReverseProxyLoadBalance.Entities;
using ReverseProxyLoadBalance.Repositories;
using ReverseProxyLoadBalance.RequestsBody;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReverseProxyLoadBalance.Controllers;

[Route("auth/user")]
[ApiController]
public class UserController(IUserRepository repository, IConfiguration configuration, ILogger<UserController> logger) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = RoleConstants.Admin)]
    public IActionResult Index()
    {
        return Ok($"Hello World {User.Identity?.Name}!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellation = default)
    {
        Byte[] secretJwtToken = Encoding.ASCII.GetBytes(configuration.GetSection("JwtToken:Secret").Value ?? String.Empty);

        User? user = await repository.GetUserByEmailAsync(request.Email, cancellation);

        if (user == null
            || String.IsNullOrEmpty(user.Email)
            || String.IsNullOrEmpty(user.UserName)
            || String.IsNullOrEmpty(user.PasswordHash)) return StatusCode(StatusCodes.Status401Unauthorized);

        try
        {
            Boolean passwordMatch = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!passwordMatch) return StatusCode(StatusCodes.Status401Unauthorized);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao validar usuário");
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        List<Claim> authClaims = [
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        ];

        ICollection<String> roles = await repository.GetRolesAsync(user, cancellation);
        foreach (String role in roles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(authClaims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretJwtToken), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        await repository.UpdateLastLoginAsync(user);

        return Ok(new
        {
            Token = tokenHandler.WriteToken(token),
            Expires = DateTime.UtcNow.AddHours(2),
        });
    }
}
