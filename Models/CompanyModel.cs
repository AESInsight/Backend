using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class CompanyModel
{
    [Key] 
    public int CompanyID { get; set; } // Unique ID for the company

    public required string CompanyName { get; set; } // Required field for the company name

    public required string CVR { get; set; } // Required field for the CVR (8-digit company registration number)
    
    [Required]
    [EmailAddress]
    public required string Email { get; set; } // Company's email address
    
    public required string PasswordHash { get; set; } // Hashed password
    
    public string? EmailPassword { get; set; } // Password for the company's email account
}