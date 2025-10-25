namespace Services.EmailService;

/// <summary>
/// Single recipient email methods
/// </summary>
public partial class EmailService
{
	/// <summary>
	/// Sends a simple email message to a single recipient
	/// </summary>
	/// <param name="to">Recipient email address</param>
	/// <param name="subject">Email subject</param>
	/// <param name="body">Email body (supports HTML)</param>
	/// <param name="isHtml">Whether the body is HTML formatted (default: true)</param>
	/// <returns>True if email was sent successfully, false otherwise</returns>
	public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
	{
		return await SendEmailAsync(new[] { to }, subject, body, isHtml);
	}
}
