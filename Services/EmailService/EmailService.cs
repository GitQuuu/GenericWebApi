using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Services.EmailService;

/// <summary>
/// Email service implementation using MailKit with SMTP
/// Supports Gmail, Outlook, SendGrid, and any SMTP provider
/// </summary>
public partial class EmailService : IEmailService
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
}
