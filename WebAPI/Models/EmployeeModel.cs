using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Models;

public class EmployeeModel
{
    [Key] // Mark EmployeeID as the primary key
    public int EmployeeID { get; set; } // Primary key for EmployeeModel

    public string? JobTitle { get; set; } // Job title of the employee

    public int Experience { get; set; } // Years of experience of the employee

    public string? Gender { get; set; } // Gender of the employee

    public int CompanyID { get; set; } // Foreign key referencing the company

    [ForeignKey("CompanyID")]
    public CompanyModel? Company { get; set; } // Navigation property for the related Company

    public ICollection<SalaryModel> Salaries { get; set; } = new List<SalaryModel>();
}
