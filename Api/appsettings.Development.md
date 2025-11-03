# appsettings.Development.json Configuration Guide

This document describes the configuration keys in `appsettings.Development.json` and their intended usage.

---

## Logging

Configuration for application logging levels.

### `Logging:LogLevel:Default`
- **Type**: String
- **Default**: `"Information"`
- **Purpose**: Sets the default logging level for the application. Controls the verbosity of logs.
- **Valid Values**: `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical`, `None`

### `Logging:LogLevel:Microsoft.AspNetCore`
- **Type**: String
- **Default**: `"Warning"`
- **Purpose**: Sets the logging level specifically for ASP.NET Core framework logs. Typically set to `Warning` or higher to reduce noise from framework internals.
- **Valid Values**: `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical`, `None`

---

## Auth

Authentication and authorization configuration supporting multiple providers.

### Auth:Local

Configuration for local JWT-based authentication.

#### `Auth:Local:Issuer`
- **Type**: String (URL)
- **Default**: `"https://localhost:5001"`
- **Purpose**: The issuer claim for JWT tokens. Should match your application's URL. Used to validate that tokens were issued by your application.
- **Example Values**:
  - `"https://localhost:5001"` (Development)
  - `"https://api.yourdomain.com"` (Production)
  - `"https://yourdomain.com"` (Production)

#### `Auth:Local:Audience`
- **Type**: String
- **Default**: `"generic-api"`
- **Purpose**: The audience claim for JWT tokens. Identifies the intended recipient of the token. Should be a unique identifier for your API.
- **Example Values**:
  - `"generic-api"`
  - `"my-company-api"`
  - `"api://my-application"`

#### `Auth:Local:SigningKey`
- **Type**: String (GUID/Secret)
- **Default**: `"d70e68fb-fd2f-40e1-bcee-9859ddfc9189"`
- **Purpose**: Secret key used to sign JWT tokens. **IMPORTANT**: Change this to a secure, unique value in production and store it in user secrets or environment variables.
- **Security**: Never commit production signing keys to source control.
- **Example Values**:
  - `"d70e68fb-fd2f-40e1-bcee-9859ddfc9189"` (GUID format)
  - `"your-super-secret-key-min-32-chars-long-for-HS256"`
  - `"base64-encoded-random-string-here"`

#### `Auth:Local:AccessTokenMinutes`
- **Type**: Integer
- **Default**: `60`
- **Purpose**: Specifies the lifetime of access tokens in minutes. After this duration, tokens expire and users must re-authenticate.
- **Example Values**:
  - `15` (15 minutes - high security)
  - `60` (1 hour - balanced)
  - `1440` (24 hours - convenience)
  - `10080` (7 days - long-lived sessions)

#### `Auth:Local:UserActivationMode`
- **Type**: String
- **Default**: `"SelfActivation"`
- **Purpose**: Determines how new user accounts are activated.
- **Valid Values**: 
  - `"SelfActivation"`: Users must activate their own accounts by clicking a confirmation link sent via email
  - `"AdminApproval"`: User accounts require explicit approval by an administrator before becoming active. No email is sent
  - `"AutoActivation"`: User accounts are activated automatically without requiring any manual action. Users can immediately log in after registration

### Auth:Entra

Configuration for Microsoft Entra ID (formerly Azure AD) authentication.

#### `Auth:Entra:Authority`
- **Type**: String (URL)
- **Default**: `"https://login.microsoftonline.com/common"`
- **Purpose**: The authority URL for Microsoft Entra ID authentication. Using `/common` allows users from any Azure AD tenant to authenticate.
- **Note**: Replace `/common` with your specific tenant ID for single-tenant applications.
- **Example Values**:
  - `"https://login.microsoftonline.com/common"` (Multi-tenant)
  - `"https://login.microsoftonline.com/{tenant-id}"` (Single-tenant)
  - `"https://login.microsoftonline.com/organizations"` (Any organizational account)

#### `Auth:Entra:Audience`
- **Type**: String
- **Default**: `"api://Override this with user-secrets or environment variable"`
- **Purpose**: The Application ID URI from your Azure AD app registration. This identifies your API in the Azure ecosystem.
- **Configuration**: Override with user secrets or environment variables. Format: `api://{your-app-id}`
- **Example Values**:
  - `"api://12345678-1234-1234-1234-123456789abc"`
  - `"api://my-api.azurewebsites.net"`

#### `Auth:Entra:AllowAllTenants`
- **Type**: String (Boolean)
- **Default**: `"true"`
- **Purpose**: When `true`, allows authentication from any Azure AD tenant. Set to `false` for single-tenant applications.
- **Valid Values**:
  - `"true"`: Allow multi-tenant authentication
  - `"false"`: Restrict to single tenant

### Auth:Google

Configuration for Google OAuth authentication.

#### `Auth:Google:ClientId`
- **Type**: String
- **Default**: `"Override this with user-secrets or environment variable.apps.googleusercontent.com"`
- **Purpose**: The OAuth 2.0 Client ID from your Google Cloud Console project.
- **Configuration**: Obtain from Google Cloud Console and override with user secrets or environment variables.
- **Example Values**:
  - `"123456789012-abcdefghijklmnopqrstuvwxyz123456.apps.googleusercontent.com"`

#### `Auth:Google:ClientSecret`
- **Type**: String
- **Default**: `"Override this with user-secrets or environment variable"`
- **Purpose**: The OAuth 2.0 Client Secret from your Google Cloud Console project.
- **Security**: Never commit this to source control. Always use user secrets or environment variables.

### `Auth:UserActivationMode`
- **Type**: String
- **Default**: `"SelfActivation"`
- **Purpose**: Global user activation mode that may override provider-specific settings.
- **Valid Values**: 
  - `"SelfActivation"`: Users must activate their own accounts by clicking a confirmation link sent via email
  - `"AdminApproval"`: User accounts require explicit approval by an administrator before becoming active. No email is sent
  - `"AutoActivation"`: User accounts are activated automatically without requiring any manual action. Users can immediately log in after registration

---

## SwaggerUI

Configuration for Swagger/OpenAPI documentation interface.

### `SwaggerUI:Title`
- **Type**: String
- **Default**: `"Replace with your desired name"`
- **Purpose**: The title displayed in the Swagger UI header and browser tab. Should be your API's name.
- **Example Values**:
  - `"My Company API"`
  - `"E-Commerce Platform API"`
  - `"Customer Management System API"`

### `SwaggerUI:Description`
- **Type**: String
- **Default**: `"Replace with your desired description"`
- **Purpose**: A brief description of your API displayed in the Swagger UI. Helps developers understand what your API does.
- **Example Values**:
  - `"RESTful API for managing customer data and orders"`
  - `"Backend API for mobile and web applications"`
  - `"Microservice for authentication and user management"`

### SwaggerUI:Contact

Contact information displayed in the Swagger UI.

#### `SwaggerUI:Contact:Name`
- **Type**: String
- **Default**: `"Replace with your name"`
- **Purpose**: The name of the person or organization responsible for the API.
- **Example Values**:
  - `"John Doe"`
  - `"API Support Team"`
  - `"My Company Inc."`

#### `SwaggerUI:Contact:Email`
- **Type**: String
- **Default**: `"Replace with your email"`
- **Purpose**: Contact email address for API support or inquiries.
- **Example Values**:
  - `"api-support@company.com"`
  - `"developer@mycompany.com"`
  - `"contact@example.com"`

#### `SwaggerUI:Contact:Url`
- **Type**: String (URL)
- **Default**: `"https://ReplaceWithYourWebsite.com"`
- **Purpose**: URL to your website, documentation, or support page.
- **Example Values**:
  - `"https://www.mycompany.com"`
  - `"https://docs.myapi.com"`
  - `"https://support.example.com"`

---

## Email

Configuration for sending emails from the application.

### Email:Smtp

SMTP server configuration for outgoing emails.

#### `Email:Smtp:Host`
- **Type**: String
- **Default**: `"mail518.mailasp.net"`
- **Purpose**: The hostname or IP address of your SMTP server.
- **Note**: Update this with your actual SMTP server address.
- **Example Values**:
  - `"smtp.gmail.com"` (Gmail)
  - `"smtp.office365.com"` (Office 365)
  - `"smtp.sendgrid.net"` (SendGrid)
  - `"smtp.mailgun.org"` (Mailgun)
  - `"smtp-relay.sendinblue.com"` (Brevo/Sendinblue)

#### `Email:Smtp:Port`
- **Type**: String (Integer)
- **Default**: `"587"`
- **Purpose**: The port number for SMTP connection.
- **Valid Values**:
  - `"587"`: TLS/STARTTLS (recommended)
  - `"465"`: SSL
  - `"25"`: Unencrypted (not recommended)
  - `"2525"`: Alternative port (some providers)

#### `Email:Smtp:Username`
- **Type**: String
- **Default**: `"kontakt@qunication.dk"`
- **Purpose**: Username for SMTP authentication.
- **Security**: Consider moving to user secrets or environment variables for production.
- **Example Values**:
  - `"noreply@mycompany.com"`
  - `"apikey"` (SendGrid uses this as username)
  - `"your-email@gmail.com"`

#### `Email:Smtp:Password`
- **Type**: String
- **Default**: `"c-3dasdZ6t@s!8T123"`
- **Purpose**: Password for SMTP authentication.
- **Security**: **CRITICAL** - Remove this from the file and use user secrets or environment variables. Never commit passwords to source control.

#### `Email:Smtp:EnableSsl`
- **Type**: String (Boolean)
- **Default**: `"true"`
- **Purpose**: Enables SSL/TLS encryption for SMTP connection. Should always be `true` for security.
- **Valid Values**:
  - `"true"`: Enable SSL/TLS (recommended)
  - `"false"`: Disable SSL/TLS (not recommended)

### Email:From

Default sender information for outgoing emails.

#### `Email:From:Address`
- **Type**: String (Email)
- **Default**: `"kontakt@qunication.dk"`
- **Purpose**: The email address that appears in the "From" field of sent emails.
- **Example Values**:
  - `"noreply@mycompany.com"`
  - `"support@example.com"`
  - `"notifications@myapp.com"`

#### `Email:From:Name`
- **Type**: String
- **Default**: `"GenericWebApi"`
- **Purpose**: The display name that appears in the "From" field of sent emails.
- **Example Values**:
  - `"My Company"`
  - `"Customer Support"`
  - `"My Application Notifications"`

---

## Seed

Configuration for database seeding with initial data (roles and admin user).

### Seed:Admin

Configuration for the default admin user created during database seeding.

#### `Seed:Admin:Email`
- **Type**: String (Email)
- **Default**: `"admin@example.com"`
- **Purpose**: The email address for the default admin user. This user is automatically created when the application starts in development mode.
- **Required**: Yes - Application will throw an exception if not configured.
- **Example Values**:
  - `"admin@example.com"`
  - `"superadmin@mycompany.com"`
  - `"root@localhost"`

#### `Seed:Admin:Password`
- **Type**: String
- **Default**: `"Admin@1234"`
- **Purpose**: The password for the default admin user. Must meet password complexity requirements (uppercase, lowercase, number, special character).
- **Required**: Yes - Application will throw an exception if not configured.
- **Security**: Change this to a strong password and consider using user secrets or environment variables.
- **Example Values**:
  - `"Admin@1234"`
  - `"SuperSecure123!"`
  - `"MyStr0ng!Pass"`

#### `Seed:Admin:FirstName`
- **Type**: String
- **Default**: `"Admin"`
- **Purpose**: The first name for the admin user's profile.
- **Required**: Yes - Application will throw an exception if not configured.
- **Example Values**:
  - `"Admin"`
  - `"System"`
  - `"John"`

#### `Seed:Admin:LastName`
- **Type**: String
- **Default**: `"Example"`
- **Purpose**: The last name for the admin user's profile.
- **Required**: Yes - Application will throw an exception if not configured.
- **Example Values**:
  - `"Example"`
  - `"Administrator"`
  - `"Doe"`

### `Seed:Roles`
- **Type**: Array of Strings
- **Default**: `["Admin", "User"]`
- **Purpose**: List of roles to create in the database during seeding. These roles are used for authorization throughout the application.
- **Required**: No - If not configured, role seeding will be skipped with a warning.
- **Example Values**:
  - `["Admin", "User"]`
  - `["Admin", "User", "Manager", "Guest"]`
  - `["SuperAdmin", "Admin", "Editor", "Viewer"]`

### `Seed:AdminRoles`
- **Type**: Array of Strings
- **Default**: `["Admin"]`
- **Purpose**: List of roles to assign to the default admin user. The admin user will be granted all roles specified in this array.
- **Required**: No - If not configured, role assignment will be skipped with a warning.
- **Example Values**:
  - `["Admin"]`
  - `["Admin", "User"]`
  - `["SuperAdmin", "Admin"]`

**Note**: Seeding only runs in Development environment by default. The admin user and roles are created only if they don't already exist.

---

## Security Best Practices

⚠️ **IMPORTANT**: This is a development configuration file. For production:

1. **Never commit sensitive data** like passwords, API keys, or signing keys to source control
2. **Use User Secrets** for local development: `dotnet user-secrets set "Key:Path" "value"`
3. **Use Azure Key Vault for production deployments** (preferred) or Environment Variables as an alternative
4. **Rotate secrets regularly**, especially the JWT signing key
5. **Use strong, unique signing keys** - never use the default GUID in production
6. **Enable SSL/TLS** for all external connections (SMTP, authentication providers)

## Configuration Override Priority

Configuration values are loaded in the following order (later sources override earlier ones):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. User Secrets (development only)
4. **Azure Key Vault** (recommended for production - requires configuration)
5. Environment Variables
6. Command-line arguments

**Note**: Azure Key Vault is the **preferred method for production** as it provides:
- Centralized secret management
- Access control and audit logging
- Automatic secret rotation
- Integration with Azure Managed Identity
- Secure storage with encryption at rest

---

## Example: Setting User Secrets

```bash
# JWT Signing Key
dotnet user-secrets set "Auth:Local:SigningKey" "your-secure-random-key-here"

# Google OAuth
dotnet user-secrets set "Auth:Google:ClientId" "your-client-id.apps.googleusercontent.com"
dotnet user-secrets set "Auth:Google:ClientSecret" "your-client-secret"

# Entra ID
dotnet user-secrets set "Auth:Entra:Audience" "api://your-app-id"

# Email Password
dotnet user-secrets set "Email:Smtp:Password" "your-smtp-password"

# Seed Configuration (Admin User)
dotnet user-secrets set "Seed:Admin:Email" "admin@mycompany.com"
dotnet user-secrets set "Seed:Admin:Password" "YourSecurePassword123!"
dotnet user-secrets set "Seed:Admin:FirstName" "Super"
dotnet user-secrets set "Seed:Admin:LastName" "Admin"
```

Run these commands from the `Api` project directory.
