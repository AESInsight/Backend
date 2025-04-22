namespace Backend.Models.DTO;

public class JobTitleSalaryDTO
{
    public string JobTitle { get; set; } = string.Empty;
    public Dictionary<string, GenderSalaryData> GenderData { get; set; } = new();
}

public class GenderSalaryData
{
    public double AverageSalary { get; set; }
    public int EmployeeCount { get; set; }
} 