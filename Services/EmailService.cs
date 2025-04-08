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
        // Get the company from the database
        var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Email == email);
        if (company == null)
        {
            _logger.LogWarning($"Company with email {email} not found");
            return;
        }

        var smtpSettings = _configuration.GetSection("SmtpSettings");
        var smtpServer = smtpSettings["Server"];
        var smtpPort = 587;

        var message = new MimeMessage();
        // Use our fixed one.com email for sending
        message.From.Add(new MailboxAddress("AES Insight", "cff@aes-insight.dk"));
        // Send to the company's email
        message.To.Add(new MailboxAddress(company.CompanyName, company.Email));
        message.Subject = "Password Reset Request for AES Insight";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
                <h2>Password Reset Request</h2>
                <p>Hello {company.CompanyName},</p>
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

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        
        // Use our fixed one.com credentials
        await client.AuthenticateAsync(smtpUsername, smtpPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
} 