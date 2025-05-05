using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class CompanyModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment ID
    public int CompanyID { get; set; } // Unique ID for the company

    public required string CompanyName { get; set; } // Required field for the company name

    public required string Industry { get; set; } // Required field for the company's industry

    public required string CVR { get; set; } // Required field for the CVR (8-digit company registration number)
    
    [Required]
    [EmailAddress]
    public required string Email { get; set; } // Company's email address
}