using Backend.Models;
using Backend.Models.DTO;

namespace Backend.Services;

public interface ICompanyService
{
    Task<List<CompanyModel>> GetAllCompaniesAsync(); // Retrieve all companies
    Task<CompanyModel?> GetCompanyByIdAsync(int id); // Retrieve a company by ID
    Task CreateCompaniesAsync(List<CompanyModel> companies); // Batch-insert companies
    Task UpdateCompanyAsync(CompanyModel company); // Update an existing company
    Task DeleteCompanyAsync(int id); // Delete a company by ID
    Task DeleteAllCompaniesAsync(); // Delete all companies
    Task GenerateSampleCompaniesAsync(); // Generate sample companies
    Task<CompanyModel?> GetCompanyByEmailAsync(string email);
    Task<bool> VerifyPasswordAsync(string email, string password);
    Task<List<string>> GetAllIndustriesAsync(); // Get all unique industries
    Task<List<JobTitleSalaryDTO>> GetAverageSalariesForJobsInIndustryAsync(string industry);
}