using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;
using Backend.Models.DTO;

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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound(new Dictionary<string, object>
                {
                    { "message", $"Employee with ID {id} not found" }
                });
            }

            var employeeDto = new EmployeeDto
            {
                EmployeeID = employee.EmployeeID,
                JobTitle = employee.JobTitle,
                Experience = employee.Experience,
                Gender = employee.Gender,
                CompanyID = employee.CompanyID
            };

            return Ok(new Dictionary<string, object>
            {
                { "data", employeeDto }
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new Dictionary<string, object>
            {
                { "error", ex.Message }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while retrieving the employee" },
                { "details", ex.Message }
            });
        }
    }

    [HttpGet("company/{companyId}")]
    public async Task<IActionResult> GetEmployeesByCompanyId(int companyId)
    {
        try
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var filteredEmployees = employees
                .Where(e => e.CompanyID == companyId)
                .Select(e => new EmployeeDto
                {
                    EmployeeID = e.EmployeeID,
                    JobTitle = e.JobTitle,
                    Experience = e.Experience,
                    Gender = e.Gender,
                    CompanyID = e.CompanyID
                })
                .ToList();

            if (!filteredEmployees.Any())
            {
                return NotFound(new Dictionary<string, object>
                {
                    { "message", $"No employees found for CompanyID {companyId}" }
                });
            }

            return Ok(new Dictionary<string, object>
            {
                { "data", filteredEmployees }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while retrieving employees" },
                { "details", ex.Message }
            });
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> BulkUploadEmployees([FromBody] List<EmployeeModel> employees)
    {
        try
        {
            if (employees == null || !employees.Any())
            {
                return BadRequest(new Dictionary<string, object>
                {
                    { "message", "No employees provided" }
                });
            }

            var result = await _employeeService.BulkCreateEmployeesAsync(employees);

            return Ok(new Dictionary<string, object>
            {
                { "message", $"Successfully processed {result.Count} employees" },
                { "processedCount", result.Count }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while processing the bulk upload" },
                { "details", ex.Message }
            });
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeModel updatedEmployee)
    {
        try
        {
            if (updatedEmployee == null || id <= 0)
            {
                return BadRequest(new Dictionary<string, object>
                {
                    { "message", "Invalid employee data or ID" }
                });
            }

            var result = await _employeeService.UpdateEmployeeAsync(id, updatedEmployee);

            return Ok(new Dictionary<string, object>
            {
                { "message", "Employee updated successfully" },
                { "employee", result }
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new Dictionary<string, object>
            {
                { "error", ex.Message }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while updating the employee" },
                { "details", ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new Dictionary<string, object>
                {
                    { "message", "Invalid employee ID" }
                });
            }

            var employee = await _employeeService.DeleteEmployeeAsync(id);

            return Ok(new Dictionary<string, object>
            {
                { "message", "Employee deleted successfully" },
                { "employee", employee }
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new Dictionary<string, object>
            {
                { "error", ex.Message }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while deleting the employee" },
                { "details", ex.Message }
            });
        }
    }

    [HttpPost("generate-sample-data")]
    public async Task<IActionResult> GenerateSampleData()
    {
        try
        {
            var random = new Random();
            var sampleEmployees = new List<EmployeeModel>();

            for (int i = 1; i <= 100; i++)
            {
                sampleEmployees.Add(new EmployeeModel
                {
                    EmployeeID = i,
                    JobTitle = random.Next(0, 2) == 0 ? "Software Engineer" : "Data Analyst",
                    Experience = random.Next(0, 30),
                    Gender = random.Next(0, 2) == 0 ? "Male" : "Female",
                    CompanyID = random.Next(1, 4)
                });
            }

            await _employeeService.BulkCreateEmployeesAsync(sampleEmployees);

            return Ok(new Dictionary<string, object>
            {
                { "message", "Successfully generated 100 sample employees" },
                { "generatedCount", sampleEmployees.Count }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while generating sample data" },
                { "details", ex.Message }
            });
        }
    }

    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllEmployees()
    {
        try
        {
            await _employeeService.DeleteAllEmployeesAsync();
            return Ok(new Dictionary<string, object>
            {
                { "message", "All employees have been deleted successfully." }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while deleting employees" },
                { "details", ex.Message }
            });
        }
    }

    [HttpGet("getAllJobTitles")]
    public async Task<IActionResult> GetAllJobTitles()
    {
        try
        {
            var jobTitles = await _employeeService.GetAllJobTitlesAsync();

            if (!jobTitles.Any())
            {
                return NotFound(new Dictionary<string, object>
                {
                    { "message", "No job titles found" }
                });
            }

            return Ok(new Dictionary<string, object>
            {
                { "data", jobTitles }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while retrieving job titles" },
                { "details", ex.Message }
            });
        }
    }

    [HttpGet("getAllEmployeesBy/{jobTitle}")]
    public async Task<IActionResult> GetEmployeesByJobTitle(string jobTitle)
    {
        try
        {
            var employees = await _employeeService.GetEmployeesByJobTitleAsync(jobTitle);

            if (!employees.Any())
            {
                return NotFound(new Dictionary<string, object>
                {
                    { "message", $"No employees found with job title: {jobTitle}" }
                });
            }

            return Ok(new Dictionary<string, object>
            {
                { "data", employees }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while retrieving employees" },
                { "details", ex.Message }
            });
        }
    }

    [HttpGet("getSalaryDifferences/{jobTitle}")]
    public async Task<IActionResult> GetSalaryDifferencesByGender(string jobTitle)
    {
        try
        {
            var salaryDifferences = await _employeeService.GetSalaryDifferencesByGenderAsync(jobTitle);

            if (!salaryDifferences["Male"].Any() && !salaryDifferences["Female"].Any())
            {
                return NotFound(new Dictionary<string, object>
                {
                    { "message", $"No salary data found for job title: {jobTitle}" }
                });
            }

            return Ok(new Dictionary<string, object>
            {
                { "data", salaryDifferences }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while retrieving salary differences" },
                { "details", ex.Message }
            });
        }
    }

    [HttpGet("getAllSalaryDifferences")]
    public async Task<IActionResult> GetAllSalaryDifferencesByGender()
    {
        try
        {
            var salaryDifferences = await _employeeService.GetAllSalaryDifferencesByGenderAsync();

            if (!salaryDifferences["Male"].Any() && !salaryDifferences["Female"].Any())
            {
                return NotFound(new Dictionary<string, object>
                {
                    { "message", "No salary data found" }
                });
            }

            return Ok(new Dictionary<string, object>
            {
                { "data", salaryDifferences }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Dictionary<string, object>
            {
                { "error", "An error occurred while retrieving salary differences" },
                { "details", ex.Message }
            });
        }
    }
}
