namespace Middleware;

public interface IJwtBuilder
{
    string GetToken(string userId);
    string ValidateToken(string token);
}