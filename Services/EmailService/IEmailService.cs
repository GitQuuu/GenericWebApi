namespace Services.EmailService;

/// <summary>
/// Service for sending emails via SMTP
/// </summary>
public interface IEmailService
{
	/// <summary>
	/// Sends a simple email message
	/// </summary>
	/// <param name="to">Recipient email address</param>
	/// <param name="subject">Email subject</param>
	/// <param name="body">Email body (supports HTML)</param>
	/// <param name="isHtml">Whether the body is HTML formatted (default: true)</param>
	/// <returns>True if email was sent successfully, false otherwise</returns>
	Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
	
	/// <summary>
	/// Sends an email to multiple recipients
	/// </summary>
	/// <param name="recipients">List of recipient email addresses</param>
	/// <param name="subject">Email subject</param>
	/// <param name="body">Email body (supports HTML)</param>
	/// <param name="isHtml">Whether the body is HTML formatted (default: true)</param>
	/// <returns>True if email was sent successfully, false otherwise</returns>
	Task<bool> SendEmailAsync(IEnumerable<string> recipients, string subject, string body, bool isHtml = true);
	
	/// <summary>
	/// Sends an email with CC and BCC recipients
	/// </summary>
	/// <param name="to">Primary recipient email address</param>
	/// <param name="subject">Email subject</param>
	/// <param name="body">Email body (supports HTML)</param>
	/// <param name="cc">CC recipients (optional)</param>
	/// <param name="bcc">BCC recipients (optional)</param>
	/// <param name="isHtml">Whether the body is HTML formatted (default: true)</param>
	/// <returns>True if email was sent successfully, false otherwise</returns>
	Task<bool> SendEmailAsync(string to, string subject, string body, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null, bool isHtml = true);
}
