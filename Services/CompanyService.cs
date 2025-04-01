using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using System.Collections.Generic;

namespace Backend.Services;

public class CompanyService : ICompanyService
{
    private readonly ApplicationDbContext _dbContext;

    public CompanyService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
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
                CVR = random.Next(10000000, 99999999).ToString() // Random 8-digit CVR number
            });
        }

        // Insert data into the database
        foreach (var company in sampleCompanies)
        {
            var sql = "INSERT INTO Company (CompanyID, CompanyName, CVR) VALUES (@CompanyID, @CompanyName, @CVR)";
            await _dbContext.Database.ExecuteSqlRawAsync(sql,
                new[]
                {
                    new Microsoft.Data.SqlClient.SqlParameter("@CompanyID", company.CompanyID),
                    new Microsoft.Data.SqlClient.SqlParameter("@CompanyName", company.CompanyName),
                    new Microsoft.Data.SqlClient.SqlParameter("@CVR", company.CVR)
                });
        }
    }
}