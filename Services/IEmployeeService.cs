using Backend.Models;

namespace Backend.Services;

public interface IEmployeeService
{
    Task<List<EmployeeModel>> BulkCreateEmployeesAsync(List<EmployeeModel> employees);
    Task DeleteAllEmployeesAsync(); 
    Task <int> GetMaxEmployeeIdAsync();
} 