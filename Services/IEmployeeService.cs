using Backend.Models;

namespace Backend.Services;

public interface IEmployeeService
{
    Task<List<EmployeeModel>> GetAllEmployeesAsync();
    Task<EmployeeModel> GetEmployeeByIdAsync(int id);
    Task<EmployeeModel> UpdateEmployeeAsync(int id, EmployeeModel employee);
    Task<List<EmployeeModel>> BulkCreateEmployeesAsync(List<EmployeeModel> employees);
    Task DeleteAllEmployeesAsync(); 
    Task <int> GetMaxEmployeeIdAsync();
} 