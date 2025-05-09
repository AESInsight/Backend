namespace Backend.Models.DTO;

public class EmployeeDto
{
    public int EmployeeID { get; set; }
    public string? JobTitle { get; set; }
    public int Experience { get; set; }
    public string? Gender { get; set; }
    public int CompanyID { get; set; } // Only include CompanyID
}