using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;
using Backend.Models.DTO;

namespace Backend.Controllers;

[ApiController]
[Route("api/company")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
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
                CVR = c.CVR,
                Email = c.Email,
                PasswordHash = c.PasswordHash // Include PasswordHash in the mapping
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
            var companyDTO = new CompanyDTO
            {
                CompanyID = company.CompanyID,
                CompanyName = company.CompanyName,
                CVR = company.CVR,
                Email = company.Email,
                PasswordHash = company.PasswordHash // Include PasswordHash in the mapping
            };

            return Ok(companyDTO);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
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
                Email = dto.Email,
                PasswordHash = dto.PasswordHash 
            }).ToList();

            await _companyService.CreateCompaniesAsync(companies);
            return Ok(new { message = "Companies inserted successfully", count = companies.Count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while inserting companies", details = ex.Message });
        }
    }

    // PUT: api/company/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany(int id, [FromBody] CompanyModel company)
    {
        try
        {
            if (company == null || id != company.CompanyID)
            {
                return BadRequest(new { message = "Invalid company data" });
            }

            await _companyService.UpdateCompanyAsync(company);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
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

            await _companyService.DeleteCompanyAsync(id);
            return NoContent();
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
}