using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;
using System.Text.Json;

namespace Backend.Controllers;

[ApiController]
[Route("api/employee")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpPost("bulk-upload")]
    public async Task<IActionResult> BulkUploadEmployees([FromBody] List<EmployeeModel> employees)
    {
        try
        {
            if (employees == null || !employees.Any())
            {
                return BadRequest("No employees provided");
            }

            // Validate each employee
            foreach (var employee in employees)
            {
                if (string.IsNullOrEmpty(employee.EmployeeID) ||
                    string.IsNullOrEmpty(employee.JobTitle) ||
                    string.IsNullOrEmpty(employee.Gender) ||
                    string.IsNullOrEmpty(employee.CompanyID))
                {
                    return BadRequest($"Invalid employee data: EmployeeID {employee.EmployeeID} is missing required fields");
                }

                if (employee.Salary < 0 || employee.Experience < 0)
                {
                    return BadRequest($"Invalid employee data: EmployeeID {employee.EmployeeID} has invalid salary or experience values");
                }
            }

            // Process the bulk upload
            var result = await _employeeService.BulkCreateEmployeesAsync(employees);
            
            return Ok(new { 
                message = $"Successfully processed {result.Count} employees",
                processedCount = result.Count
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while processing the bulk upload", details = ex.Message });
        }
    }
} 