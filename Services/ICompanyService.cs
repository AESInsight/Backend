using Backend.Models;

namespace Backend.Services;

public interface ICompanyService
{
    Task<List<CompanyModel>> GetAllCompaniesAsync(); // Hent alle virksomheder
    Task<CompanyModel> GetCompanyByIdAsync(int id); // Hent en virksomhed baseret på ID
    Task CreateCompanyAsync(CompanyModel company); // Opret en ny virksomhed
    Task UpdateCompanyAsync(CompanyModel company); // Opdater en eksisterende virksomhed
    Task DeleteCompanyAsync(int id); // Slet en virksomhed baseret på ID
    Task BulkCreateCompaniesAsync(List<CompanyModel> companies); // Batch-opret virksomheder
    Task InsertCompanyAsync(int companyId, string companyName, string cvr); // Tilføj denne
    Task BulkInsertCompaniesAsync(List<CompanyModel> companies); // Tilføj denne
    Task GenerateSampleCompaniesAsync(); // Tilføj denne
}