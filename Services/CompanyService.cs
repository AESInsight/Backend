using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Backend.Services;

public class CompanyService : ICompanyService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<CompanyService> _logger;

    public CompanyService(ApplicationDbContext dbContext, ILogger<CompanyService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<CompanyModel>> GetAllCompaniesAsync()
    {
        return await _dbContext.Companies.ToListAsync();
    }

    public async Task<CompanyModel> GetCompanyByIdAsync(int id)
    {
        var company = await _dbContext.Companies.FindAsync(id);
        if (company == null)
        {
            throw new KeyNotFoundException($"Company with ID {id} not found.");
        }
        return company;
    }

    public async Task UpdateCompanyAsync(CompanyModel company)
    {
        var existingCompany = await _dbContext.Companies.FirstOrDefaultAsync(c => c.CompanyID == company.CompanyID);

        if (existingCompany == null)
        {
            throw new KeyNotFoundException($"Company with ID {company.CompanyID} not found.");
        }

        if (string.IsNullOrEmpty(company.CVR) || company.CVR.Length != 8 || !company.CVR.All(char.IsDigit))
        {
            throw new ArgumentException("CVR must be exactly 8 digits.");
        }

        existingCompany.CompanyName = company.CompanyName;
        existingCompany.CVR = company.CVR;
        existingCompany.Email = company.Email;

        if (!string.IsNullOrEmpty(company.PasswordHash))
        {
            existingCompany.PasswordHash = company.PasswordHash;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteCompanyAsync(int id)
    {
        var company = await _dbContext.Companies.FindAsync(id);
        if (company != null)
        {
            _dbContext.Companies.Remove(company);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteAllCompaniesAsync()
    {
        var companies = await _dbContext.Companies.ToListAsync();
        _dbContext.Companies.RemoveRange(companies);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateCompaniesAsync(List<CompanyModel> companies)
    {
        try
        {
            foreach (var company in companies)
            {
                if (string.IsNullOrEmpty(company.CVR) || company.CVR.Length != 8 || !company.CVR.All(char.IsDigit))
                {
                    throw new ArgumentException($"CVR for company '{company.CompanyName}' must be exactly 8 digits.");
                }

                if (string.IsNullOrEmpty(company.Email) || !company.Email.Contains("@"))
                {
                    throw new ArgumentException($"Email for company '{company.CompanyName}' must be a valid email address.");
                }

                if (string.IsNullOrEmpty(company.PasswordHash))
                {
                    throw new ArgumentException($"Password for company '{company.CompanyName}' cannot be empty.");
                }

                if (!company.PasswordHash.StartsWith("$2"))
                {
                    company.PasswordHash = BCrypt.Net.BCrypt.HashPassword(company.PasswordHash);
                }

                var existingCompany = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Email == company.Email);
                if (existingCompany != null)
                {
                    throw new ArgumentException($"A company with email '{company.Email}' already exists.");
                }

                // Assign the next available ID
                var maxId = await _dbContext.Companies.MaxAsync(c => (int?)c.CompanyID) ?? 0;
                company.CompanyID = maxId + 1;

                _dbContext.Companies.Add(company);
            }

            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError($"Database error while creating companies: {ex.Message}");
            _logger.LogError($"Inner exception: {ex.InnerException?.Message}");
            throw new Exception($"Database error while creating companies: {ex.InnerException?.Message ?? ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while creating companies: {ex.Message}");
            throw;
        }
    }

    public async Task GenerateSampleCompaniesAsync()
    {
        var random = new Random();
        var sampleCompanies = new List<CompanyModel>();

        for (int i = 1; i <= 3; i++)
        {
            sampleCompanies.Add(new CompanyModel
            {
                CompanyName = $"Company {i}",
                CVR = random.Next(10000000, 99999999).ToString(),
                Email = $"company{i}@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123")
            });
        }

        await CreateCompaniesAsync(sampleCompanies);
    }

    public async Task<CompanyModel?> GetCompanyByEmailAsync(string email)
    {
        return await _dbContext.Companies.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<bool> VerifyPasswordAsync(string email, string password)
    {
        var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Email == email);
        if (company == null)
        {
            return false;
        }

        return BCrypt.Net.BCrypt.Verify(password, company.PasswordHash);
    }
}