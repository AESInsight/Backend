using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;
using Backend.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using System.Diagnostics.CodeAnalysis;

namespace Backend.Controllers;

[ApiController]
[Route("api/company")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ApplicationDbContext _dbContext;

    public CompanyController(ICompanyService companyService, ApplicationDbContext dbContext)
    {
        _companyService = companyService;
        _dbContext = dbContext;
    }

    // GET: api/company
    [HttpGet]
    public async Task<IActionResult> GetAllCompanies()
    {
        try
        {
            var companies = await _companyService.GetAllCompaniesAsync();
            var companyDTOs = companies.Select(c => new CompanyDTO
            {
                CompanyID = c.CompanyID,
                CompanyName = c.CompanyName,
                Industry = c.Industry,
                CVR = c.CVR,
                Email = c.Email
            }).ToList();

            return Ok(companyDTOs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving companies", details = ex.Message });
        }
    }

    // GET: api/company/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompanyById(int id)
    {
        try
        {
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
                return NotFound(new { error = $"Company with ID {id} not found." });

            var companyDTO = new CompanyDTO
            {
                CompanyID = company.CompanyID,
                CompanyName = company.CompanyName,
                Industry = company.Industry,
                CVR = company.CVR,
                Email = company.Email
            };

            return Ok(companyDTO);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the company", details = ex.Message });
        }
    }

    // POST: api/company
    [HttpPost]
    public async Task<IActionResult> InsertCompanies([FromBody] List<CompanyDTO> companyDTOs)
    {
        try
        {
            if (companyDTOs == null || !companyDTOs.Any())
            {
                return BadRequest(new { message = "No companies provided" });
            }

            var companies = companyDTOs.Select(dto => new CompanyModel
            {
                CompanyName = dto.CompanyName,
                Industry = dto.Industry,
                CVR = dto.CVR,
                Email = dto.Email
            }).ToList();

            await _companyService.CreateCompaniesAsync(companies);
            return Ok(new { message = "Companies inserted successfully", count = companies.Count });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while inserting companies", details = ex.Message });
        }
    }

    // PUT: api/company/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany(int id, [FromBody] CompanyDTO companyDTO)
    {
        try
        {
            if (companyDTO == null || id != companyDTO.CompanyID)
            {
                return BadRequest(new { message = "Invalid company data" });
            }

            var company = new CompanyModel
            {
                CompanyID = companyDTO.CompanyID,
                CompanyName = companyDTO.CompanyName,
                Industry = companyDTO.Industry,
                CVR = companyDTO.CVR,
                Email = companyDTO.Email
            };

            await _companyService.UpdateCompanyAsync(company);
            return Ok(new { message = "Company updated successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while updating the company", details = ex.Message });
        }
    }

    // DELETE: api/company/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(int id)
    {
        try
        {
            var existingCompany = await _companyService.GetCompanyByIdAsync(id);
            if (existingCompany == null)
            {
                return NotFound(new { message = "Company not found" });
            }

            // Check if company has employees
            var hasEmployees = await _dbContext.Employee.AnyAsync(e => e.CompanyID == id);
            if (hasEmployees)
            {
                return BadRequest(new { message = "Cannot delete company with associated employees" });
            }

            await _companyService.DeleteCompanyAsync(id);
            return Ok(new { message = "Company deleted successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while deleting the company", details = ex.Message });
        }
    }

    // POST: api/company/generate-sample-companies
    [HttpPost("generate-sample-companies")]
    [ExcludeFromCodeCoverage]
    public async Task<IActionResult> GenerateSampleCompanies()
    {
        try
        {
            await _companyService.GenerateSampleCompaniesAsync();
            return Ok(new { message = "Sample companies generated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while generating sample companies", details = ex.Message });
        }
    }

    // DELETE: api/company/delete-all
    [HttpDelete("delete-all")]
    [ExcludeFromCodeCoverage]
    public async Task<IActionResult> DeleteAllCompanies()
    {
        try
        {
            await _companyService.DeleteAllCompaniesAsync();
            return Ok(new { message = "All companies deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while deleting all companies", details = ex.Message });
        }
    }

    // GET: api/company/getAllIndustries
    [HttpGet("getAllIndustries")]
    public async Task<IActionResult> GetAllIndustries()
    {
        try
        {
            var industries = await _companyService.GetAllIndustriesAsync();
            return Ok(industries);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving industries", details = ex.Message });
        }
    }

    // GET: api/company/getAverageSalaryForJobsIn{industry}
    [HttpGet("getAverageSalaryForJobsIn{industry}")]
    public async Task<IActionResult> GetAverageSalariesForJobsInIndustry(string industry)
    {
        try
        {
            var averageSalaries = await _companyService.GetAverageSalariesForJobsInIndustryAsync(industry);
            return Ok(averageSalaries);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving average salaries", details = ex.Message });
        }
    }
}