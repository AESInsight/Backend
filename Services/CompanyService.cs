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
        // Retrieve the existing company from the database
        var existingCompany = await _dbContext.Companies.FirstOrDefaultAsync(c => c.CompanyID == company.CompanyID);

        if (existingCompany == null)
        {
            throw new KeyNotFoundException($"Company with ID {company.CompanyID} not found.");
        }

        if (string.IsNullOrEmpty(company.CVR) || company.CVR.Length != 8 || !company.CVR.All(char.IsDigit))
        {
            throw new ArgumentException("CVR must be exactly 8 digits.");
        }

        // Update the values of the existing company
        existingCompany.CompanyName = company.CompanyName;
        existingCompany.CVR = company.CVR;
        existingCompany.Email = company.Email;
        
        // Only update the password if a new one is provided
        if (!string.IsNullOrEmpty(company.PasswordHash))
        {
            existingCompany.PasswordHash = company.PasswordHash;
        }

        // Save the changes
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
        foreach (var company in companies)
        {
            // Validate CVR
            if (string.IsNullOrEmpty(company.CVR) || company.CVR.Length != 8 || !company.CVR.All(char.IsDigit))
            {
                throw new ArgumentException($"CVR for company '{company.CompanyName}' must be exactly 8 digits.");
            }
        }

        await _dbContext.Companies.AddRangeAsync(companies);
        await _dbContext.SaveChangesAsync();
    }

    public async Task GenerateSampleCompaniesAsync()
    {
        var random = new Random();
        var sampleCompanies = new List<CompanyModel>();

        // Generate 10 random companies
        for (int i = 1; i <= 3; i++)
        {
            sampleCompanies.Add(new CompanyModel
            {
                CompanyID = i, // 1, 2, 3, etc.
                CompanyName = $"Company {i}",
                CVR = random.Next(10000000, 99999999).ToString(), // Random 8-digit CVR number
                Email = $"company{i}@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123")
            });
        }

        // Insert data into the database
        foreach (var company in sampleCompanies)
        {
            var sql = "INSERT INTO Company (CompanyID, CompanyName, CVR, Email, PasswordHash) VALUES (@CompanyID, @CompanyName, @CVR, @Email, @PasswordHash)";
            await _dbContext.Database.ExecuteSqlRawAsync(sql,
                new[]
                {
                    new Microsoft.Data.SqlClient.SqlParameter("@CompanyID", company.CompanyID),
                    new Microsoft.Data.SqlClient.SqlParameter("@CompanyName", company.CompanyName),
                    new Microsoft.Data.SqlClient.SqlParameter("@CVR", company.CVR),
                    new Microsoft.Data.SqlClient.SqlParameter("@Email", company.Email),
                    new Microsoft.Data.SqlClient.SqlParameter("@PasswordHash", company.PasswordHash)
                });
        }
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