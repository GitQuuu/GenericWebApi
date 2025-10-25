namespace Services.EmailService;

/// <summary>
/// Multiple recipients email methods
/// </summary>
public partial class EmailService
{
	/// <summary>
	/// Sends an email to multiple recipients
	/// </summary>
	/// <param name="recipients">List of recipient email addresses</param>
	/// <param name="subject">Email subject</param>
	/// <param name="body">Email body (supports HTML)</param>
	/// <param name="isHtml">Whether the body is HTML formatted (default: true)</param>
	/// <returns>True if email was sent successfully, false otherwise</returns>
	public async Task<bool> SendEmailAsync(IEnumerable<string> recipients, string subject, string body, bool isHtml = true)
	{
		return await SendEmailAsync(recipients.First(), subject, body, null, recipients.Skip(1), isHtml);
	}
}
