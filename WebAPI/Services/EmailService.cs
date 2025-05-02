using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Backend.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly ApplicationDbContext _dbContext;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger, ApplicationDbContext dbContext)
    {
        _configuration = configuration;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        var smtpServer = smtpSettings["Server"];
        var smtpPort = 587;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("AES Insight", "cff@aes-insight.dk"));
        message.To.Add(new MailboxAddress(email, email));
        message.Subject = "Password Reset Request for AES Insight";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
                <h2>Password Reset Request</h2>
                <p>You have requested to reset your password for your AES Insight account.</p>
                <p>To reset your password, please use the following token:</p>
                <p><strong>{resetToken}</strong></p>
                <p>This token will expire in 5 minutes.</p>
                <p>If you did not request this password reset, please ignore this email.</p>
                <p>Best regards,<br>AES Insight Team</p>
            "
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync("cff@aes-insight.dk", "#SecurePassword123");
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}