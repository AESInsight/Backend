using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _context;

    public EmployeeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeModel>> BulkCreateEmployeesAsync(List<EmployeeModel> employees)
{
    try
    {
        // Tilføj alle employees til databasen
        await _context.Employee.AddRangeAsync(employees);

        // Gem ændringerne
        await _context.SaveChangesAsync();

        return employees;
    }
    catch (Exception ex)
    {
        // Log den indre undtagelse for at få flere detaljer
        throw new Exception($"Error during bulk employee creation: {ex.InnerException?.Message ?? ex.Message}", ex);
    }
}
public async Task DeleteAllEmployeesAsync()
{
    _context.Employee.RemoveRange(_context.Employee);
    await _context.SaveChangesAsync();
}
public async Task<int> GetMaxEmployeeIdAsync()
{
    return await _context.Employee.MaxAsync(e => (int?)e.EmployeeID) ?? 0;
}
} 