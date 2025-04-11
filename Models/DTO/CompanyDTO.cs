namespace Backend.Models.DTO;

public class CompanyDTO
{
    public int CompanyID { get; set; } // Unique ID for the company
    public string CompanyName { get; set; } = string.Empty; // Company name
    public string CVR { get; set; } = string.Empty; // CVR (8-digit company registration number)
    public string Email { get; set; } = string.Empty; // Company's email address
    public string PasswordHash { get; set; } = string.Empty; // Hashed password
}