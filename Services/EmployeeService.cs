using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.Data;

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
            // Add all employees to the database
            await _context.Employee.AddRangeAsync(employees);

            // Save the changes
            await _context.SaveChangesAsync();

            return employees;
        }
        catch (Exception ex)
        {
            // Log the inner exception for more details
            throw new Exception($"Error during bulk employee creation: {ex.InnerException?.Message ?? ex.Message}", ex);
        }
    }

    public async Task DeleteAllEmployeesAsync()
    {
        // Remove all employees from the database
        _context.Employee.RemoveRange(_context.Employee);

        // Save the changes
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetMaxEmployeeIdAsync()
    {
        // Get the highest EmployeeID in the database, or return 0 if the table is empty
        return await _context.Employee.MaxAsync(e => (int?)e.EmployeeID) ?? 0;
    }
}