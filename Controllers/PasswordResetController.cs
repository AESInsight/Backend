using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
        var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Email == request.Email);
        if (company == null)
        {
            return Ok(new { message = "If your email is registered, you will receive a password reset link." });
        }

        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        await _emailService.SendPasswordResetEmailAsync(request.Email, token);

        return Ok(new { message = "If your email is registered, you will receive a password reset link." });
    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Email == request.Email);
        if (company == null)
        {
            return BadRequest(new { message = "Invalid request" });
        }

        // In a real implementation, you would verify the token here
        // For now, we'll just update the password
        var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        company.PasswordHash = newPasswordHash;
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "Password has been reset successfully" });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyPassword([FromBody] VerifyPasswordRequest request)
    {
        var isValid = await _companyService.VerifyPasswordAsync(request.Email, request.Password);
        return Ok(new { isValid });
    }
}

public class PasswordResetRequest
{
    public string Email { get; set; } = null!;
}

public class ResetPasswordRequest
{
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

public class VerifyPasswordRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
} 