using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

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

        if (string.IsNullOrEmpty(company.EmailPassword))
        {
            _logger.LogWarning($"Email password not set for company {company.CompanyName}");
            return;
        }

        var smtpSettings = _configuration.GetSection("SmtpSettings");
        var smtpServer = smtpSettings["Server"];
        var smtpPort = 587;
        
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(company.CompanyName, company.Email));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Password Reset Request";

        var resetLink = $"https://aes-insight.dk/reset-password?token={resetToken}&email={email}";
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
                <h2>Password Reset Request</h2>
                <p>You have requested to reset your password. Click the link below to proceed:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>If you didn't request this, please ignore this email.</p>
                <p>This link will expire in 1 hour.</p>
            "
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        
        // Use the company's email and email password for authentication
        await client.AuthenticateAsync(company.Email, company.EmailPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
} 