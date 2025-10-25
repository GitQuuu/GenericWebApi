# API Extensions

This folder contains extension methods that organize the configuration logic from `Program.cs` into modular, reusable components.

## Extension Files

### 1. **DatabaseExtensions.cs**
- `AddDatabaseConfiguration()` - Configures Entity Framework DbContext with SQLite
- `AddIdentityConfiguration()` - Configures ASP.NET Core Identity with roles
- `SeedDatabaseAsync()` - Seeds the database with initial data

### 2. **AuthenticationExtensions.cs**
- `AddJwtAuthentication()` - Configures all JWT Bearer authentication schemes:
  - **Local JWT** - Default authentication using symmetric key
  - **Entra (Microsoft)** - Azure AD/Entra ID authentication
  - **Google** - Google OAuth authentication

### 3. **ApplicationServicesExtensions.cs**
- `AddApplicationServices()` - Registers all application services:
  - Controllers
  - HttpContextAccessor
  - Identity seeder
  - Authentication services (orchestrator, identity provider, token, user)
  - Response service

### 4. **SwaggerExtensions.cs**
- `AddSwaggerConfiguration()` - Configures Swagger/OpenAPI documentation with JWT Bearer security
- `UseSwaggerConfiguration()` - Configures Swagger UI middleware (development only)

### 5. **MiddlewareExtensions.cs**
- `ConfigureDevelopmentMiddleware()` - Configures environment-specific middleware (CORS, migrations, HSTS)
- `ConfigureRequestPipeline()` - Configures the HTTP request pipeline (HTTPS, routing, auth, controllers)

## Benefits

✅ **Cleaner Program.cs** - Reduced from 214 lines to 22 lines
✅ **Better organization** - Related configurations grouped together
✅ **Reusability** - Extension methods can be reused across projects
✅ **Maintainability** - Easier to locate and modify specific configurations
✅ **Testability** - Individual configuration sections can be tested independently
