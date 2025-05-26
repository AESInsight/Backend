using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Backend.Models;

[ExcludeFromCodeCoverage]
public class CompanyModel
{
    [Key]
    public int CompanyID { get; set; } // Unique ID for the company

    public required string CompanyName { get; set; } // Required field for the company name

    public required string Industry { get; set; } // Required field for the company's industry

    public required string CVR { get; set; } // Required field for the CVR (8-digit company registration number)

    [Required]
    [EmailAddress]
    public required string Email { get; set; } // Company's email address

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public string? ResetPasswordToken { get; set; }

    public DateTime? ResetPasswordTokenExpiry { get; set; }
}