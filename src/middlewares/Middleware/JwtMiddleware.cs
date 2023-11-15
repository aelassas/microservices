using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace Middleware;

public class JwtMiddleware(IJwtBuilder jwtBuilder) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Get the token from the Authorization header
        var bearer = context.Request.Headers["Authorization"].ToString();
        var token = bearer.Replace("Bearer ", string.Empty);

        if (!string.IsNullOrEmpty(token))
        {
            // Verify the token using the IJwtBuilder
            var userId = jwtBuilder.ValidateToken(token);

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