using System.Diagnostics.CodeAnalysis;
namespace Backend.Models.DTO;

[ExcludeFromCodeCoverage]
public class EmployeeIndustryDto
{
    public int EmployeeID { get; set; }
    public string JobTitle { get; set; }
    public int CompanyID { get; set; }
    public string Industry { get; set; }
}