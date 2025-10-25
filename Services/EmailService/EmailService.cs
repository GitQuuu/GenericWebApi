using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Services.EmailService;

/// <summary>
/// Email service implementation using MailKit with SMTP
/// Supports Gmail, Outlook, SendGrid, and any SMTP provider
/// </summary>
public class EmailService : IEmailService
{
	private readonly IConfiguration _configuration;
	private readonly ILogger<EmailService> _logger;
	private readonly string _smtpHost;
	private readonly int _smtpPort;
	private readonly string _smtpUsername;
	private readonly string _smtpPassword;
	private readonly string _fromEmail;
	private readonly string _fromName;
	private readonly bool _enableSsl;

	public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
	{
		_configuration = configuration;
		_logger = logger;
		
		// Load SMTP configuration
		_smtpHost = _configuration["Email:Smtp:Host"] 
			?? throw new InvalidOperationException("Email:Smtp:Host configuration is missing");
		_smtpPort = int.Parse(_configuration["Email:Smtp:Port"] ?? "587");
		_smtpUsername = _configuration["Email:Smtp:Username"] 
			?? throw new InvalidOperationException("Email:Smtp:Username configuration is missing");
		_smtpPassword = _configuration["Email:Smtp:Password"] 
			?? throw new InvalidOperationException("Email:Smtp:Password configuration is missing");
		_fromEmail = _configuration["Email:From:Address"] 
			?? throw new InvalidOperationException("Email:From:Address configuration is missing");
		_fromName = _configuration["Email:From:Name"] ?? "GenericWebApi";
		_enableSsl = bool.Parse(_configuration["Email:Smtp:EnableSsl"] ?? "true");
	}

	public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
	{
		return await SendEmailAsync(new[] { to }, subject, body, isHtml);
	}

	public async Task<bool> SendEmailAsync(IEnumerable<string> recipients, string subject, string body, bool isHtml = true)
	{
		return await SendEmailAsync(recipients.First(), subject, body, null, recipients.Skip(1), isHtml);
	}

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
