namespace Middleware;

public class JwtOptions
{
    public string Secret { get; set; }
    public int ExpiryMinutes { get; set; }
}