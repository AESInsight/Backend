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
    private static readonly Dictionary<string, (string Email, DateTime Expiration)> _resetTokens = new();

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
        // Check if email exists in users
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Email);

        if (user == null)
        {
            return Ok(new { message = "If your email is registered, you will receive a password reset link." });
        }

        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var expiration = DateTime.UtcNow.AddMinutes(5);
        _resetTokens[token] = (request.Email, expiration);
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

        if (!_resetTokens.TryGetValue(request.Token, out var tokenInfo))
        {
            return BadRequest(new { message = "Invalid token" });
        }

        if (DateTime.UtcNow > tokenInfo.Expiration)
        {
            _resetTokens.Remove(request.Token);
            return BadRequest(new { message = "Token has expired. Please request a new password reset." });
        }

        // Find user by email
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == tokenInfo.Email);
        if (user != null)
        {
            using (var hmac = new HMACSHA512())
            {
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.NewPassword));
                user.PasswordSalt = hmac.Key;
            }
            await _dbContext.SaveChangesAsync();
            _resetTokens.Remove(request.Token);
            return Ok(new { message = "Password has been reset successfully" });
        }

        return BadRequest(new { message = "Invalid request" });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyPassword([FromBody] VerifyPasswordRequest request)
    {
        // Find user by email
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Email);
        if (user != null)
        {
            using (var hmac = new HMACSHA512(user.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
                return Ok(new { isValid = computedHash.SequenceEqual(user.PasswordHash) });
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