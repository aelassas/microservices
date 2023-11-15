using IdentityMicroservice.Model;
using IdentityMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using Middleware;

namespace IdentityMicroservice.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdentityController(IUserRepository userRepository, IJwtBuilder jwtBuilder, IEncryptor encryptor)
    : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] User user, [FromQuery(Name = "d")] string destination = "frontend")
    {
        var u = userRepository.GetUser(user.Email);

        if (u == null)
        {
            return NotFound("User not found.");
        }

        if (destination == "backend" && !u.IsAdmin)
        {
            return BadRequest("Could not authenticate user.");
        }

        var isValid = u.ValidatePassword(user.Password, encryptor);

        if (!isValid)
        {
            return BadRequest("Could not authenticate user.");
        }

        var token = jwtBuilder.GetToken(u.Id);

        return Ok(token);
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] User user)
    {
        var u = userRepository.GetUser(user.Email);

        if (u != null)
        {
            return BadRequest("User already exists.");
        }

        user.SetPassword(user.Password, encryptor);
        userRepository.InsertUser(user);

        return Ok();
    }

    [HttpGet("validate")]
    public IActionResult Validate([FromQuery(Name = "email")] string email, [FromQuery(Name = "token")] string token)
    {
        var u = userRepository.GetUser(email);

        if (u == null)
        {
            return NotFound("User not found.");
        }

        var userId = jwtBuilder.ValidateToken(token);

        if (userId != u.Id)
        {
            return BadRequest("Invalid token.");
        }

        return Ok(userId);
    }
}