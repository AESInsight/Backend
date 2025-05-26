using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Models;
using Backend.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/salary")]
public class SalaryController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public SalaryController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // POST: api/salary/add
    [HttpPost("add")]
    public async Task<IActionResult> AddSalary([FromBody] SalaryModel salary)
    {
        if (salary.EmployeeID <= 0 || salary.Salary < 0)
        {
            return BadRequest("Invalid salary or EmployeeID.");
        }

        // If no Timestamp is provided, set it to the current time
        if (salary.Timestamp == default)
        {
            salary.Timestamp = DateTime.UtcNow;
        }

        _dbContext.Salaries.Add(salary);
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "Salary added successfully." });
    }

    // GET: api/salary/latest/{employeeId}
    [HttpGet("latest/{employeeId}")]
    public async Task<IActionResult> GetLatestSalary(int employeeId)
    {
        var latestSalary = await _dbContext.Salaries
            .Where(s => s.EmployeeID == employeeId)
            .OrderByDescending(s => s.Timestamp)
            .FirstOrDefaultAsync();

        if (latestSalary == null)
        {
            return NotFound(new { message = $"No salary found for EmployeeID {employeeId}" });
        }

        var dto = new SalaryDto
        {
            SalaryID = latestSalary.SalaryID,
            EmployeeID = latestSalary.EmployeeID,
            Salary = latestSalary.Salary,
            Timestamp = latestSalary.Timestamp
        };

        return Ok(dto);
    }

    // GET: api/salary/employee/{employeeId}
    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetSalaryHistory(int employeeId)
    {
        var salaryHistory = await _dbContext.Salaries
            .Where(s => s.EmployeeID == employeeId)
            .OrderByDescending(s => s.Timestamp)
            .Select(s => new SalaryDto
            {
                SalaryID = s.SalaryID,
                EmployeeID = s.EmployeeID,
                Salary = s.Salary,
                Timestamp = s.Timestamp
            })
            .ToListAsync();

        if (!salaryHistory.Any())
        {
            return NotFound(new { message = $"No salary history found for EmployeeID {employeeId}" });
        }

        return Ok(salaryHistory);
    }
        // GET: api/salary/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAllSalaries()
    {
        var salaries = await _dbContext.Salaries
            .Select(s => new
            {
                s.SalaryID,
                s.EmployeeID,
                s.Salary,
                s.Timestamp
            })
            .ToListAsync();

        return Ok(salaries);
    }

    // PUT: api/salary/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSalary(int id, [FromBody] SalaryModel updatedSalary)
    {
        if (id <= 0 || updatedSalary.Salary < 0)
        {
            return BadRequest("Invalid salary ID or amount.");
        }

        var existingSalary = await _dbContext.Salaries.FindAsync(id);
        if (existingSalary == null)
        {
            return NotFound(new { message = $"Salary with ID {id} not found." });
        }

        existingSalary.Salary = updatedSalary.Salary;
        existingSalary.Timestamp = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "Salary updated successfully." });
    }

    // DELETE: api/salary/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSalary(int id)
    {
        var salary = await _dbContext.Salaries.FindAsync(id);
        if (salary == null)
        {
            return NotFound(new { message = $"Salary with ID {id} not found." });
        }

        _dbContext.Salaries.Remove(salary);
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "Salary deleted successfully." });
    }
}
