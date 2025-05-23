using System.Diagnostics.CodeAnalysis;
namespace Backend.Models.DTO;

[ExcludeFromCodeCoverage]
public class JobTitleSalaryDTO
{
    public string JobTitle { get; set; } = string.Empty;
    public Dictionary<string, GenderSalaryData> GenderData { get; set; } = new();
}
[ExcludeFromCodeCoverage]
public class GenderSalaryData
{
    public double AverageSalary { get; set; }
    public int EmployeeCount { get; set; }
} 