using Backend.Models;

namespace Backend.Services;

public interface IEmployeeService
{
    Task<List<EmployeeModel>> GetAllEmployeesAsync();
    Task<EmployeeModel> GetEmployeeByIdAsync(int id);
    Task<List<EmployeeModel>> BulkCreateEmployeesAsync(List<EmployeeModel> employees);
    Task DeleteAllEmployeesAsync(); 
    Task <int> GetMaxEmployeeIdAsync();
} 