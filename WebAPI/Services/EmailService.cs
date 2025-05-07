using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Backend.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        var smtpServer = smtpSettings["Server"];
        var smtpPort = smtpSettings.GetValue<int>("Port", 587);
        var smtpUsername = smtpSettings["Username"];
        var smtpPassword = smtpSettings["Password"];

        if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
        {
            _logger.LogError("SMTP settings are not properly configured");
            throw new InvalidOperationException("SMTP settings are not properly configured");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("AES Insight", smtpUsername));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Password Reset Request for AES Insight";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
                <h2>Password Reset Request</h2>
                <p>Hello,</p>
                <p>You have requested to reset your password for your AES Insight account.</p>
                <p>To reset your password, please follow these steps:</p>
                <ol>
                    <li>Go to <a href='http://localhost:5170/swagger'>Swagger UI</a></li>
                    <li>Find the POST /api/PasswordReset/reset endpoint</li>
                    <li>Click 'Try it out'</li>
                    <li>Enter the following information:
                        <ul>
                            <li>token: {resetToken}</li>
                            <li>newPassword: (your new password)</li>
                            <li>confirmPassword: (your new password again)</li>
                        </ul>
                    </li>
                    <li>Click 'Execute'</li>
                </ol>
                <p><strong>Important:</strong> This token will expire in 5 minutes.</p>
                <p>If you did not request this password reset, please ignore this email.</p>
                <p>Best regards,<br>AES Insight Team</p>
            "
        };

        message.Body = bodyBuilder.ToMessageBody();

        try
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUsername, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                _logger.LogInformation($"Password reset email sent to {email}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending password reset email: {ex.Message}");
            throw;
        }
    }
} 