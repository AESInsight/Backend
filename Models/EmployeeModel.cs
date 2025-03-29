using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Models;

public class EmployeeModel
{
    [Key] // Markerer EmployeeID som primærnøgle
    public int EmployeeID { get; set; } //Primary key for EmployeeModel
    public string JobTitle { get; set; }
    public double Salary { get; set; }
    public int Experience { get; set; }
    public string Gender { get; set; }
    public int CompanyID { get; set; }
}
