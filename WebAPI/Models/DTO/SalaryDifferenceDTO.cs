using System.Diagnostics.CodeAnalysis;

namespace Backend.Models.DTO;

[ExcludeFromCodeCoverage]
public class SalaryDifferenceDTO
{
    public DateTime Month { get; set; }
    public double AverageSalary { get; set; }
} 