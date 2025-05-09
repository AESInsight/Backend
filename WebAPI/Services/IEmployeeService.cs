using Backend.Models;
using Backend.Models.DTO;

namespace Backend.Services;

public interface IEmployeeService
{
    Task<List<EmployeeModel>> GetAllEmployeesAsync();
    Task<EmployeeModel> GetEmployeeByIdAsync(int id);
    Task<EmployeeModel> UpdateEmployeeAsync(int id, EmployeeModel employee);
    Task<EmployeeModel> DeleteEmployeeAsync(int id);
    Task<List<EmployeeModel>> BulkCreateEmployeesAsync(List<EmployeeModel> employees);
    Task DeleteAllEmployeesAsync(); 
    Task<int> GetMaxEmployeeIdAsync();
    Task<List<string>> GetAllJobTitlesAsync();
    Task<List<EmployeeModel>> GetEmployeesByJobTitleAsync(string jobTitle);
    Task<Dictionary<string, List<SalaryDifferenceDTO>>> GetSalaryDifferencesByGenderAsync(string jobTitle);
    Task<Dictionary<string, List<SalaryDifferenceDTO>>> GetAllSalaryDifferencesByGenderAsync();
}