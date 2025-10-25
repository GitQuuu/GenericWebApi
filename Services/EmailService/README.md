# Email Service

A free, production-ready email service using **MailKit** with SMTP support.

## Features

- ✅ **Free** - No API costs, works with any SMTP provider
- ✅ **Multiple recipients** - Send to single or multiple recipients
- ✅ **CC/BCC support** - Full email functionality
- ✅ **HTML emails** - Support for both HTML and plain text
- ✅ **Logging** - Built-in error logging
- ✅ **Async** - Fully asynchronous implementation

## Configuration

### 1. Update `appsettings.json` or `appsettings.Development.json`

```json
{
  "Email": {
    "Smtp": {
      "Host": "smtp.gmail.com",
      "Port": "587",
      "Username": "your-email@gmail.com",
      "Password": "your-app-password",
      "EnableSsl": "true"
    },
    "From": {
      "Address": "your-email@gmail.com",
      "Name": "Your App Name"
    }
  }
}
```

### 2. Free SMTP Provider Options

#### **Gmail** (Recommended for development)
- Host: `smtp.gmail.com`
- Port: `587`
- **Important**: Use an [App Password](https://support.google.com/accounts/answer/185833), not your regular password
- Free tier: 500 emails/day

#### **Outlook/Hotmail**
- Host: `smtp-mail.outlook.com`
- Port: `587`
- Use your regular Outlook credentials

#### **SendGrid** (Recommended for production)
- Host: `smtp.sendgrid.net`
- Port: `587`
- Username: `apikey`
- Password: Your SendGrid API key
- Free tier: 100 emails/day

#### **Mailgun**
- Host: `smtp.mailgun.org`
- Port: `587`
- Free tier: 5,000 emails/month

## Usage Examples

### Basic Usage in a Controller

```csharp
using Services.EmailService;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IEmailService _emailService;

    public NotificationController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send-welcome")]
    public async Task<IActionResult> SendWelcomeEmail(string userEmail, string userName)
    {
        var subject = "Welcome to GenericWebApi!";
        var body = $@"
            <h1>Welcome {userName}!</h1>
            <p>Thank you for registering with our service.</p>
            <p>We're excited to have you on board!</p>
        ";

        var success = await _emailService.SendEmailAsync(userEmail, subject, body);
        
        return success 
            ? Ok("Email sent successfully") 
            : StatusCode(500, "Failed to send email");
    }
}
```

### Send to Multiple Recipients

```csharp
var recipients = new[] { "user1@example.com", "user2@example.com" };
await _emailService.SendEmailAsync(recipients, "Team Update", "<h1>Hello Team!</h1>");
```

### Send with CC and BCC

```csharp
await _emailService.SendEmailAsync(
    to: "primary@example.com",
    subject: "Important Notice",
    body: "<p>This is important</p>",
    cc: new[] { "manager@example.com" },
    bcc: new[] { "admin@example.com" }
);
```

### Send Plain Text Email

```csharp
await _emailService.SendEmailAsync(
    to: "user@example.com",
    subject: "Plain Text Email",
    body: "This is a plain text email without HTML formatting.",
    isHtml: false
);
```

## Common Use Cases

### Password Reset Email

```csharp
public async Task<bool> SendPasswordResetEmail(string email, string resetToken)
{
    var resetLink = $"https://yourapp.com/reset-password?token={resetToken}";
    var body = $@"
        <h2>Password Reset Request</h2>
        <p>Click the link below to reset your password:</p>
        <a href='{resetLink}'>Reset Password</a>
        <p>This link expires in 1 hour.</p>
    ";
    
    return await _emailService.SendEmailAsync(email, "Reset Your Password", body);
}
```

### Email Verification

```csharp
public async Task<bool> SendVerificationEmail(string email, string verificationCode)
{
    var body = $@"
        <h2>Verify Your Email</h2>
        <p>Your verification code is: <strong>{verificationCode}</strong></p>
        <p>This code expires in 15 minutes.</p>
    ";
    
    return await _emailService.SendEmailAsync(email, "Verify Your Email", body);
}
```

## Security Best Practices

1. **Never commit credentials** - Use User Secrets for development:
   ```bash
   dotnet user-secrets set "Email:Smtp:Password" "your-password"
   ```

2. **Use environment variables** in production:
   ```bash
   Email__Smtp__Password=your-password
   ```

3. **For Gmail** - Use App Passwords, not your main password

4. **Rate limiting** - Implement rate limiting to prevent abuse

## Troubleshooting

### Gmail "Less secure app" error
- Enable 2-factor authentication
- Generate an App Password: https://myaccount.google.com/apppasswords

### Connection timeout
- Check firewall settings
- Verify SMTP port (587 for TLS, 465 for SSL)
- Try `EnableSsl: false` for port 25 (not recommended)

### Authentication failed
- Double-check username and password
- For Gmail, use App Password
- For SendGrid, username must be "apikey"

## Dependencies

- **MailKit** 4.3.0 - Industry-standard email library for .NET
