using Microsoft.AspNetCore.Identity;

namespace ReverseProxyLoadBalance.Entities;

public class User : IdentityUser
{
    public User()
    {
        this.LastLogin = DateTime.UtcNow;
    }

    public DateTime? LastLogin { get; set; }
}
