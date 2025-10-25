# EmailService Structure

The EmailService has been organized into partial classes for better maintainability and separation of concerns.

## Folder Structure

```
Services/EmailService/
├── IEmailService.cs                          # Interface definition
├── EmailService.cs                           # Main class with constructor & fields
├── SendEmailAsync.cs                         # Single recipient method
├── SendEmailToMultipleAsync.cs               # Multiple recipients method
├── SendEmailWithCcBccAsync.cs                # Full-featured method with CC/BCC
├── README.md                                 # Usage documentation
└── STRUCTURE.md                              # This file
```

## File Responsibilities

### **EmailService.cs** (Main)
- Constructor
- Dependency injection setup
- Private fields (_configuration, _logger, SMTP settings)
- Configuration loading and validation

### **SendEmailAsync.cs**
- Simple single-recipient email method
- Delegates to the multiple recipients method

### **SendEmailToMultipleAsync.cs**
- Multiple recipients email method
- Delegates to the full-featured method

### **SendEmailWithCcBccAsync.cs**
- Core implementation with full email functionality
- Supports CC and BCC recipients
- Handles SMTP connection, authentication, and sending
- Error logging and exception handling

## Benefits of This Structure

1. **Separation of Concerns** - Each file has a single responsibility
2. **Easy Navigation** - Find specific functionality quickly
3. **Maintainability** - Modify one method without affecting others
4. **Scalability** - Easy to add new email methods (e.g., attachments, templates)
5. **Readability** - Smaller, focused files are easier to understand

## Adding New Methods

To add new email functionality (e.g., attachments):

1. Create a new file: `SendEmailWithAttachmentsAsync.cs`
2. Make it a partial class: `public partial class EmailService`
3. Implement your method
4. Add the method signature to `IEmailService.cs`

Example:
```csharp
// SendEmailWithAttachmentsAsync.cs
namespace Services.EmailService;

public partial class EmailService
{
    public async Task<bool> SendEmailWithAttachmentsAsync(
        string to, 
        string subject, 
        string body, 
        IEnumerable<string> attachmentPaths)
    {
        // Implementation here
    }
}
```
