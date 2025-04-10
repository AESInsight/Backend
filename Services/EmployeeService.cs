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
            throw new Exception($"Error during employee creation: {ex.InnerException?.Message ?? ex.Message}", ex);
        }
    }

    public async Task DeleteAllEmployeesAsync()
    {
        // Remove all employees from the database
        _context.Employee.RemoveRange(_context.Employee);

        // Save the changes
        await _context.SaveChangesAsync();
    }

    public Task<List<EmployeeModel>> GetAllEmployeesAsync()
    {
        return _context.Employee.ToListAsync();
    }
    

    public async Task<EmployeeModel> GetEmployeeByIdAsync(int id)
    {
        var employee = await _context.Employee
            .Include(e => e.Company) // Include related Company data
            .FirstOrDefaultAsync(e => e.EmployeeID == id);

        if (employee == null)
        {
            throw new KeyNotFoundException($"Employee with ID {id} was not found.");
        }

        return employee;
    }

    public async Task<int> GetMaxEmployeeIdAsync()
    {
        // Get the highest EmployeeID in the database, or return 0 if the table is empty
        return await _context.Employee.MaxAsync(e => (int?)e.EmployeeID) ?? 0;
    }

    public async Task<EmployeeModel> UpdateEmployeeAsync(int id, EmployeeModel updatedEmployee)
    {
        var existingEmployee = await _context.Employee.FirstOrDefaultAsync(e => e.EmployeeID == id);

        if (existingEmployee == null)
        {
            throw new KeyNotFoundException($"Employee with ID {id} not found.");
        }

        // Update the fields of the existing employee
        existingEmployee.JobTitle = updatedEmployee.JobTitle;
        existingEmployee.Salary = updatedEmployee.Salary;
        existingEmployee.Experience = updatedEmployee.Experience;
        existingEmployee.Gender = updatedEmployee.Gender;
        existingEmployee.CompanyID = updatedEmployee.CompanyID;

        // Save changes to the database
        await _context.SaveChangesAsync();

        return existingEmployee;
    }
        public Task<EmployeeModel> DeleteEmployeeAsync(int id)
    {
        var employee = _context.Employee.Find(id);
        if (employee != null)
        {
            _context.Employee.Remove(employee);
            _context.SaveChangesAsync();
        }
        if (employee == null)
        {
            throw new KeyNotFoundException($"Employee with ID {id} not found.");
        }
        return Task.FromResult(employee);
    }
}