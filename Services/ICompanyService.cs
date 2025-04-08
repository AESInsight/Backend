using Backend.Models;

namespace Backend.Services;

public interface ICompanyService
{
    Task<List<CompanyModel>> GetAllCompaniesAsync(); // Retrieve all companies
    Task<CompanyModel> GetCompanyByIdAsync(int id); // Retrieve a company by ID
    Task<CompanyModel> CreateCompanyAsync(CompanyModel company); // Create a new company
    Task UpdateCompanyAsync(CompanyModel company); // Update an existing company
    Task DeleteCompanyAsync(int id); // Delete a company by ID
    Task BulkCreateCompaniesAsync(List<CompanyModel> companies); // Batch-create companies
    Task InsertCompanyAsync(int companyId, string companyName, string cvr, string email, string passwordHash); // Insert a single company
    Task BulkInsertCompaniesAsync(List<CompanyModel> companies); // Batch-insert companies
    Task GenerateSampleCompaniesAsync(); // Generate sample companies
    Task<CompanyModel?> GetCompanyByEmailAsync(string email);
    Task<bool> VerifyPasswordAsync(string email, string password);
}