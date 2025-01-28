namespace ReverseProxyLoadBalance.RequestsBody;

public class LoginRequest
{
    public required String Email { get; set; }
    public required String Password { get; set; }
}
