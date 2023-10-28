using System;

namespace Middleware;

public interface IJwtBuilder
{
    string GetToken(Guid userId);
    Guid ValidateToken(string token);
}