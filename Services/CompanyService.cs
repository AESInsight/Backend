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
            throw new Exception($"Company with ID {id} not found.");
        }
        return company;
    }

    public async Task<CompanyModel> CreateCompanyAsync(CompanyModel company)
    {
        try
        {
            // Set command timeout to 30 seconds
            _dbContext.Database.SetCommandTimeout(30);
            
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();
            return company;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating company");
            throw;
        }
    }

    public async Task UpdateCompanyAsync(CompanyModel company)
    {
        // Retrieve the existing company from the database
        var existingCompany = await _dbContext.Companies.FirstOrDefaultAsync(c => c.CompanyID == company.CompanyID);

        if (existingCompany == null)
        {
            throw new Exception($"Company with ID {company.CompanyID} not found.");
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

    public async Task BulkCreateCompaniesAsync(List<CompanyModel> companies)
    {
        await _dbContext.Companies.AddRangeAsync(companies);
        await _dbContext.SaveChangesAsync();
    }

    public async Task InsertCompanyAsync(int companyId, string companyName, string cvr, string email, string passwordHash)
    {
        var company = new CompanyModel
        {
            CompanyID = companyId,
            CompanyName = companyName,
            CVR = cvr,
            Email = email,
            PasswordHash = passwordHash
        };

        await CreateCompanyAsync(company);
    }

    public async Task BulkInsertCompaniesAsync(List<CompanyModel> companies)
    {
        foreach (var company in companies)
        {
            await InsertCompanyAsync(company.CompanyID, company.CompanyName, company.CVR, company.Email, company.PasswordHash);
        }
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