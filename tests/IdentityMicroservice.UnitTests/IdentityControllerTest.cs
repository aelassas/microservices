using IdentityMicroservice.Controllers;
using IdentityMicroservice.Model;
using IdentityMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Middleware;
using Moq;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace IdentityMicroservice.UnitTests;

public class IdentityControllerTest
{
    private readonly IdentityController _controller;
    private static readonly string AdminUserId = "653e4410614d711b7fc951a7";
    private static readonly string FrontendUserId = "653e4410614d711b7fc952a7";
    private static readonly User UnknownUser = new()
    {
        Id = "653e4410614d711b7fc957a7",
        Email = "unknown@store.com",
        Password = "4kg245EBBE+1IF20pKSBafiNhE/+WydWZo41cfThUqh7tz7+n7Yn9w==",
        Salt = "2lApH7EgXLHjYAvlmPIDAaQ5ypyXlH8PBVmOI+0zhMBu5HxZqIH7+w==",
        IsAdmin = false
    };
    private readonly List<User> _users = new()
    {
        new()
        {
            Id = AdminUserId,
            Email = "admin@store.com",
            Password = "Ukg255EBBE+1IF20pKSBafiNhE/+WydWZo41cfThUqh7tz7+n7Yn9w==",
            Salt = "4lApH7EgXLHjYAvlmPIDAaQ5ypyXlH8PBVmOI+0zhMBu5HxZqIH7+w==",
            IsAdmin = true
        },
        new()
        {
            Id = FrontendUserId,
            Email = "jdoe@store.com",
            Password = "Vhq8Klm83fCVILYhCzp2vKUJ/qSB+tmP/a9bD3leUnp1acBjS2I5jg==",
            Salt = "7+UwBowz/iv/sW7q+eYhJSfa6HiMQtJXyHuAShU+c1bUo6QUL4LIPA==",
            IsAdmin = false
        }
    };

    private static IConfiguration InitConfiguration()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
        return config;
    }

    public IdentityControllerTest()
    {
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>()))
            .Returns<string>(email => _users.FirstOrDefault(u => u.Email == email));
        mockRepo.Setup(repo => repo.InsertUser(It.IsAny<User>()))
            .Callback<User>(_users.Add);
        var configuration = InitConfiguration();
        var jwtSection = configuration.GetSection("jwt");
        var jwtOptions = Options.Create(new JwtOptions
        {
            Secret = jwtSection["secret"],
            ExpiryMinutes = int.Parse(jwtSection["expiryMinutes"] ?? "60")
        });
        _controller = new IdentityController(mockRepo.Object, new JwtBuilder(jwtOptions), new Encryptor());
    }

    [Fact]
    public void LoginTest()
    {
        // User not found
        var notFoundObjectResult = _controller.Login(UnknownUser);
        Assert.IsType<NotFoundObjectResult>(notFoundObjectResult);

        // Backend failure
        var user = new User
        {
            Id = FrontendUserId,
            Email = "jdoe@store.com",
            Password = "aaaaaa",
            IsAdmin = false
        };
        var badRequestObjectResult = _controller.Login(user, "backend");
        Assert.IsType<BadRequestObjectResult>(badRequestObjectResult);

        // Wrong password
        user.Password = "bbbbbb";
        badRequestObjectResult = _controller.Login(user);
        Assert.IsType<BadRequestObjectResult>(badRequestObjectResult);

        // Frontend success
        user.Password = "aaaaaa";
        var okObjectResult = _controller.Login(user);
        var okResult = Assert.IsType<OkObjectResult>(okObjectResult);
        var token = Assert.IsType<string>(okResult.Value);
        Assert.NotEmpty(token);

        // Backend success
        var adminUser = new User
        {
            Id = AdminUserId,
            Email = "admin@store.com",
            Password = "aaaaaa",
            IsAdmin = true
        };
        okObjectResult = _controller.Login(adminUser, "backend");
        okResult = Assert.IsType<OkObjectResult>(okObjectResult);
        token = Assert.IsType<string>(okResult.Value);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void RegisterTest()
    {
        // Failure (user already exists)
        var user = new User
        {
            Id = FrontendUserId,
            Email = "jdoe@store.com",
            Password = "aaaaaa",
            IsAdmin = false
        };
        var badRequestObjectResult = _controller.Register(user);
        Assert.IsType<BadRequestObjectResult>(badRequestObjectResult);

        // Success (new user)
        user = new User
        {
            Id = "145e4410614d711b7fc952a7",
            Email = "ctaylor@store.com",
            Password = "cccccc",
            IsAdmin = false
        };
        var okResult = _controller.Register(user);
        Assert.IsType<OkResult>(okResult);
        Assert.NotNull(_users.FirstOrDefault(u => u.Id == user.Id));
    }

    [Fact]
    public void ValidateTest()
    {
        // User not found
        var notFoundObjectResult = _controller.Validate(UnknownUser.Email, string.Empty);
        Assert.IsType<NotFoundObjectResult>(notFoundObjectResult);

        // Invalid token
        var badRequestObjectResult = _controller.Validate("jdoe@store.com", "zzz");
        Assert.IsType<BadRequestObjectResult>(badRequestObjectResult);

        // Success
        var user = new User
        {
            Id = FrontendUserId,
            Email = "jdoe@store.com",
            Password = "aaaaaa",
            IsAdmin = false
        };
        var okObjectResult = _controller.Login(user);
        var okResult = Assert.IsType<OkObjectResult>(okObjectResult);
        var token = Assert.IsType<string>(okResult.Value);
        Assert.NotEmpty(token);
        okObjectResult = _controller.Validate(user.Email, token);
        okResult = Assert.IsType<OkObjectResult>(okObjectResult);
        var userId = Assert.IsType<string>(okResult.Value);
        Assert.Equal(user.Id, userId);
    }
}