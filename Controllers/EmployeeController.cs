using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

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

    [HttpGet("all")]
    public async Task<IActionResult> GetAllEmployees()
    {
        try
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving employees", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            return Ok(employee);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the employee", details = ex.Message });
        }
    }

    [HttpGet("company/{companyId}")]
    public async Task<IActionResult> GetEmployeesByCompanyId(int companyId)
    {
        try
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var filteredEmployees = employees.Where(e => e.CompanyID == companyId).ToList();

            if (!filteredEmployees.Any())
            {
                return NotFound(new { message = $"No employees found for CompanyID {companyId}" });
            }

            return Ok(filteredEmployees);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving employees", details = ex.Message });
        }
    }

    [HttpPost("add")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BulkUploadEmployees([FromBody] List<EmployeeModel> employees)
    {
        try
        {
            if (employees == null || !employees.Any())
            {
                return BadRequest("No employees provided");
            }

            // Find the highest existing EmployeeID
            var maxEmployeeId = await _employeeService.GetMaxEmployeeIdAsync();

            // Generate unique EmployeeIDs for the new employees
            int currentId = maxEmployeeId + 1;
            foreach (var employee in employees)
            {
                if (employee.CompanyID <= 0)
                {
                    return BadRequest($"Invalid employee data: CompanyID {employee.CompanyID} is invalid");
                }

                if (string.IsNullOrEmpty(employee.JobTitle) || string.IsNullOrEmpty(employee.Gender))
                {
                    return BadRequest($"Invalid employee data: Missing required fields");
                }

                if (employee.Salary < 0 || employee.Experience < 0)
                {
                    return BadRequest($"Invalid employee data: Invalid salary or experience values");
                }

                // Assign a new EmployeeID if it is not provided
                if (employee.EmployeeID <= 0)
                {
                    employee.EmployeeID = currentId++;
                }
            }

            // Process the bulk upload
            var result = await _employeeService.BulkCreateEmployeesAsync(employees);

            return Ok(new
            {
                message = $"Successfully processed {result.Count} employees",
                processedCount = result.Count
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while processing the bulk upload", details = ex.Message });
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeModel updatedEmployee)
    {
        try
        {
            if (updatedEmployee == null || id <= 0)
            {
                return BadRequest("Invalid employee data or ID");
            }

            if (updatedEmployee.CompanyID <= 0 || string.IsNullOrEmpty(updatedEmployee.JobTitle) || string.IsNullOrEmpty(updatedEmployee.Gender))
            {
                return BadRequest("Invalid employee data: Missing required fields or invalid CompanyID");
            }

            if (updatedEmployee.Salary < 0 || updatedEmployee.Experience < 0)
            {
                return BadRequest("Invalid employee data: Invalid salary or experience values");
            }

            updatedEmployee.EmployeeID = id;

            var result = await _employeeService.UpdateEmployeeAsync(id, updatedEmployee);

            return Ok(new { message = "Employee updated successfully", employee = result });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while updating the employee", details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid employee ID");
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound(new { error = $"Employee with ID {id} not found" });
            }

            // Call the correct method to delete the employee by ID
            await _employeeService.DeleteEmployeeAsync(id);

            return Ok(new { message = "Employee deleted successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while deleting the employee", details = ex.Message });
        }
    }

    [HttpPost("generate-sample-data")]
    public async Task<IActionResult> GenerateSampleData()
    {
        string GetRandomJobTitle(Random random)
        {
            var jobTitles = new[]
            {
                "Software Engineer",
                "Data Analyst",
                "Project Manager",
                "HR Specialist",
                "Marketing Manager",
                "Sales Representative",
                "Product Manager",
                "Business Analyst",
                "DevOps Engineer",
                "UI/UX Designer"
            };

            return jobTitles[random.Next(jobTitles.Length)];
        }

        try
        {
            var random = new Random();
            var sampleEmployees = new List<EmployeeModel>();
            var maxEmployeeId = await _employeeService.GetMaxEmployeeIdAsync();

            for (int i = 1; i <= 100; i++)
            {
                sampleEmployees.Add(new EmployeeModel
                {
                    EmployeeID = maxEmployeeId + i,
                    JobTitle = GetRandomJobTitle(random),
                    Salary = random.Next(30000, 120000),
                    Experience = random.Next(0, 30),
                    Gender = random.Next(0, 2) == 0 ? "Male" : "Female",
                    CompanyID = random.Next(1, 4)
                });
            }

            await _employeeService.BulkCreateEmployeesAsync(sampleEmployees);

            return Ok(new
            {
                message = "Successfully generated 100 sample employees",
                generatedCount = sampleEmployees.Count
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while generating sample data", details = ex.Message });
        }
    }

    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllEmployees()
    {
        try
        {
            await _employeeService.DeleteAllEmployeesAsync();
            return Ok(new { message = "All employees have been deleted successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while deleting employees", details = ex.Message });
        }
    }
}