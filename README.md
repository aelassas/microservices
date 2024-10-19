[![Build](https://github.com/aelassas/microservices/actions/workflows/build.yml/badge.svg)](https://github.com/aelassas/microservices/actions/workflows/build.yml) [![Test](https://github.com/aelassas/microservices/actions/workflows/test.yml/badge.svg)](https://github.com/aelassas/microservices/actions/workflows/test.yml)

## Contents

1.  [Introduction](#introduction)
2.  [Best Practices](#best-practises)
3.  [Development Environment](#dev-env)
4.  [Prerequisites](#prerequisites)
5.  [Architecture](#architecture)
6.  [Source Code](#src)
7.  [Microservices](#microservices)
    1.  [Catalog Microservice](#catalog-microservice)
    2.  [Identity Microservice](#identity-microservice)
    3.  [Adding JWT to Catalog Microservice](#catalog-microservice-jwt)
    4.  [Cart Microservice](#cart-microservice)
8.  [API Gateways](#gateways)
9.  [Client Apps](#apps)
10.  [Unit Tests](#unit-test)
11.  [Monitoring using Health Checks](#health-checks)
12.  [How to Run the Application](#run-app)
13.  [How to Deploy the Application](#deploy-app)
14.  [References](#references)
15.  [History](#history)

## <a id="introduction" name="introduction">Introduction</a>

Following the recent release of .NET 8, I have taken it upon myself to write a comprehensive guide on building microservices using ASP.NET Core 8\. This latest version of .NET is a significant release that offers a number of new features and enhancements, as well as performance improvements and long-term support. It builds on the performance enhancements introduced in .NET 7 by further optimizing the Just-In-Time (JIT) compiler, garbage collector, and runtime. The result is faster startup times, better overall application performance, and reduced memory usage. You can find out more about all the new features and enhancements [here](https://devblogs.microsoft.com/dotnet/announcing-dotnet-8/). Now, let's focus on microservices.

![Image](https://github.com/aelassas/microservices/blob/main/img/microservices-logical.png?raw=true)

A microservices architecture consists of a collection of small, independent, and loosely coupled services. Each service is self-contained, implements a single business capability, is responsible for persisting its own data, is a separate codebase, and can be deployed independently.

API gateways are entry points for clients. Instead of calling services directly, clients call the API gateway, which forwards the call to the appropriate services.

There are multiple advantages using microservices architecture:

*   Developers can better understand the functionality of a service.
*   Failure in one service does not impact other services.
*   It's easier to manage bug fixes and feature releases.
*   Services can be deployed in multiple servers to enhance performance.
*   Services are easy to change and test.
*   Services are easy and fast to deploy.
*   Allows to choose technology that is suited for a particular functionality.

Before choosing microservices architecture, here are some challenges to consider:

*   Services are simple but the entire system as a whole is more complex.
*   Communication between services can be complex.
*   More services equals more resources.
*   Global testing can be difficult.
*   Debugging can be harder.

Microservices architecture is great for large companies, but can be complicated for small companies who need to create and iterate quickly, and don't want to get into complex orchestration.

This article provides a comprehensive guide on building microservices using ASP.NET Core, constructing API gateways using Ocelot, establishing repositories using MongoDB, managing JWT in microservices, unit testing microservices using xUnit and Moq, monitoring microservices using health checks, and finally deploying microservices using Docker.

## <a id="best-practises" name="best-practises">Best Practices</a>

Here's a breakdown of some best practices:

*   **Single Responsibility**: Each microservice should have a single responsibility or purpose. This means that it should do one thing and do it well. This makes it easier to understand, develop, test, and maintain each microservice.
*   **Separate Data Store**: Microservices should ideally have their own data storage. This can be a separate database, which is isolated from other microservices. This isolation ensures that changes or issues in one microservice's data won't affect others.
*   **Asynchronous Communication**: Use asynchronous communication patterns, like message queues or publish-subscribe systems, to enable communication. This makes the system more resilient and decouples services from each other.
*   **Containerization**: Use containerization technologies like Docker to package and deploy microservices. Containers provide a consistent and isolated environment for your microservices, making it easier to manage and scale them.
*   **Orchestration**: Use container orchestration tools like Kubernetes to manage and scale your containers. Kubernetes provides features for load balancing, scaling, and monitoring, making it a great choice for orchestrating microservices.
*   **Build and Deploy Separation**: Keep the build and deployment processes separate. This means that the build process should result in a deployable artifact, like a Docker container image, which can then be deployed in different environments without modification.
*   **Stateless**: Microservices should be stateless as much as possible. Any necessary state should be stored in the database or an external data store. Stateless services are easier to scale and maintain.
*   **Micro Frontends**: If you're building a web application, consider using the micro frontends approach. This involves breaking down the user interface into smaller, independently deployable components that can be developed and maintained by separate teams.

## <a id="dev-env" name="dev-env">Development Environment</a>

*   Visual Studio 2022 >= 17.8.0
*   .NET 8.0
*   MongoDB
*   Postman

## <a id="prerequisites" name="prerequisites">Prerequisites</a>

*   C#
*   ASP.NET Core
*   Ocelot
*   Swashbuckle
*   Serilog
*   JWT
*   MongoDB
*   xUnit
*   Moq

## <a id="architecture" name="architecture">Architecture</a>

![Image](https://github.com/aelassas/microservices/blob/main/img/architecture.jpg?raw=true)

There are three microservices:

*   **Catalog microservice**: allows to manage the catalog.
*   **Cart microservice**: allows to manage the cart.
*   **Identity microservice**: allows to manage authentication and users.

Each microservice implements a single business capability and has its own dedicated database. This is called database-per-service pattern. This pattern allows for better separation of concerns, data isolation, and scalability. In a microservices architecture, services are designed to be small, focused, and independent, each responsible for a specific functionality. To maintain this separation, it's essential to ensure that each microservice manages its data independently. Here are other pros of this pattern:

*   Data schema can be modified without impacting other microservices.
*   Each microservice has its own data store, preventing accidental or unauthorized access to another service's data.
*   Since each microservice and its database are separate, they can be scaled independently based on their specific needs.
*   Each microservice can choose the database technology that best suits its requirements, without being bound to a single, monolithic database.
*   If one of the database server is down, this will not affect to other services.

There are two API gateways, one for the frontend and one for the backend.

Below is the frontend API gateway:

*   **GET /catalog**: retrieves catalog items.
*   **GET /catalog/{id}**: retrieves a catalog item.
*   **GET /cart**: retrieves cart items.
*   **POST /cart**: adds a cart item.
*   **PUT /cart**: updates a cart item.
*   **DELETE /cart**: deletes a cart item.
*   **POST /identity/login**: performs a login.
*   **POST /identity/register**: registers a user.
*   **GET /identity/validate**: validates a JWT token.

Below is the backend API gateway:

*   **GET /catalog**: retrieves catalog items.
*   **GET /catalog/{id}**: retrieves a catalog item.
*   **POST /catalog**: creates a catalog item.
*   **PUT /catalog**: updates a catalog item.
*   **DELETE /catalog/{id}**: deletes a catalog item.
*   **PUT /cart/update-catalog-item**: updates a catalog item in carts.
*   **DELETE /cart/delete-catalog-item**: deletes catalog item references from carts.
*   **POST /identity/login**: performs a login.
*   **GET /identity/validate**: validates a JWT token.

Finally, there are two client apps. A frontend for accessing the store and a backend for managing the store.

The frontend allows registered users to see the available catalog items, allows to add catalog items to the cart, and remove catalog items from the cart.

Here is a screenshot of the store page in the frontend:

![Image](https://github.com/aelassas/microservices/blob/main/img/store_frontend.jpg?raw=true)

The backend allows admin users to see the available catalog items, allows to add new catalog items, update catalog items, and remove catalog items.

Here is a screenshot of the store page in the backend:

![Image](https://github.com/aelassas/microservices/blob/main/img/store_backend.jpg?raw=true)

## <a id="src" name="src">Source Code</a>

![Image](https://github.com/aelassas/microservices/blob/main/img/store-solution2.png?raw=true)

*   `CatalogMicroservice` project contains the source code of the microservice managing the catalog.
*   `CartMicroservice` project contains the source code of the microservice managing the cart.
*   `IdentityMicroservice` project contains the source code of the microservice managing authentication and users.
*   `Middleware` project contains the source code of common functionalities used by microservices.
*   `FrontendGateway` project contains the source code of the frontend API gateway.
*   `BackendGateway` project contains the source code of the backend API gateway.
*   `Frontend` project contains the source code of the frontend client app.
*   `Backend` project contains the source code of the backend client app.
*   `test` solution folder contains unit tests of all microservices.

Microservices and gateways are developed using ASP.NET Core and C#. Client apps are developed using HTML and Vanilla JavaScript in order to focus on microservices.

## <a id="microservices" name="microservices">Microservices</a>

### <a id="catalog-microservice" name="catalog-microservice">Catalog Microservice</a>

Let's start with the simplest microservice, `CatalogMicroservice`.

`CatalogMicroservice` is responsible for managing the catalog.

Below is the model used by `CatalogMicroservice`:

<pre lang="cs">public class CatalogItem
{
    public static readonly string DocumentName = nameof(CatalogItem);

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; init; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
}</pre>

Below is the repository interface:

<pre lang="cs">public interface ICatalogRepository
{
    IList<CatalogItem> GetCatalogItems();
    CatalogItem? GetCatalogItem(string catalogItemId);
    void InsertCatalogItem(CatalogItem catalogItem);
    void UpdateCatalogItem(CatalogItem catalogItem);
    void DeleteCatalogItem(string catagItemId);
}</pre>

Below is the repository:

<pre lang="cs">public class CatalogRepository(IMongoDatabase db) : ICatalogRepository
{
    private readonly IMongoCollection<CatalogItem> _col = 
                     db.GetCollection<CatalogItem>(CatalogItem.DocumentName);

    public IList<CatalogItem> GetCatalogItems() =>
        _col.Find(FilterDefinition<CatalogItem>.Empty).ToList();

    public CatalogItem GetCatalogItem(string catalogItemId) =>
        _col.Find(c => c.Id == catalogItemId).FirstOrDefault();

    public void InsertCatalogItem(CatalogItem catalogItem) =>
        _col.InsertOne(catalogItem);

    public void UpdateCatalogItem(CatalogItem catalogItem) =>
        _col.UpdateOne(c => c.Id == catalogItem.Id, Builders<CatalogItem>.Update
            .Set(c => c.Name, catalogItem.Name)
            .Set(c => c.Description, catalogItem.Description)
            .Set(c => c.Price, catalogItem.Price));

    public void DeleteCatalogItem(string catalogItemId) =>
        _col.DeleteOne(c => c.Id == catalogItemId);
}</pre>

Below is the controller:

<pre lang="cs">[Route("api/[controller]")]
[ApiController]
public class CatalogController(ICatalogRepository catalogRepository) : ControllerBase
{
    // GET: api/<CatalogController>
    [HttpGet]
    public IActionResult Get()
    {
        var catalogItems = catalogRepository.GetCatalogItems();
        return Ok(catalogItems);
    }

    // GET api/<CatalogController>/653e4410614d711b7fc953a7
    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var catalogItem = catalogRepository.GetCatalogItem(id);
        return Ok(catalogItem);
    }

    // POST api/<CatalogController>
    [HttpPost]
    public IActionResult Post([FromBody] CatalogItem catalogItem)
    {
        catalogRepository.InsertCatalogItem(catalogItem);
        return CreatedAtAction(nameof(Get), new { id = catalogItem.Id }, catalogItem);
    }

    // PUT api/<CatalogController>
    [HttpPut]
    public IActionResult Put([FromBody] CatalogItem? catalogItem)
    {
        if (catalogItem != null)
        {
            catalogRepository.UpdateCatalogItem(catalogItem);
            return Ok();
        }
        return new NoContentResult();
    }

    // DELETE api/<CatalogController>/653e4410614d711b7fc953a7
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        catalogRepository.DeleteCatalogItem(id);
        return Ok();
    }
}</pre>

`ICatalogRepository` is added using dependency injection in _Startup.cs_ in order to make the microservice testable:

<pre lang="cs">// This method gets called by the runtime. 
// Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddMongoDb(Configuration);
    services.AddSingleton<ICatalogRepository>(sp =>
        new CatalogRepository(sp.GetService<IMongoDatabase>() ?? 
            throw new Exception("IMongoDatabase not found"))
    );
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog", Version = "v1" });
    });

    // ...
}</pre>

Below is `AddMongoDB` extension method:

<pre lang="cs">public static void AddMongoDb
  (this IServiceCollection services, IConfiguration configuration)
{
    services.Configure<MongoOptions>(configuration.GetSection("mongo"));
    services.AddSingleton(c =>
    {
        var options = c.GetService<IOptions<MongoOptions>>();

        return new MongoClient(options.Value.ConnectionString);
    });
    services.AddSingleton(c =>
    {
        var options = c.GetService<IOptions<MongoOptions>>();
        var client = c.GetService<MongoClient>();

        return client.GetDatabase(options.Value.Database);
    });
}</pre>

Below is `Configure` method in _Startup.cs_:

<pre lang="cs">// This method gets called by the runtime. 
// Use this method to configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog V1");
    });

    var option = new RewriteOptions();
    option.AddRedirect("^$", "swagger");
    app.UseRewriter(option);

    // ...

    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}</pre>

Below is _appsettings.json_:

<pre lang="json">{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "mongo": {
    "connectionString": "mongodb://127.0.0.1:27017",
    "database": "store-catalog"
  }
}
</pre>

API documentation is generated using Swashbuckle. Swagger middleware is configured in _Startup.cs_, in `ConfigureServices` and `Configure` methods in _Startup.cs._

If you run `CatalogMicroservice` project using IISExpress or Docker, you will get the Swagger UI when accessing [http://localhost:44326/](http://localhost:44326/):

![Image](https://github.com/aelassas/microservices/blob/main/img/swagger_ui.jpg?raw=true)

### <a id="identity-microservice" name="identity-microservice">Identity Microservice</a>

Now, let's move on to `IdentityMicroservice`.

`IdentityMicroservice` is responsible for authentication and managing users.

Below is the model used by `IdentityMicroservice`:

<pre lang="cs">public class User
{
    public static readonly string DocumentName = nameof(User);

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; init; }
    public required string Email { get; init; }
    public required string Password { get; set; }
    public string? Salt { get; set; }
    public bool IsAdmin { get; init; }

    public void SetPassword(string password, IEncryptor encryptor)
    {
        Salt = encryptor.GetSalt();
        Password = encryptor.GetHash(password, Salt);
    }

    public bool ValidatePassword(string password, IEncryptor encryptor) =>
        Password == encryptor.GetHash(password, Salt);
}</pre>

`IEncryptor` middleware is used for encrypting passwords.

Below is the repository interface:

<pre lang="cs">public interface IUserRepository
{
    User? GetUser(string email);
    void InsertUser(User user);
}</pre>

Below is the repository implementation:

<pre lang="cs">public class UserRepository(IMongoDatabase db) : IUserRepository
{
    private readonly IMongoCollection<User> _col = 
                     db.GetCollection<User>(User.DocumentName);

    public User? GetUser(string email) =>
        _col.Find(u => u.Email == email).FirstOrDefault();

    public void InsertUser(User user) =>
        _col.InsertOne(user);
}</pre>

Below is the controller:

<pre lang="cs">[Route("api/[controller]")]
[ApiController]
public class IdentityController
  (IUserRepository userRepository, IJwtBuilder jwtBuilder, IEncryptor encryptor)
    : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] User user, 
           [FromQuery(Name = "d")] string destination = "frontend")
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
    public IActionResult Validate([FromQuery(Name = "email")] string email, 
                                  [FromQuery(Name = "token")] string token)
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
}</pre>

`IUserRepository`, `IJwtBuilder` and `IEncryptor` middlewares are added using dependency injection in _Startup.cs_:

<pre lang="cs">// This method gets called by the runtime. 
// Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddMongoDb(Configuration);
    services.AddJwt(Configuration);
    services.AddTransient<IEncryptor, Encryptor>();
    services.AddSingleton<IUserRepository>(sp =>
        new UserRepository(sp.GetService<IMongoDatabase>() ?? 
            throw new Exception("IMongoDatabase not found"))
    );
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "User", Version = "v1" });
    });

    // ...
}</pre>

Below is `AddJwt` extension method:

<pre lang="cs">public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
{
    var options = new JwtOptions();
    var section = configuration.GetSection("jwt");
    section.Bind(options);
    services.Configure<JwtOptions>(section);
    services.AddSingleton<IJwtBuilder, JwtBuilder>();
    services.AddAuthentication()
        .AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey
                                   (Encoding.UTF8.GetBytes(options.Secret))
            };
        });
}</pre>

`IJwtBuilder` is responsible for creating JWT tokens and validating them:

<pre lang="cs">public interface IJwtBuilder
{
    string GetToken(string userId);
    string ValidateToken(string token);
}</pre>

Below is the implementation of `IJwtBuilder`:

<pre lang="cs">public class JwtBuilder(IOptions<JwtOptions> options) : IJwtBuilder
{
    private readonly JwtOptions _options = options.Value;

    public string GetToken(string userId)
    {
        var signingKey = new SymmetricSecurityKey
                         (Encoding.UTF8.GetBytes(_options.Secret));
        var signingCredentials = new SigningCredentials
                                 (signingKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim("userId", userId)
        };
        var expirationDate = DateTime.Now.AddMinutes(_options.ExpiryMinutes);
        var jwt = new JwtSecurityToken(claims: claims, 
                  signingCredentials: signingCredentials, expires: expirationDate);
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return encodedJwt;
    }

    public string ValidateToken(string token)
    {
        var principal = GetPrincipal(token);
        if (principal == null)
        {
            return string.Empty;
        }

        ClaimsIdentity identity;
        try
        {
            identity = (ClaimsIdentity)principal.Identity;
        }
        catch (NullReferenceException)
        {
            return string.Empty;
        }
        var userIdClaim = identity?.FindFirst("userId");
        if (userIdClaim == null)
        {
            return string.Empty;
        }
        var userId = userIdClaim.Value;
        return userId;
    }

    private ClaimsPrincipal GetPrincipal(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            if (jwtToken == null)
            {
                return null;
            }
            var key = Encoding.UTF8.GetBytes(_options.Secret);
            var parameters = new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            IdentityModelEventSource.ShowPII = true;
            ClaimsPrincipal principal = 
                  tokenHandler.ValidateToken(token, parameters, out _);
            return principal;
        }
        catch (Exception)
        {
            return null;
        }
    }
}</pre>

`IEncryptor` is simply responsible for encrypting passwords:

<pre lang="cs">public interface IEncryptor
{
    string GetSalt();
    string GetHash(string value, string salt);
}</pre>

Below is the implementation of `IEncryptor`:

<pre lang="cs">public class Encryptor: IEncryptor
{
    private const int SALT_SIZE = 40;
    private const int ITERATIONS_COUNT = 10000;

    public string GetSalt()
    {
        var saltBytes = new byte[SALT_SIZE];
        var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);

        return Convert.ToBase64String(saltBytes);
    }

    public string GetHash(string value, string salt)
    {
        var pbkdf2 = new Rfc2898DeriveBytes
            (value, GetBytes(salt), ITERATIONS_COUNT, HashAlgorithmName.SHA256);

        return Convert.ToBase64String(pbkdf2.GetBytes(SALT_SIZE));
    }

    private static byte[] GetBytes(string value)
    {
        var bytes = new byte[value.Length + sizeof(char)];
        Buffer.BlockCopy(value.ToCharArray(), 0, bytes, 0, bytes.Length);

        return bytes;
    }
}</pre>

Below is `Configure` method in _Startup.cs_:

<pre lang="cs">// This method gets called by the runtime. 
// Use this method to configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog V1");
    });

    var option = new RewriteOptions();
    option.AddRedirect("^$", "swagger");
    app.UseRewriter(option);

    // ...

    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}</pre>

Below is _appsettings.json_:

<pre lang="json">{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "mongo": {
    "connectionString": "mongodb://127.0.0.1:27017",
    "database": "store-identity"
  },
  "jwt": {
    "secret": "9095a623-a23a-481a-aa0c-e0ad96edc103",
    "expiryMinutes": 60
  }
}</pre>

Now, let's test `IdentityMicroservice`.

Open Postman and execute the following `POST` request [http://localhost:44397/api/identity/register](http://localhost:44397/api/identity/register) with the following payload to register a user:

<pre lang="json">{
  "email": "user@store.com",
  "password": "pass"
}
</pre>

Now, execute the following `POST` request [http://localhost:44397/api/identity/login](http://localhost:44397/api/identity/login) with the following payload to create a JWT token:

<pre lang="json">{
  "email": "user@store.com",
  "password": "pass"
}</pre>

![Image](https://github.com/aelassas/microservices/blob/main/img/register.jpg?raw=true)

You can then check the generated token on [jwt.io](https://jwt.io/):

![Image](https://github.com/aelassas/microservices/blob/main/img/401.jpg?raw=true)

That's it. You can execute the following `GET` request [http://localhost:44397/api/identity/validate?email={email}&token={token}](http://localhost:44397/api/identity/validate?email={email}&token={token}) in the same way to validate a JWT token. If the token is valid, the response will be the user Id which is an `ObjectId`.

If you run `IdentityMicroservice` project using IISExpress or Docker, you will get the Swagger UI when accessing [http://localhost:44397/](http://localhost:44397/):

![Image](https://github.com/aelassas/microservices/blob/main/img/identity.jpg?raw=true)

### <a id="catalog-microservice-jwt" name="catalog-microservice-jwt">Adding JWT to Catalog Microservice</a>

Now, let's add JWT authentication to catalog microservice.

First, we have to add `jwt` section in _appsettings.json_:

<pre lang="json">{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "jwt": {
    "secret": "9095a623-a23a-481a-aa0c-e0ad96edc103"
  },
  "mongo": {
    "connectionString": "mongodb://127.0.0.1:27017",
    "database": "store-catalog"
  }
}</pre>

Then, we have to add JWT configuration in `ConfigureServices` method in _Startup.cs_:

<pre lang="cs">public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();

    services.AddJwtAuthentication(Configuration); // JWT Configuration

    // ...
}</pre>

Where `AddJwtAuthentication` extension method is implemented as follows:

<pre lang="cs">public static void AddJwtAuthentication
    (this IServiceCollection services, IConfiguration configuration)
{
    var section = configuration.GetSection("jwt");
    var options = section.Get<JwtOptions>();
    var key = Encoding.UTF8.GetBytes(options.Secret);
    section.Bind(options);
    services.Configure<JwtOptions>(section);

    services.AddSingleton<IJwtBuilder, JwtBuilder>();
    services.AddTransient<JwtMiddleware>();

    services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

    services.AddAuthorization(x =>
    {
        x.DefaultPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();
    });
}</pre>

`JwtMiddleware` is responsible for validating JWT token:

<pre lang="cs">public class JwtMiddleware(IJwtBuilder jwtBuilder) : IMiddleware
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
}</pre>

If the JWT token or the user ID are invalid, we send **401 Unauthorized status**.

Then, we resigster `JwtMiddleware` in `Configure` method in _Startup.cs_:

<pre lang="cs">public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // ...

    app.UseMiddleware<JwtMiddleware>(); // JWT Middleware

    app.UseAuthentication();

    app.UseAuthorization();

    // ...
}</pre>

Then, we specify that we require JWT authentication for our endpoints in _CatalogController.cs_ through `[Authorize]` attribute:

<pre lang="cs">// GET: api/<CatalogController>
[HttpGet]
[Authorize]
public IActionResult Get()
{
    var catalogItems = _catalogRepository.GetCatalogItems();
    return Ok(catalogItems);
}

// ...</pre>

Now, catalog microservice is secured through JWT authentication. Cart microservice was secured in the same way.

Finally, we need to add JWT authentication to Swagger. To do so, we need to update `AddSwaggerGen` in `ConfigureServices` in _Statup.cs_:

<pre lang="cs">public void ConfigureServices(IServiceCollection services)
{
    //...

    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please insert JWT token with the prefix Bearer into field",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });
    });

    //...
}</pre>

Now if you want to run Postman on catalog or cart microservices, you need to specify the **Bearer Token** in **Authorization** tab.

If you want to test catalog or cart microservices with Swagger, you need to click on **Authorize** button and enter the JWT token with the prefix `Bearer` into authorization field.

### <a id="cart-microservice" name="cart-microservice">Cart Microservice</a>

`CartMicroservice` is responsible for managing the cart.

Below are the models used by `CartMicroservice`:

<pre lang="cs">public class Cart
{
    public static readonly string DocumentName = nameof(Cart);

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; init; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? UserId { get; init; }
    public List<CartItem> CartItems { get; init; } = new();
}

public class CartItem
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string? CatalogItemId { get; init; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}</pre>

Below is the repository interface:

<pre lang="cs">public interface ICartRepository
{
    IList<CartItem> GetCartItems(string userId);
    void InsertCartItem(string userId, CartItem cartItem);
    void UpdateCartItem(string userId, CartItem cartItem);
    void DeleteCartItem(string userId, string cartItemId);
    void UpdateCatalogItem(string catalogItemId, string name, decimal price);
    void DeleteCatalogItem(string catalogItemId);
}</pre>

Below is the repository:

<pre lang="cs">public class CartRepository(IMongoDatabase db) : ICartRepository
{
    private readonly IMongoCollection<Cart> _col = 
                           db.GetCollection<Cart>(Cart.DocumentName);

    public IList<CartItem> GetCartItems(string userId) =>
        _col
        .Find(c => c.UserId == userId)
        .FirstOrDefault()?.CartItems ?? new List<CartItem>();

    public void InsertCartItem(string userId, CartItem cartItem)
    {
        var cart = _col.Find(c => c.UserId == userId).FirstOrDefault();
        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                CartItems = new List<CartItem> { cartItem }
            };
            _col.InsertOne(cart);
        }
        else
        {
            var ci = cart
                .CartItems
                .FirstOrDefault(ci => ci.CatalogItemId == cartItem.CatalogItemId);

            if (ci == null)
            {
                cart.CartItems.Add(cartItem);
            }
            else
            {
                ci.Quantity++;
            }

            var update = Builders<Cart>.Update
                .Set(c => c.CartItems, cart.CartItems);
            _col.UpdateOne(c => c.UserId == userId, update);
        }
    }

    public void UpdateCartItem(string userId, CartItem cartItem)
    {
        var cart = _col.Find(c => c.UserId == userId).FirstOrDefault();
        if (cart != null)
        {
            cart.CartItems.RemoveAll(ci => ci.CatalogItemId == cartItem.CatalogItemId);
            cart.CartItems.Add(cartItem);
            var update = Builders<Cart>.Update
                .Set(c => c.CartItems, cart.CartItems);
            _col.UpdateOne(c => c.UserId == userId, update);
        }
    }

    public void DeleteCartItem(string userId, string catalogItemId)
    {
        var cart = _col.Find(c => c.UserId == userId).FirstOrDefault();
        if (cart != null)
        {
            cart.CartItems.RemoveAll(ci => ci.CatalogItemId == catalogItemId);
            var update = Builders<Cart>.Update
                .Set(c => c.CartItems, cart.CartItems);
            _col.UpdateOne(c => c.UserId == userId, update);
        }
    }

    public void UpdateCatalogItem(string catalogItemId, string name, decimal price)
    {
        // Update catalog item in carts
        var carts = GetCarts(catalogItemId);
        foreach (var cart in carts)
        {
            var cartItem = cart.CartItems.FirstOrDefault
                           (i => i.CatalogItemId == catalogItemId);
            if (cartItem != null)
            {
                cartItem.Name = name;
                cartItem.Price = price;
                var update = Builders<Cart>.Update
                    .Set(c => c.CartItems, cart.CartItems);
                _col.UpdateOne(c => c.Id == cart.Id, update);
            }
        }
    }

    public void DeleteCatalogItem(string catalogItemId)
    {
        // Delete catalog item from carts
        var carts = GetCarts(catalogItemId);
        foreach (var cart in carts)
        {
            cart.CartItems.RemoveAll(i => i.CatalogItemId == catalogItemId);
            var update = Builders<Cart>.Update
                .Set(c => c.CartItems, cart.CartItems);
            _col.UpdateOne(c => c.Id == cart.Id, update);
        }
    }

    private IList<Cart> GetCarts(string catalogItemId) =>
        _col.Find(c => c.CartItems.Any(i => i.CatalogItemId == catalogItemId)).ToList();
}</pre>

Below is the controller:

<pre lang="cs">[Route("api/[controller]")]
[ApiController]
public class CartController(ICartRepository cartRepository) : ControllerBase
{
    // GET: api/<CartController>
    [HttpGet]
    [Authorize]
    public IActionResult Get([FromQuery(Name = "u")] string userId)
    {
        var cartItems = cartRepository.GetCartItems(userId);
        return Ok(cartItems);
    }

    // POST api/<CartController>
    [HttpPost]
    [Authorize]
    public IActionResult Post([FromQuery(Name = "u")] string userId, 
                              [FromBody] CartItem cartItem)
    {
        cartRepository.InsertCartItem(userId, cartItem);
        return Ok();
    }

    // PUT api/<CartController>
    [HttpPut]
    [Authorize]
    public IActionResult Put([FromQuery(Name = "u")] string userId, 
                             [FromBody] CartItem cartItem)
    {
        cartRepository.UpdateCartItem(userId, cartItem);
        return Ok();
    }

    // DELETE api/<CartController>
    [HttpDelete]
    [Authorize]
    public IActionResult Delete([FromQuery(Name = "u")] string userId, 
                                [FromQuery(Name = "ci")] string cartItemId)
    {
        cartRepository.DeleteCartItem(userId, cartItemId);
        return Ok();
    }

    // PUT api/<CartController>/update-catalog-item
    [HttpPut("update-catalog-item")]
    [Authorize]
    public IActionResult Put([FromQuery(Name = "ci")] string catalogItemId, 
    [FromQuery(Name = "n")] string name, [FromQuery(Name = "p")] decimal price)
    {
        cartRepository.UpdateCatalogItem(catalogItemId, name, price);
        return Ok();
    }

    // DELETE api/<CartController>/delete-catalog-item
    [HttpDelete("delete-catalog-item")]
    [Authorize]
    public IActionResult Delete([FromQuery(Name = "ci")] string catalogItemId)
    {
        cartRepository.DeleteCatalogItem(catalogItemId);
        return Ok();
    }
}</pre>

`ICartRepository` is added using dependency injection in _Startup.cs_ in order to make the microservice testable:

<pre lang="cs">public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();

    services.AddJwtAuthentication(Configuration); // JWT Configuration

    services.AddMongoDb(Configuration);

    services.AddSingleton<ICartRepository>(sp =>
        new CartRepository(sp.GetService<IMongoDatabase>() ?? 
            throw new Exception("IMongoDatabase not found"))
    );

    // ...
}</pre>

`Configure` method in _Startup.cs_ is the same as the one in `CatalogMicroservice`.

Below is _appsettings.json_:

<pre lang="json">{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "jwt": {
    "secret": "9095a623-a23a-481a-aa0c-e0ad96edc103"
  },
  "mongo": {
    "connectionString": "mongodb://127.0.0.1:27017",
    "database": "store-cart"
  }
}</pre>

API documentation is generated using Swashbuckle. Swagger middleware is configured in _Startup.cs_, in `ConfigureServices` and `Configure` methods in _Startup.cs._

If you run `CartMicroservice` project using IISExpress or Docker, you will get the Swagger UI when accessing [http://localhost:44388/](http://localhost:44388/).

### <a id="gateways" name="gateways">API Gateways</a>

There are two API gateways, one for the frontend and one for the backend.

Let's start with the frontend.

_ocelot.json_ configuration file was added in _Program.cs_ as follows:

<pre lang="cs">var builder = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config
                .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("ocelot.json", false, true)
                .AddJsonFile($"appsettings.
                {hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                .AddJsonFile($"ocelot.
                {hostingContext.HostingEnvironment.EnvironmentName}.json", 
                 optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (hostingContext.HostingEnvironment.EnvironmentName == "Development")
            {
                config.AddJsonFile("appsettings.Local.json", true, true);
            }
        })
        .UseSerilog((_, config) =>
        {
            config
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console();
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });

builder.Build().Run();</pre>

Serilog is configured to write logs to the console. You can, of course, write logs to text files using `WriteTo.File(@"Logs\store.log")` and _`Serilog.Sinks.File`_ nuget package.

Then, here is _Startup.cs_:

<pre lang="cs">public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    // This method gets called by the runtime. 
    // Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddOcelot(Configuration);

        services.AddJwtAuthentication(Configuration); // JWT Configuration

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        services.AddHealthChecks()
            .AddMongoDb(
                mongodbConnectionString: (
                    Configuration.GetSection("mongo").Get<MongoOptions>()
                    ?? throw new Exception("mongo configuration section not found")
                ).ConnectionString,
                name: "mongo",
                failureStatus: HealthStatus.Unhealthy
            );

        services.AddHealthChecksUI().AddInMemoryStorage();
    }

    // This method gets called by the runtime. Use this method 
    // to configure the HTTP request pipeline.
    public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseMiddleware<RequestResponseLogging>();

        app.UseCors("CorsPolicy");

        app.UseAuthentication();

        app.UseHealthChecks("/healthz", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseHealthChecksUI();

        var option = new RewriteOptions();
        option.AddRedirect("^$", "healthchecks-ui");
        app.UseRewriter(option);

        await app.UseOcelot();
    }
}</pre>

`RequestResponseLogging` middleware is responsible for logging requests and responses:

<pre lang="cs">public class RequestResponseLogging(RequestDelegate next, ILogger<RequestResponseLogging> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        var builder = new StringBuilder();
        var request = await FormatRequest(context.Request);
        builder.Append("Request: ").AppendLine(request);
        builder.AppendLine("Request headers:");

        foreach (var header in context.Request.Headers)
        {
            builder.Append(header.Key).Append(": ").AppendLine(header.Value);
        }

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        await next(context);

        var response = await FormatResponse(context.Response);
        builder.Append("Response: ").AppendLine(response);
        builder.AppendLine("Response headers: ");

        foreach (var header in context.Response.Headers)
        {
            builder.Append(header.Key).Append(": ").AppendLine(header.Value);
        }

        logger.LogInformation(builder.ToString());

        await responseBody.CopyToAsync(originalBodyStream);
    }

    private static async Task<string> FormatRequest(HttpRequest request)
    {
        using var reader = new StreamReader(
            request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        var formattedRequest = $"{request.Method} 
        {request.Scheme}://{request.Host}{request.Path}{request.QueryString} {body}";
        request.Body.Position = 0;
        return formattedRequest;
    }

    private static async Task<string> FormatResponse(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        string text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        return $"{response.StatusCode}: {text}";
    }
}</pre>

We used logging in the gateway so that we don't need to check the logs of each microservice.

Here is _ocelot.Development.json_:

<pre lang="json">{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/catalog",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 44326
        }
      ],
      "UpstreamPathTemplate": "/catalog",
      "UpstreamHttpMethod": [ "GET" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    },
    {
      "DownstreamPathTemplate": "/api/catalog/{id}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 44326
        }
      ],
      "UpstreamPathTemplate": "/catalog/{id}",
      "UpstreamHttpMethod": [ "GET" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    },
    {
      "DownstreamPathTemplate": "/api/cart",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 44388
        }
      ],
      "UpstreamPathTemplate": "/cart",
      "UpstreamHttpMethod": [ "GET" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    },
    {
      "DownstreamPathTemplate": "/api/cart",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 44388
        }
      ],
      "UpstreamPathTemplate": "/cart",
      "UpstreamHttpMethod": [ "POST" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    },
    {
      "DownstreamPathTemplate": "/api/cart",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 44388
        }
      ],
      "UpstreamPathTemplate": "/cart",
      "UpstreamHttpMethod": [ "PUT" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    },
    {
      "DownstreamPathTemplate": "/api/cart",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 44388
        }
      ],
      "UpstreamPathTemplate": "/cart",
      "UpstreamHttpMethod": [ "DELETE" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    },
    {
      "DownstreamPathTemplate": "/api/identity/login",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 44397
        }
      ],
      "UpstreamPathTemplate": "/identity/login",
      "UpstreamHttpMethod": [ "POST" ]
    },
    {
      "DownstreamPathTemplate": "/api/identity/register",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 44397
        }
      ],
      "UpstreamPathTemplate": "/identity/register",
      "UpstreamHttpMethod": [ "POST" ]
    },
    {
      "DownstreamPathTemplate": "/api/identity/validate",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 44397
        }
      ],
      "UpstreamPathTemplate": "/identity/validate",
      "UpstreamHttpMethod": [ "GET" ]
    }
  ],
  "GlobalConfiguration": {
    "BaseUrl": "http://localhost:44300/"
  }
}</pre>

And finally, below is _appsettings.json_:

<pre lang="json">{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "jwt": {
    "secret": "9095a623-a23a-481a-aa0c-e0ad96edc103"
  },
  "mongo": {
    "connectionString": "mongodb://127.0.0.1:27017"
  }
}</pre>

Now, let's test the frontend gateway.

First, execute the following `POST` request [http//localhost:44300/identity/login](http://http//localhost:44300/identity/login) with the following payload to create a JWT token:

<pre lang="json">{
  "email": "user@store.com",
  "password": "pass"
}</pre>

We already created that user while testing `IdentityMicroservice`. If you didn't create that user, you can create one by executing the following `POST` request [http://localhost:44300/identity/register](http://localhost:44300/identity/register) with the same payload above.

![Image](https://github.com/aelassas/microservices/blob/main/img/jwt_token.jpg?raw=true)

Then, go to Authorization tab in Postman, select **Bearer Token** type and copy paste the JWT token in **Token** field. Then, execute the following `GET` request to retrieve the catalog [http://localhost:44300/catalog](http://localhost:44300/catalog):

![Image](https://github.com/aelassas/microservices/blob/main/img/token.jpg?raw=true)

If the JWT token is not valid, the response will be **401 Unauthorized**.

You can check the tokens on [jwt.io](https://jwt.io/):

![Image](https://github.com/aelassas/microservices/blob/main/img/401.jpg?raw=true)

If we open the console in Visual Studio, we can see all the logs:

![Image](https://github.com/aelassas/microservices/blob/main/img/logs.jpg?raw=true)

That's it! You can test the other API methods in the same way.

The backend gateway is done pretty much the same way. The only difference is in _ocelot.json_ file.

### <a id="apps" name="apps">Client Apps</a>

There are two client apps. One for the frontend and one for the backend.

The client apps are made using HTML and Vanilla JavaScript for the sake of simplicity.

Let's pick the login page of the frontend for example. Here is the HTML:

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Login</title>
    <link rel="icon" href="data:,">
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <link href="css/login.css" rel="stylesheet" />
</head>
<body>
    <div class="header"></div>
    <div class="login">
        <table>
            <tr>
                <td>Email</td>
                <td><input id="email" type="text" autocomplete="off" 
                                      class="form-control" /></td>
            </tr>
            <tr>
                <td>Password</td>
                <td><input id="password" type="password" class="form-control" /></td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <input id="login" type="button" value="Login" 
                                         class="btn btn-primary" />
                    <input id="register" type="button" value="Register" 
                                         class="btn btn-secondary" />
                </td>
            </tr>
        </table>
    </div>
    <script src="js/login.js" type="module"></script>
</body>
</html>
```

Here is _settings.js_:

<pre lang="jscript">export default {
    uri: "http://localhost:44300/"
};</pre>

And here is _login.js_:

<pre lang="jscript">import settings from "./settings.js";
import common from "./common.js";

window.onload = () => {
    "use strict";

    localStorage.removeItem("auth");

    function login() {
        const user = {
            "email": document.getElementById("email").value,
            "password": document.getElementById("password").value
        };

        common.post(settings.uri + "identity/login?d=frontend", (token) => {
            const auth = {
                "email": user.email,
                "token": token
            };
            localStorage.setItem("auth", JSON.stringify(auth));
            location.href = "/store.html";
        }, () => {
            alert("Wrong credentials.");
        }, user);
    };

    document.getElementById("login").onclick = () => {
        login();
    };

    document.getElementById("password").onkeyup = (e) => {
        if (e.key === 'Enter') {
            login();
        }
    };

    document.getElementById("register").onclick = () => {
        location.href = "/register.html";
    };
};</pre>

_common.js_ contains functions for executing `GET`, `POST` and `DELETE` requests:

<pre lang="jscript">export default {
    post: async (url, callback, errorCallback, content, token) => {
        try {
            const headers = {
                "Content-Type": "application/json;charset=UTF-8"
            };
            if (token) {
                headers["Authorization"] = `Bearer ${token}`;
            }
            const response = await fetch(url, {
                method: "POST",
                headers,
                body: JSON.stringify(content)
            });
            if (response.ok) {
                const data = await response.text();
                if (callback) {
                    callback(data);
                }
            } else {
                if (errorCallback) {
                    errorCallback(response.status);
                }
            }
        } catch (err) {
            if (errorCallback) {
                errorCallback(err);
            }
        }
    },
    get: async (url, callback, errorCallback, token) => {
        try {
            const headers = {
                "Content-Type": "application/json;charset=UTF-8"
            };
            if (token) {
                headers["Authorization"] = `Bearer ${token}`;
            }
            const response = await fetch(url, {
                method: "GET",
                headers
            });
            if (response.ok) {
                const data = await response.text();

                if (callback) {
                    callback(data);
                }
            } else {
                if (errorCallback) {
                    errorCallback(response.status);
                }
            }
        } catch (err) {
            if (errorCallback) {
                errorCallback(err);
            }
        }
    },
    delete: async (url, callback, errorCallback, token) => {
        try {
            const headers = {
                "Content-Type": "application/json;charset=UTF-8"
            };
            if (token) {
                headers["Authorization"] = `Bearer ${token}`;
            }
            const response = await fetch(url, {
                method: "DELETE",
                headers
            });

            if (response.ok) {
                if (callback) {
                    callback();
                }
            } else {
                if (errorCallback) {
                    errorCallback(response.status);
                }
            }
        } catch (err) {
            if (errorCallback) {
                errorCallback(err);
            }
        }
    }
};</pre>

The other pages in the frontend and in the backend are done pretty much the same way.

In the frontend, there are four pages. A login page, a page for registering users, a page for accessing the store, and a page for accessing the cart.

The frontend allows registered users to see the available catalog items, add catalog items to the cart, and remove catalog items from the cart.

Here is a screenshot of the store page in the frontend:

![Image](https://github.com/aelassas/microservices/blob/main/img/store_frontend.jpg?raw=true)

In the backend, there are two pages. A login page and a page for managing the store.

The backend allows admin users to see the available catalog items, create new catalog items, update catalog items, and remove catalog items.

Here is a screenshot of the store page in the backend:

![Image](https://github.com/aelassas/microservices/blob/main/img/store_backend.jpg?raw=true)

## <a id="unit-test" name="unit-test">Unit Tests</a>

In this section, we will be unit testing all the microservices using xUnit and Moq.

When unit testing controller logic, only the contents of a single action are tested, not the behavior of its dependencies or of the framework itself.

xUnit simplifies the testing process and allows us to spend more time focusing on writing our tests.

Moq is a popular and friendly mocking framework for .NET. We will be using it in order to mock repositories and middleware services.

To unit test catalog microservice, first a xUnit testing project `CatalogMicroservice.UnitTests` was created. Then, a unit testing class `CatalogControllerTest` was created. This class contains unit testing methods of the catalog controller.

A reference of the project `CatalogMicroservice` was added to `CatalogMicroservice.UnitTests` project.

Then, Moq was added using Nuget package manager. At this point, we can start focusing on writing our tests.

A reference of `CatalogController` was added to `CatalogControllerTest`:

<pre lang="cs">private readonly CatalogController _controller;</pre>

Then, in the constructor of our unit test class, a mock repository was added as follows:

<pre lang="cs">public CatalogControllerTest()
{
    var mockRepo = new Mock<ICatalogRepository>();
    mockRepo.Setup(repo => repo.GetCatalogItems()).Returns(_items);
    mockRepo.Setup(repo => repo.GetCatalogItem(It.IsAny<string>()))
        .Returns<string>(id => _items.FirstOrDefault(i => i.Id == id));
    mockRepo.Setup(repo => repo.InsertCatalogItem(It.IsAny<CatalogItem>()))
        .Callback<CatalogItem>(_items.Add);
    mockRepo.Setup(repo => repo.UpdateCatalogItem(It.IsAny<CatalogItem>()))
        .Callback<CatalogItem>(i =>
        {
            var item = _items.FirstOrDefault(catalogItem => catalogItem.Id == i.Id);
            if (item != null)
            {
                item.Name = i.Name;
                item.Description = i.Description;
                item.Price = i.Price;
            }
        });
    mockRepo.Setup(repo => repo.DeleteCatalogItem(It.IsAny<string>()))
        .Callback<string>(id => _items.RemoveAll(i => i.Id == id));
    _controller = new CatalogController(mockRepo.Object);
}</pre>

where `_items` is a list of `CatalogItem`:

<pre lang="cs">private static readonly string A54Id = "653e4410614d711b7fc953a7";
private static readonly string A14Id = "253e4410614d711b7fc953a7";
private readonly List<CatalogItem> _items = new()
{
    new()
    {
        Id = A54Id,
        Name = "Samsung Galaxy A54 5G",
        Description = "Samsung Galaxy A54 5G mobile phone",
        Price = 500
    },
    new()
    {
        Id = A14Id,
        Name = "Samsung Galaxy A14 5G",
        Description = "Samsung Galaxy A14 5G mobile phone",
        Price = 200
    }
};</pre>

Then, here is the test of **GET api/catalog**:

<pre lang="cs">[Fact]
public void GetCatalogItemsTest()
{
    var okObjectResult = _controller.Get();
    var okResult = Assert.IsType<OkObjectResult>(okObjectResult);
    var items = Assert.IsType<List<CatalogItem>>(okResult.Value);
    Assert.Equal(2, items.Count);
}</pre>

Here is the test of **GET api/catalog/{id}**:

<pre lang="cs">[Fact]
public void GetCatalogItemTest()
{
    var id = A54Id;
    var okObjectResult = _controller.Get(id);
    var okResult = Assert.IsType<OkObjectResult>(okObjectResult);
    var item = Assert.IsType<CatalogItem>(okResult.Value);
    Assert.Equal(id, item.Id);
}</pre>

Here is the test of **POST api/calatlog**:

<pre lang="cs">[Fact]
public void InsertCatalogItemTest()
{
    var createdResponse = _controller.Post(
        new CatalogItem
        {
            Id = "353e4410614d711b7fc953a7",
            Name = "iPhone 15",
            Description = "iPhone 15 mobile phone",
            Price = 1500
        }
    );
    var response = Assert.IsType<CreatedAtActionResult>(createdResponse);
    var item = Assert.IsType<CatalogItem>(response.Value);
    Assert.Equal("iPhone 15", item.Name);
}</pre>

Here is the test of **PUT api/catalog**:

<pre lang="cs">[Fact]
public void UpdateCatalogItemTest()
{
    var id = A54Id;
    var okObjectResult = _controller.Put(
        new CatalogItem
        {
            Id = id,
            Name = "Samsung Galaxy S23 Ultra",
            Description = "Samsung Galaxy S23 Ultra mobile phone",
            Price = 1500
        });
    Assert.IsType<OkResult>(okObjectResult);
    var item = _items.FirstOrDefault(i => i.Id == id);
    Assert.NotNull(item);
    Assert.Equal("Samsung Galaxy S23 Ultra", item.Name);
    okObjectResult = _controller.Put(null);
    Assert.IsType<NoContentResult>(okObjectResult);
}</pre>

Here is the test of **DELETE api/catalog/{id}**:

<pre lang="cs">[Fact]
public void DeleteCatalogItemTest()
{
    var id = A54Id;
    var item = _items.FirstOrDefault(i => i.Id == id);
    Assert.NotNull(item);
    var okObjectResult = _controller.Delete(id);
    Assert.IsType<OkResult>(okObjectResult);
    item = _items.FirstOrDefault(i => i.Id == id);
    Assert.Null(item);
}</pre>

Unit tests of cart microservice and identity microservice were written in the same way.

Here are the unit tests of cart microservice:

<pre lang="cs">public class CartControllerTest
{
    private readonly CartController _controller;
    private static readonly string UserId = "653e43b8c76b6b56a720803e";
    private static readonly string A54Id = "653e4410614d711b7fc953a7";
    private static readonly string A14Id = "253e4410614d711b7fc953a7";
    private readonly Dictionary<string, List<CartItem>> _carts = new()
    {
        {
            UserId,
            new()
            {
                new()
                {
                    CatalogItemId = A54Id,
                    Name = "Samsung Galaxy A54 5G",
                    Price = 500,
                    Quantity = 1
                },
                new()
                {
                    CatalogItemId = A14Id,
                    Name = "Samsung Galaxy A14 5G",
                    Price = 200,
                    Quantity = 2
                }
            }
        }
    };

    public CartControllerTest()
    {
        var mockRepo = new Mock<ICartRepository>();
        mockRepo.Setup(repo => repo.GetCartItems(It.IsAny<string>()))
            .Returns<string>(id => _carts[id]);
        mockRepo.Setup(repo => repo.InsertCartItem(It.IsAny<string>(), 
                               It.IsAny<CartItem>()))
            .Callback<string, CartItem>((userId, item) =>
            {
                if (_carts.TryGetValue(userId, out var items))
                {
                    items.Add(item);
                }
                else
                {
                    _carts.Add(userId, new List<CartItem> { item });
                }
            });
        mockRepo.Setup(repo => repo.UpdateCartItem(It.IsAny<string>(), 
                               It.IsAny<CartItem>()))
            .Callback<string, CartItem>((userId, item) =>
            {
                if (_carts.TryGetValue(userId, out var items))
                {
                    var currentItem = items.FirstOrDefault
                        (i => i.CatalogItemId == item.CatalogItemId);
                    if (currentItem != null)
                    {
                        currentItem.Name = item.Name;
                        currentItem.Price = item.Price;
                        currentItem.Quantity = item.Quantity;
                    }
                }
            });
        mockRepo.Setup(repo => repo.UpdateCatalogItem
                 (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
            .Callback<string, string, decimal>((catalogItemId, name, price) =>
            {
                var cartItems = _carts
                .Values
                .Where(items => items.Any(i => i.CatalogItemId == catalogItemId))
                .SelectMany(items => items)
                .ToList();

                foreach (var cartItem in cartItems)
                {
                    cartItem.Name = name;
                    cartItem.Price = price;
                }
            });
        mockRepo.Setup(repo => repo.DeleteCartItem
                      (It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string>((userId, catalogItemId) =>
            {
                if (_carts.TryGetValue(userId, out var items))
                {
                    items.RemoveAll(i => i.CatalogItemId == catalogItemId);
                }
            });
        mockRepo.Setup(repo => repo.DeleteCatalogItem(It.IsAny<string>()))
            .Callback<string>((catalogItemId) =>
            {
                foreach (var cart in _carts)
                {
                    cart.Value.RemoveAll(i => i.CatalogItemId == catalogItemId);
                }
            });
        _controller = new CartController(mockRepo.Object);
    }

    [Fact]
    public void GetCartItemsTest()
    {
        var okObjectResult = _controller.Get(UserId);
        var okResult = Assert.IsType<OkObjectResult>(okObjectResult);
        var items = Assert.IsType<List<CartItem>>(okResult.Value);
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public void InsertCartItemTest()
    {
        var okObjectResult = _controller.Post(
            UserId,
            new CartItem
            {
                CatalogItemId = A54Id,
                Name = "Samsung Galaxy A54 5G",
                Price = 500,
                Quantity = 1
            }
        );
        Assert.IsType<OkResult>(okObjectResult);
        Assert.NotNull(_carts[UserId].FirstOrDefault(i => i.CatalogItemId == A54Id));
    }

    [Fact]
    public void UpdateCartItemTest()
    {
        var catalogItemId = A54Id;
        var okObjectResult = _controller.Put(
            UserId,
            new CartItem
            {
                CatalogItemId = A54Id,
                Name = "Samsung Galaxy A54",
                Price = 550,
                Quantity = 2
            }
        );
        Assert.IsType<OkResult>(okObjectResult);
        var catalogItem = _carts[UserId].FirstOrDefault
                          (i => i.CatalogItemId == catalogItemId);
        Assert.NotNull(catalogItem);
        Assert.Equal("Samsung Galaxy A54", catalogItem.Name);
        Assert.Equal(550, catalogItem.Price);
        Assert.Equal(2, catalogItem.Quantity);
    }

    [Fact]
    public void DeleteCartItemTest()
    {
        var id = A14Id;
        var items = _carts[UserId];
        var item = items.FirstOrDefault(i => i.CatalogItemId == id);
        Assert.NotNull(item);
        var okObjectResult = _controller.Delete(UserId, id);
        Assert.IsType<OkResult>(okObjectResult);
        item = items.FirstOrDefault(i => i.CatalogItemId == id);
        Assert.Null(item);
    }

    [Fact]
    public void UpdateCatalogItemTest()
    {
        var catalogItemId = A54Id;
        var okObjectResult = _controller.Put(
            A54Id,
            "Samsung Galaxy A54",
            550
        );
        Assert.IsType<OkResult>(okObjectResult);
        var catalogItem = _carts[UserId].FirstOrDefault
                         (i => i.CatalogItemId == catalogItemId);
        Assert.NotNull(catalogItem);
        Assert.Equal("Samsung Galaxy A54", catalogItem.Name);
        Assert.Equal(550, catalogItem.Price);
        Assert.Equal(1, catalogItem.Quantity);
    }

    [Fact]
    public void DeleteCatalogItemTest()
    {
        var id = A14Id;
        var items = _carts[UserId];
        var item = items.FirstOrDefault(i => i.CatalogItemId == id);
        Assert.NotNull(item);
        var okObjectResult = _controller.Delete(id);
        Assert.IsType<OkResult>(okObjectResult);
        item = items.FirstOrDefault(i => i.CatalogItemId == id);
        Assert.Null(item);
    }
}</pre>

Here are the unit tests of identity microservice:

<pre lang="cs">public class IdentityControllerTest
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
        _controller = new IdentityController
                      (mockRepo.Object, new JwtBuilder(jwtOptions), new Encryptor());
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
            Id ="145e4410614d711b7fc952a7",
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
}</pre>

If we run the unit tests, we will notice that they all pass:

![Image](https://github.com/aelassas/microservices/blob/main/img/tests.png?raw=true)

You can find unit test results on [GitHub Actions](https://github.com/aelassas/microservices/actions/workflows/test.yml).

## <a id="health-checks" name="health-checks">Monitoring using Health Checks</a>

In this section, we will see how to add health checks to catalog microservice for monitoring purposes.

Health checks are endpoints provided by a service to check whether the service is running properly.

Heath checks are used to monitor services such as:

*   Database (SQL Server, Oracle, MySql, MongoDB, etc.)
*   External API connectivity
*   Disk connectivity (read/write)
*   Cache service (Redis, Memcached, etc.)

If you don't find an implementation that suits you, you can create your own custom implementation.

To add health checks to catalog microservice, the following nuget packages were added:

*   `AspNetCore.HealthChecks.MongoDb`
*   `AspNetCore.HealthChecks.UI`
*   `AspNetCore.HealthChecks.UI.Client`
*   `AspNetCore.HealthChecks.UI.InMemory.Storage`

`AspNetCore.HealthChecks.MongoDb` package is used to check the health of MongoDB.

`AspNetCore.HealthChecks.UI` packages are used to use health check UI that stores and shows the health checks results from the configured `HealthChecks` uris.

Then, `ConfigureServices` method in _Startup.cs_ was updated as follows:

<pre lang="cs">services.AddHealthChecks()
    .AddMongoDb(
        mongodbConnectionString: (
            Configuration.GetSection("mongo").Get<MongoOptions>()
            ?? throw new Exception("mongo configuration section not found")
        ).ConnectionString,
        name: "mongo",
        failureStatus: HealthStatus.Unhealthy
    );
services.AddHealthChecksUI().AddInMemoryStorage();</pre>

And `Configure` method in _Startup.cs_ was updated as follows:

<pre lang="cs">app.UseHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHealthChecksUI();</pre>

Finally, _appsettings.json_ was updated as follows:

<pre lang="json">{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "mongo": {
    "connectionString": "mongodb://127.0.0.1:27017",
    "database": "store-catalog"
  },
  "jwt": {
    "secret": "9095a623-a23a-481a-aa0c-e0ad96edc103",
    "expiryMinutes": 60
  },
  "HealthChecksUI": {
    "HealthChecks": [
      {
        "Name": "HTTP-Api-Basic",
        "Uri": "http://localhost:44397/healthz"
      }
    ],
    "EvaluationTimeOnSeconds": 10,
    "MinimumSecondsBetweenFailureNotifications": 60
  }
}</pre>

If we run catalog microservice, we will get the following UI when accessing [http://localhost:44326/healthchecks-ui](http://localhost:44326/healthchecks-ui):

![Image](https://github.com/aelassas/microservices/blob/main/img/ui_20201207210028.jpg?raw=true)

That's it. Health checks of other microservices and gateways were implemented in the same way.

## <a id="run-app" name="run-app">How to Run the Application</a>

To run the application, open the solution _store.sln_ in Visual Studio 2022 as administrator.

You will need to install MongoDB if it is not installed.

First, right click on the solution, click on properties and select multiple startup projects. Select all the projects as startup projects except Middleware and unit tests projects.

Then, press **F5** to run the application.

You can access the frontend from [http://localhost:44317/](http://localhost:44317/).

You can access the backend from [http://localhost:44301/](http://localhost:44301/).

To login to the frontend for the first time, just click on **Register** to create a new user and login.

To login to the backend for the first time, you will need to create an admin user. To do so, open Swagger through [http://localhost:44397/](http://localhost:44397/) and register or open Postman and execute the following `POST` request [http://localhost:44397/api/identity/register](http://localhost:44397/api/identity/register) with the following payload:

<pre lang="json">{
  "email": "admin@store.com",
  "password": "pass",
  "isAdmin": true
}</pre>

Finally, you can login to the backend with the admin user you created.

If you want to modify MongoDB connection string, you need to update _appsettings.json_ of microservices and gateways.

Below are all the endpoints:

*   Frontend: [http://localhost:44317/](http://localhost:44317/)
*   Backend: [http://localhost:44301/](http://localhost:44301/)
*   Frontend gateway: [http://localhost:44300/](http://localhost:44300/)
*   Backend gateway: [http://localhost:44359/](http://localhost:44359/)
*   Identity microservice: [http://localhost:44397/](http://localhost:44397/)
*   Catalog microservice: [http://localhost:44326/](http://localhost:44326/)
*   Cart microservice: [http://localhost:44388/](http://localhost:44388/)

## <a id="deploy-app" name="deploy-app">How to Deploy the Application</a>

You can deploy the application using Docker.

You will need to install Docker it is not installed.

First, copy the source code to a folder on your machine.

Then open a terminal, go to that folder (where _store.sln_ file is located) and run the following command:

<pre lang="shell">docker-compose up</pre>

That's it, the application will be deployed and will run.

Then, you can access the frontend from [http://<hostname>:44317/](http://<hostname>:44317/) and the backend from [http://](http://host-ip:44301/)[<hostname>](http://host-ip:44317/)[:44301/](http://host-ip:44301/).

Here is a screenshot of the application running on Ubuntu:

![Image](https://github.com/aelassas/microservices/blob/main/img/ubuntu.png?raw=true)

For those who want to understand how the deployment is done, here is _docker-compose.yml_:

<pre lang="YAML">version: "3.8"
services:
  mongo:
    image: mongo
    ports:
       - 27017:27017

  catalog:
    build:
      context: .
      dockerfile: src/microservices/CatalogMicroservice/Dockerfile
    depends_on:
      - mongo
    ports:
      - 44326:80

  cart:
    build:
      context: .
      dockerfile: src/microservices/CartMicroservice/Dockerfile
    depends_on:
      - mongo
    ports:
      - 44388:80

  identity:
    build:
      context: .
      dockerfile: src/microservices/IdentityMicroservice/Dockerfile
    depends_on:
      - mongo
    ports:
      - 44397:80

  frontendgw:
    build:
      context: .
      dockerfile: src/gateways/FrontendGateway/Dockerfile
    depends_on:
      - mongo
      - catalog
      - cart
      - identity
    ports:
      - 44300:80

  backendgw:
    build:
      context: .
      dockerfile: src/gateways/BackendGateway/Dockerfile
    depends_on:
      - mongo
      - catalog
      - identity
    ports:
      - 44359:80

  frontend:
    build:
      context: .
      dockerfile: src/uis/Frontend/Dockerfile
    ports:
      - 44317:80

  backend:
    build:
      context: .
      dockerfile: src/uis/Backend/Dockerfile
    ports:
      - 44301:80</pre>

Then, _appsettings.Production.json_ was used in microservices and gateways, and _ocelot.Production.json_ was used in gateways.

For example, here is _appsettings.Production.json_ of catalog microservice:

<pre lang="json">{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "mongo": {
    "connectionString": "mongodb://mongo",
    "database": "store-catalog"
  },
  "HealthChecksUI": {
    "HealthChecks": [
      {
        "Name": "HTTP-Api-Basic",
        "Uri": "http://catalog/healthz"
      }
    ],
    "EvaluationTimeOnSeconds": 10,
    "MinimumSecondsBetweenFailureNotifications": 60
  }
}</pre>

Here is _Dockerfile_ of catalog microservice:

<pre lang="shell"># syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/microservices/CatalogMicroservice/CatalogMicroservice.csproj", 
      "microservices/CatalogMicroservice/"]
COPY src/middlewares middlewares/
RUN dotnet restore "microservices/CatalogMicroservice/CatalogMicroservice.csproj"

WORKDIR "/src/microservices/CatalogMicroservice"
COPY src/microservices/CatalogMicroservice .
RUN dotnet build "CatalogMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CatalogMicroservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogMicroservice.dll"]</pre>

Multistage build is explained [here](https://docs.microsoft.com/en-us/visualstudio/containers/container-build?view=vs-2019). It helps make the process of building containers more efficient, and makes containers smaller by allowing them to contain only the bits that your app needs at run time.

Here is _ocelot.Production.json_ of the frontend gateway:

<pre lang="json">{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/catalog",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "catalog",
          "Port": 80
        }
      ],
      "UpstreamPathTemplate": "/catalog",
      "UpstreamHttpMethod": [ "GET" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    },
    {
      "DownstreamPathTemplate": "/api/catalog/{id}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "catalog",
          "Port": 80
        }
      ],
      "UpstreamPathTemplate": "/catalog/{id}",
      "UpstreamHttpMethod": [ "GET" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    },
    {
      "DownstreamPathTemplate": "/api/cart",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "cart",
          "Port": 80
        }
      ],
      "UpstreamPathTemplate": "/cart",
      "UpstreamHttpMethod": [ "GET" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    },
    {
      "DownstreamPathTemplate": "/api/cart",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "cart",
          "Port": 80
        }
      ],
      "UpstreamPathTemplate": "/cart",
      "UpstreamHttpMethod": [ "POST" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    },
    {
      "DownstreamPathTemplate": "/api/cart",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "cart",
          "Port": 80
        }
      ],
      "UpstreamPathTemplate": "/cart",
      "UpstreamHttpMethod": [ "PUT" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    },
    {
      "DownstreamPathTemplate": "/api/cart",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "cart",
          "Port": 80
        }
      ],
      "UpstreamPathTemplate": "/cart",
      "UpstreamHttpMethod": [ "DELETE" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    },
    {
      "DownstreamPathTemplate": "/api/identity/login",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "identity",
          "Port": 80
        }
      ],
      "UpstreamPathTemplate": "/identity/login",
      "UpstreamHttpMethod": [ "POST" ]
    },
    {
      "DownstreamPathTemplate": "/api/identity/register",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "identity",
          "Port": 80
        }
      ],
      "UpstreamPathTemplate": "/identity/register",
      "UpstreamHttpMethod": [ "POST" ]
    },
    {
      "DownstreamPathTemplate": "/api/identity/validate",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "identity",
          "Port": 80
        }
      ],
      "UpstreamPathTemplate": "/identity/validate",
      "UpstreamHttpMethod": [ "GET" ]
    }
  ],
  "GlobalConfiguration": {
    "BaseUrl": "http://localhost:44300/"
  }
}</pre>

Here is _appsettings.Production.json_ of the frontend gateway:

<pre lang="json">{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "jwt": {
    "secret": "9095a623-a23a-481a-aa0c-e0ad96edc103"
  },
  "mongo": {
    "connectionString": "mongodb://mongo"
  },
  "HealthChecksUI": {
    "HealthChecks": [
      {
        "Name": "HTTP-Api-Basic",
        "Uri": "http://frontendgw/healthz"
      }
    ],
    "EvaluationTimeOnSeconds": 10,
    "MinimumSecondsBetweenFailureNotifications": 60
  }
}</pre>

And finally, here is _Dockerfile_ of the frontend gateway:

<pre lang="shell"># syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/gateways/FrontendGateway/FrontendGateway.csproj", "gateways/FrontendGateway/"]
COPY src/middlewares middlewares/
RUN dotnet restore "gateways/FrontendGateway/FrontendGateway.csproj"

WORKDIR "/src/gateways/FrontendGateway"
COPY src/gateways/FrontendGateway .
RUN dotnet build "FrontendGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FrontendGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FrontendGateway.dll"]</pre>

The configurations of other microservices and the backend gateway are done in pretty much the same way.

That's it! I hope you enjoyed reading this article.

## <a id="references" name="references">References</a>

*   [Microservices architecture style](https://docs.microsoft.com/en-us/azure/architecture/guide/architecture-styles/microservices)
*   [Health monitoring](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/monitor-app-health)
*   [Testing ASP.NET Core services](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/test-aspnet-core-services-web-apps)
*   [Multistage build](https://docs.microsoft.com/en-us/visualstudio/containers/container-build?view=vs-2019#multistage-build)
