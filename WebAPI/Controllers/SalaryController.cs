using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Models.DTO;
using Backend.Services;

namespace Backend.Controllers;

[ApiController]
[Route("api/salary")]
public class SalaryController : ControllerBase
{
    private readonly ISalaryService _salaryService;

    public SalaryController(ISalaryService salaryService)
    {
        _salaryService = salaryService;
    }

    // POST: api/salary/add
    [HttpPost("add")]
    public async Task<IActionResult> AddSalary([FromBody] SalaryModel salary)
    {
        if (salary.EmployeeID <= 0 || salary.Salary < 0)
            return BadRequest(new Dictionary<string, object>
            {
                { "message", "Invalid salary or EmployeeID." }
            });

        try
        {
            var result = await _salaryService.AddSalaryAsync(salary);
            return Ok(new Dictionary<string, object>
            {
                { "message", "Salary added successfully." },
                { "data", result }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while adding the salary" },
                { "details", ex.Message }
            });
        }
    }

    // GET: api/salary/latest/{employeeId}
    [HttpGet("latest/{employeeId}")]
    public async Task<IActionResult> GetLatestSalary(int employeeId)
    {
        try
        {
            var latestSalary = await _salaryService.GetLatestSalaryForEmployeeAsync(employeeId);
            if (latestSalary == null)
                return NotFound(new Dictionary<string, object>
                {
                    { "message", $"No salary found for EmployeeID {employeeId}" }
                });

            var dto = new SalaryDto
            {
                SalaryID = latestSalary.SalaryID,
                EmployeeID = latestSalary.EmployeeID,
                Salary = latestSalary.Salary,
                Timestamp = latestSalary.Timestamp
            };
            return Ok(new Dictionary<string, object>
            {
                { "data", dto }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while retrieving the latest salary" },
                { "details", ex.Message }
            });
        }
    }

    // GET: api/salary/employee/{employeeId}
    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetSalaryHistory(int employeeId)
    {
        try
        {
            var salaryHistory = await _salaryService.GetSalaryHistoryAsync(employeeId);
            if (!salaryHistory.Any())
                return NotFound(new Dictionary<string, object>
                {
                    { "message", $"No salary history found for EmployeeID {employeeId}" }
                });

            var dtos = salaryHistory.Select(s => new SalaryDto
            {
                SalaryID = s.SalaryID,
                EmployeeID = s.EmployeeID,
                Salary = s.Salary,
                Timestamp = s.Timestamp
            }).ToList();

            return Ok(new Dictionary<string, object>
            {
                { "data", dtos }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while retrieving salary history" },
                { "details", ex.Message }
            });
        }
    }
}
