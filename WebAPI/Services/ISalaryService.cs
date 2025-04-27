using Backend.Models;

namespace Backend.Services;

public interface ISalaryService
{
    Task<SalaryModel?> GetLatestSalaryForEmployeeAsync(int employeeId);
    Task<List<SalaryModel>> GetSalaryHistoryAsync(int employeeId);
    Task<SalaryModel> AddSalaryAsync(SalaryModel salary);
}
