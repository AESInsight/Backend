using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordResetController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<PasswordResetController> _logger;
    private readonly ICompanyService _companyService;

    public PasswordResetController(
        IEmailService emailService,
        ApplicationDbContext dbContext,
        ILogger<PasswordResetController> logger,
        ICompanyService companyService)
    {
        _emailService = emailService;
        _dbContext = dbContext;
        _logger = logger;
        _companyService = companyService;
    }

    [HttpPost("request-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest request)
    {
        // Check both Users and Companies tables
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Email == request.Email);

        if (user == null && company == null)
        {
            return Ok(new { message = "If your email is registered, you will receive a password reset link." });
        }

        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var expiration = DateTime.UtcNow.AddMinutes(5);

        if (user != null)
        {
            user.ResetPasswordToken = token;
            user.ResetPasswordTokenExpiry = expiration;
        }
        else if (company != null)
        {
            company.ResetPasswordToken = token;
            company.ResetPasswordTokenExpiry = expiration;
        }

        await _dbContext.SaveChangesAsync();
        await _emailService.SendPasswordResetEmailAsync(request.Email, token);

        return Ok(new { message = "If your email is registered, you will receive a password reset link." });
    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (request.NewPassword != request.ConfirmPassword)
        {
            return BadRequest(new { message = "Passwords do not match" });
        }

        // Check both Users and Companies tables
        var user = await _dbContext.Users.FirstOrDefaultAsync(u =>
            u.ResetPasswordToken == request.Token &&
            u.ResetPasswordTokenExpiry > DateTime.UtcNow);

        var company = await _dbContext.Companies.FirstOrDefaultAsync(c =>
            c.ResetPasswordToken == request.Token &&
            c.ResetPasswordTokenExpiry > DateTime.UtcNow);

        if (user == null && company == null)
        {
            return BadRequest(new { message = "Invalid or expired token" });
        }

        using (var hmac = new HMACSHA512())
        {
            if (user != null)
            {
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.NewPassword));
                user.PasswordSalt = hmac.Key;
                user.ResetPasswordToken = null;
                user.ResetPasswordTokenExpiry = null;
            }
            else if (company != null)
            {
                company.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.NewPassword));
                company.PasswordSalt = hmac.Key;
                company.ResetPasswordToken = null;
                company.ResetPasswordTokenExpiry = null;
            }
        }

        await _dbContext.SaveChangesAsync();
        return Ok(new { message = "Password has been reset successfully" });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyPassword([FromBody] VerifyPasswordRequest request)
    {
        // Check both Users and Companies tables
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Email == request.Email);

        if (user != null)
        {
            using (var hmac = new HMACSHA512(user.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
                return Ok(new { isValid = computedHash.SequenceEqual(user.PasswordHash) });
            }
        }
        else if (company != null)
        {
            using (var hmac = new HMACSHA512(company.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
                return Ok(new { isValid = computedHash.SequenceEqual(company.PasswordHash) });
            }
        }

        return Ok(new { isValid = false });
    }
}

public class PasswordResetRequest
{
    public string Email { get; set; } = null!;
}

public class ResetPasswordRequest
{
    public string Token { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}

public class VerifyPasswordRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}