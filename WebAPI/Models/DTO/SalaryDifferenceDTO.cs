using System.Diagnostics.CodeAnalysis;

namespace Backend.Models.DTO;

[ExcludeFromCodeCoverage]
public class SalaryDifferenceDTO
{
    public string JobTitle { get; set; } = string.Empty;
    public decimal MaleAverageSalary { get; set; }
    public decimal FemaleAverageSalary { get; set; }
    public decimal Difference { get; set; }
    public DateTime Month { get; set; }
    public double AverageSalary { get; set; }
} 