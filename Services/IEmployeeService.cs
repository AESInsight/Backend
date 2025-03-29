using Backend.Models;

namespace Backend.Services;

public interface IEmployeeService
{
    Task<List<EmployeeModel>> BulkCreateEmployeesAsync(List<EmployeeModel> employees);
    Task DeleteAllEmployeesAsync(); // Tilføj denne
    Task <int> GetMaxEmployeeIdAsync(); // Tilføj denne
} 