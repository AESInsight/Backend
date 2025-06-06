using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.Data;
using Backend.Models.DTO;

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
            await _context.Employee.AddRangeAsync(employees);
            await _context.SaveChangesAsync();
            return employees;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error during employee creation: {ex.InnerException?.Message ?? ex.Message}", ex);
        }
    }

    public async Task DeleteAllEmployeesAsync()
    {
        _context.Employee.RemoveRange(_context.Employee);
        await _context.SaveChangesAsync();
    }

    public Task<List<EmployeeModel>> GetAllEmployeesAsync()
    {
        return _context.Employee.ToListAsync();
    }

    public async Task<EmployeeModel> GetEmployeeByIdAsync(int id)
    {
        var employee = await _context.Employee
            .Include(e => e.Company)
            .FirstOrDefaultAsync(e => e.EmployeeID == id);

        if (employee == null)
            throw new KeyNotFoundException($"Employee with ID {id} was not found.");

        return employee;
    }

    public async Task<int> GetMaxEmployeeIdAsync()
    {
        return await _context.Employee.MaxAsync(e => (int?)e.EmployeeID) ?? 0;
    }

    public async Task<EmployeeModel> UpdateEmployeeAsync(int id, EmployeeModel updatedEmployee)
    {
        var existingEmployee = await _context.Employee.FirstOrDefaultAsync(e => e.EmployeeID == id);

        if (existingEmployee == null)
            throw new KeyNotFoundException($"Employee with ID {id} not found.");

        existingEmployee.JobTitle = updatedEmployee.JobTitle;
        existingEmployee.Experience = updatedEmployee.Experience;
        existingEmployee.Gender = updatedEmployee.Gender;
        existingEmployee.CompanyID = updatedEmployee.CompanyID;

        await _context.SaveChangesAsync();
        return existingEmployee;
    }

    public Task<EmployeeModel> DeleteEmployeeAsync(int id)
    {
        var employee = _context.Employee.Find(id);
        if (employee == null)
            throw new KeyNotFoundException($"Employee with ID {id} not found.");

        _context.Employee.Remove(employee);
        _context.SaveChangesAsync();

        return Task.FromResult(employee);
    }

    public async Task<List<string>> GetAllJobTitlesAsync()
    {
        return await _context.Employee
            .Select(e => e.JobTitle!)
            .Where(title => title != null)
            .Distinct()
            .OrderBy(title => title)
            .ToListAsync();
    }

    public async Task<List<EmployeeModel>> GetEmployeesByJobTitleAsync(string jobTitle)
    {
        return await _context.Employee
            .Where(e => e.JobTitle == jobTitle)
            .Include(e => e.Company)
            .ToListAsync();
    }

    public async Task<Dictionary<string, List<SalaryDifferenceDTO>>> GetSalaryDifferencesByGenderAsync(string jobTitle)
    {
        var result = new Dictionary<string, List<SalaryDifferenceDTO>>();
        result["Male"] = new List<SalaryDifferenceDTO>();
        result["Female"] = new List<SalaryDifferenceDTO>();

        try
        {
            Console.WriteLine($"Getting salary differences for job title: {jobTitle}");

            // Get all relevant employees with their latest salary per month
            var employeeSalaries = await _context.Employee
                .Where(e => e.JobTitle == jobTitle)
                .Join(
                    _context.Salaries,
                    e => e.EmployeeID,
                    s => s.EmployeeID,
                    (e, s) => new
                    {
                        e.EmployeeID,
                        e.Gender,
                        s.Salary,
                        s.Timestamp
                    }
                )
                .OrderByDescending(x => x.Timestamp)
                .GroupBy(x => new
                {
                    x.EmployeeID,
                    Year = x.Timestamp.Year,
                    Month = x.Timestamp.Month
                })
                .Select(g => new
                {
                    g.First().EmployeeID,
                    g.First().Gender,
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Salary = g.First().Salary
                })
                .ToListAsync();

            Console.WriteLine($"Found {employeeSalaries.Count} salary records for job title {jobTitle}");

            if (!employeeSalaries.Any())
            {
                Console.WriteLine("No salary records found for this job title");
                return result;
            }

            // Group by month and gender
            var monthlyGroups = employeeSalaries
                .GroupBy(x => new { x.Month, x.Gender })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Gender = g.Key.Gender,
                    AverageSalary = Math.Round(g.Average(x => x.Salary), 2)
                })
                .OrderBy(x => x.Month)
                .ToList();

            Console.WriteLine($"Found {monthlyGroups.Count} monthly salary groups");

            // Add to result
            foreach (var group in monthlyGroups)
            {
                var dto = new SalaryDifferenceDTO
                {
                    Month = group.Month,
                    AverageSalary = group.AverageSalary
                };

                if (group.Gender == "Male")
                {
                    result["Male"].Add(dto);
                }
                else if (group.Gender == "Female")
                {
                    result["Female"].Add(dto);
                }
            }

            Console.WriteLine($"Male entries: {result["Male"].Count}, Female entries: {result["Female"].Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetSalaryDifferencesByGenderAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }

        return result;
    }

    public async Task<Dictionary<string, List<SalaryDifferenceDTO>>> GetAllSalaryDifferencesByGenderAsync()
    {
        var result = new Dictionary<string, List<SalaryDifferenceDTO>>();
        result["Male"] = new List<SalaryDifferenceDTO>();
        result["Female"] = new List<SalaryDifferenceDTO>();

        try
        {
            Console.WriteLine("Getting all salary differences");

            // Get all employees with their latest salary per month
            var employeeSalaries = await _context.Employee
                .Join(
                    _context.Salaries,
                    e => e.EmployeeID,
                    s => s.EmployeeID,
                    (e, s) => new
                    {
                        e.EmployeeID,
                        e.Gender,
                        s.Salary,
                        s.Timestamp
                    }
                )
                .OrderByDescending(x => x.Timestamp)
                .GroupBy(x => new
                {
                    x.EmployeeID,
                    Year = x.Timestamp.Year,
                    Month = x.Timestamp.Month
                })
                .Select(g => new
                {
                    g.First().EmployeeID,
                    g.First().Gender,
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Salary = g.First().Salary
                })
                .ToListAsync();

            Console.WriteLine($"Found {employeeSalaries.Count} total salary records");

            if (!employeeSalaries.Any())
            {
                Console.WriteLine("No salary records found");
                return result;
            }

            // Group by month and gender
            var monthlyGroups = employeeSalaries
                .GroupBy(x => new { x.Month, x.Gender })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Gender = g.Key.Gender,
                    AverageSalary = Math.Round(g.Average(x => x.Salary), 2)
                })
                .OrderBy(x => x.Month)
                .ToList();

            Console.WriteLine($"Found {monthlyGroups.Count} monthly salary groups");

            // Add to result
            foreach (var group in monthlyGroups)
            {
                var dto = new SalaryDifferenceDTO
                {
                    Month = group.Month,
                    AverageSalary = group.AverageSalary
                };

                if (group.Gender == "Male")
                {
                    result["Male"].Add(dto);
                }
                else if (group.Gender == "Female")
                {
                    result["Female"].Add(dto);
                }
            }

            Console.WriteLine($"Male entries: {result["Male"].Count}, Female entries: {result["Female"].Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllSalaryDifferencesByGenderAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }

        return result;
    }

    public async Task<EmployeeDto?> GetEmployeeIndustryByIdAsync(int id)
    {
        var employee = await _context.Employee
            .Include(e => e.Company)
            .FirstOrDefaultAsync(e => e.EmployeeID == id);

        if (employee == null || employee.Company == null)
            return null;

        return new EmployeeDto
        {
            EmployeeID = employee.EmployeeID,
            CompanyID = employee.CompanyID,
            Industry = employee.Company.Industry
        };
    }

    public async Task<List<EmployeeModel>> GetEmployeesByCompanyIdAsync(int companyId)
    {
        return await _context.Employee
            .Where(e => e.CompanyID == companyId)
            .Include(e => e.Company)
            .ToListAsync();
    }
}
