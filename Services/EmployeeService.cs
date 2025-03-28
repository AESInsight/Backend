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
            // Add all employees to the context
            await _context.Employees.AddRangeAsync(employees);
            
            // Save changes to the database
            await _context.SaveChangesAsync();
            
            return employees;
        }
        catch (Exception ex)
        {
            // Log the error and rethrow
            throw new Exception($"Error during bulk employee creation: {ex.Message}", ex);
        }
    }
} 