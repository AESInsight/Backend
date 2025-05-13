using System.Diagnostics.CodeAnalysis;

namespace Backend.Models.DTO;

[ExcludeFromCodeCoverage]
public class SalaryDTO
{
    public int SalaryID { get; set; } // Unique ID of the salary entry
    public int EmployeeID { get; set; } // Reference to the employee
    public decimal Salary { get; set; } // Salary amount
    public DateTime Timestamp { get; set; } // When this salary was registered
}
