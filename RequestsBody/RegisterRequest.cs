namespace ReverseProxyLoadBalance.RequestsBody;

public class RegisterRequest
{
    public required String Email { get; set; }
    public required String Password { get; set; }
}
