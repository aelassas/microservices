using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace Middleware;

public class JwtMiddleware : IMiddleware
{
    private readonly IJwtBuilder _jwtBuilder;

    public JwtMiddleware(IJwtBuilder jwtBuilder)
    {
        _jwtBuilder = jwtBuilder;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Get the token from the Authorization header
        var bearer = context.Request.Headers["Authorization"].ToString();
        var token = bearer.Replace("Bearer ", "");

        if (!token.IsNullOrEmpty())
        {
            // Verify the token using the IJwtBuilder
            var userId = _jwtBuilder.ValidateToken(token);

            if (ObjectId.TryParse(userId, out _))
            {
                // Store the userId in the HttpContext items for later use
                context.Items["userId"] = userId;
            }
            else
            {
                // If token or userId are invalid, send 401 Unauthorized status
                context.Response.StatusCode = 401;
            }
        }

        // Continue processing the request
        await next(context);
    }
}