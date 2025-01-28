using Microsoft.AspNetCore.Identity;
using ReverseProxyLoadBalance.Entities;

namespace ReverseProxyLoadBalance.Implements;

public class BCryptPasswordHasher : IPasswordHasher<User>
{
    public String HashPassword(User user, String password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    public PasswordVerificationResult VerifyHashedPassword(User user, String hashedPassword, String providedPassword)
    {
        Boolean isValid = BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);

        if (isValid)
        {
            return PasswordVerificationResult.Success;
        }
        else
        {
            return PasswordVerificationResult.Failed;
        }
    }
}
