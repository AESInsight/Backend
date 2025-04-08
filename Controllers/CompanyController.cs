using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;

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
        var companies = await _companyService.GetAllCompaniesAsync();
        return Ok(companies);
    }

    // GET: api/company/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompanyById(int id)
    {
        var company = await _companyService.GetCompanyByIdAsync(id);
        if (company == null)
        {
            return NotFound(new { message = "Company not found" });
        }
        return Ok(company);
    }

    // POST: api/company
    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyModel company)
    {
        if (company == null || string.IsNullOrEmpty(company.CompanyName) || string.IsNullOrEmpty(company.CVR))
        {
            return BadRequest(new { message = "Invalid company data" });
        }

        await _companyService.CreateCompanyAsync(company);
        return CreatedAtAction(nameof(GetCompanyById), new { id = company.CompanyID }, company);
    }

    // PUT: api/company/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany(int id, [FromBody] CompanyModel company)
    {
        if (company == null || id != company.CompanyID)
        {
            return BadRequest(new { message = "Invalid company data" });
        }

        try
        {
            await _companyService.UpdateCompanyAsync(company);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // DELETE: api/company/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(int id)
    {
        var existingCompany = await _companyService.GetCompanyByIdAsync(id);
        if (existingCompany == null)
        {
            return NotFound(new { message = "Company not found" });
        }

        await _companyService.DeleteCompanyAsync(id);
        return NoContent();
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

    // POST: api/company/insert-company
    [HttpPost("insert-company")]
    public async Task<IActionResult> InsertCompany([FromBody] CompanyModel company)
    {
        if (company == null || string.IsNullOrEmpty(company.CompanyName) || string.IsNullOrEmpty(company.CVR) || string.IsNullOrEmpty(company.Email) || string.IsNullOrEmpty(company.PasswordHash) || string.IsNullOrEmpty(company.EmailPassword))
        {
            return BadRequest(new { message = "Invalid company data" });
        }

        await _companyService.InsertCompanyAsync(company.CompanyID, company.CompanyName, company.CVR, company.Email, company.PasswordHash, company.EmailPassword);
        return Ok(new { message = "Company inserted successfully" });
    }

    // POST: api/company/bulk-insert-companies
    [HttpPost("bulk-insert-companies")]
    public async Task<IActionResult> BulkInsertCompanies([FromBody] List<CompanyModel> companies)
    {
        if (companies == null || !companies.Any())
        {
            return BadRequest(new { message = "No companies provided" });
        }

        await _companyService.BulkInsertCompaniesAsync(companies);
        return Ok(new { message = "Companies inserted successfully", count = companies.Count });
    }
}