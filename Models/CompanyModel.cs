using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class CompanyModel
{
    [Key] // Mark CompanyID as the primary key
    public int CompanyID { get; set; } // Unique ID for the company

    public required string CompanyName { get; set; } // Required field for the company name

    public required string CVR { get; set; } // Required field for the CVR (8-digit company registration number)
}