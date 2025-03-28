using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class EmployeeModel
{
    [Key]
    public string EmployeeID { get; set; }
    public string JobTitle { get; set; }
    public double Salary { get; set; }
    public int Experience { get; set; }
    public string Gender { get; set; }
    public string CompanyID { get; set; }
}
