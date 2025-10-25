using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Services.EmailService;

/// <summary>
/// Full-featured email methods with CC/BCC support
/// </summary>
public partial class EmailService
{
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
	public async Task<bool> SendEmailAsync(string to, string subject, string body, IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null, bool isHtml = true)
	{
		try
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(_fromName, _fromEmail));
			message.To.Add(MailboxAddress.Parse(to));
			
			// Add CC recipients
			if (cc != null)
			{
				foreach (var ccEmail in cc)
				{
					message.Cc.Add(MailboxAddress.Parse(ccEmail));
				}
			}
			
			// Add BCC recipients
			if (bcc != null)
			{
				foreach (var bccEmail in bcc)
				{
					message.Bcc.Add(MailboxAddress.Parse(bccEmail));
				}
			}
			
			message.Subject = subject;
			
			var bodyBuilder = new BodyBuilder();
			if (isHtml)
			{
				bodyBuilder.HtmlBody = body;
			}
			else
			{
				bodyBuilder.TextBody = body;
			}
			
			message.Body = bodyBuilder.ToMessageBody();
			
			using var client = new SmtpClient();
			
			// Connect to SMTP server
			await client.ConnectAsync(_smtpHost, _smtpPort, _enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
			
			// Authenticate
			await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
			
			// Send email
			await client.SendAsync(message);
			
			// Disconnect
			await client.DisconnectAsync(true);
			
			_logger.LogInformation("Email sent successfully to {To}", to);
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to send email to {To}. Subject: {Subject}", to, subject);
			return false;
		}
	}
}
