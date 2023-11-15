[![Build](https://github.com/aelassas/microservices/actions/workflows/build.yml/badge.svg)](https://github.com/aelassas/microservices/actions/workflows/build.yml) [![Test](https://github.com/aelassas/microservices/actions/workflows/test.yml/badge.svg)](https://github.com/aelassas/microservices/actions/workflows/test.yml) [![CodeFactor](https://www.codefactor.io/repository/github/aelassas/microservices/badge)](https://www.codefactor.io/repository/github/aelassas/microservices)

Microservices sample architecture using ASP.NET Core, Ocelot, MongoDB and JWT.

# Development Environment

- Visual Studio 2022 >= 17.8.0
- .NET 8.0
- MongoDB
- Postman

# Architecture

There are three microservices:

- **CatalogMicroservice**: allows to manage the catalog.
- **CartMicroservice**: allows to manage the cart.
- **IdentityMicroservice**: allows to manage users.

Each microservice implements a single businness capability and has its own MongoDB database.

There are two API gateways, one for the frontend and one for the backend.

Below is the frontend API gateway:

- **GET /catalog**: retrieves catalog items.
- **GET /catalog/{id}**: retrieves a catalog item.
- **GET /cart**: retrieves cart items.
- **POST /cart**: adds a cart item.
- **PUT /cart**: updates a cart item.
- **DELETE /cart**: deletes a cart item.
- **POST /identity/login**: performs a login.
- **POST /identity/register**: registers a user.
- **GET /identity/validate**: validates a JWT token.

Below is the backend API gateway:

- **GET /catalog**: retrieves catalog items.
- **GET /catalog/{id}**: retrieves a catalog item.
- **POST /catalog**: creates a catalog item.
- **PUT /catalog**: updates a catalog item.
- **DELETE /catalog**: deletes a catalog item.
- **PUT /cart/update-catalog-item**: updates a catalog item in carts.
- **DELETE /cart/delete-catalog-item**: deletes catalog item references from carts.
- **POST /identity/login**: performs a login.
- **POST /identity/register**: registers a user.
- **GET /identity/validate**: validates a JWT token.

Finally, there are two client apps. A frontend for accessing the store and a backend for managing the store.

The frontend allows registered users to see the available catalog items, allows to add catalog items to the cart, and allows to remove catalog items from the cart.

The backend allows admin users to see the available catalog items, allows to add new catalog items, and allows to remove catalog items.

# Source Code

- **CatalogMicroservice** project contains the source code of the microservice managing the catalog.
- **CartMicroservice** project contains the source code of the microservice managing the cart.
- **IdentityMicroservice** project contains the source code of the microservice managing users.
- **Middleware** project contains the source code of common functionalities used by microservices.
- **FrontendGateway** project contains the source code of the frontend API gateway.
- **BackendGateway** project contains the source code of the backend API gateway.
- **Frontend** project contains the source code of the frontend client app.
- **Backend** project contains the source code of the backend client app.

# How to Run the Application

You can run the application using IISExpress in Visual Studio 2022 or Docker.

You will need to install MongoDB if it is not installed.

First, right click on the solution, click on properties and select multiple startup projects. Select all the projects as startup projects except Middleware, Model and unit tests projects.

Then, press **F5** to run the application.

You can access the frontend from https://localhost:44317/.

You can access the backend from https://localhost:44301/.

To login to the frontend for the first time, just click on **Register** to create a new user and login.

To login to the backend for the first time, you will need to create an admin user. To do so, open Swagger on https://localhost:44397/ or Postman and execute the following POST request https://localhost:44397/api/identity/register with the following payload:

```js
{
  "email": "admin@store.com",
  "password": "password",
  "isAdmin": true
}
```
Finally, you can login to the backend with the admin user you created and create new products.

If you want to modify MongoDB connection string, you need to update *appsettings.json* of microservices and gateways.

# Further Reading

- [Microservices architecture style](https://docs.microsoft.com/en-us/azure/architecture/guide/architecture-styles/microservices)
- [Health monitoring](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/monitor-app-health)
- [Load balancing](https://ocelot.readthedocs.io/en/latest/features/loadbalancer.html)
- [Testing ASP.NET Core services](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/test-aspnet-core-services-web-apps)
