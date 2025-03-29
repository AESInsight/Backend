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
        return await _dbContext.Companies.FindAsync(id);
    }

    public async Task CreateCompanyAsync(CompanyModel company)
    {
        await _dbContext.Companies.AddAsync(company);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCompanyAsync(CompanyModel company)
    {
        // Hent den eksisterende virksomhed fra databasen
        var existingCompany = await _dbContext.Companies.FirstOrDefaultAsync(c => c.CompanyID == company.CompanyID);

        if (existingCompany == null)
        {
            throw new Exception($"Company with ID {company.CompanyID} not found.");
        }

        // Opdater værdierne på den eksisterende virksomhed
        existingCompany.CompanyName = company.CompanyName;
        existingCompany.CVR = company.CVR;

        // Gem ændringerne
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

    public async Task InsertCompanyAsync(int companyId, string companyName, string cvr)
    {
        var sql = "INSERT INTO Company (CompanyID, CompanyName, CVR) VALUES (@CompanyID, @CompanyName, @CVR)";
        await _dbContext.Database.ExecuteSqlRawAsync(sql, 
            new[] 
            {
                new Microsoft.Data.SqlClient.SqlParameter("@CompanyID", companyId),
                new Microsoft.Data.SqlClient.SqlParameter("@CompanyName", companyName),
                new Microsoft.Data.SqlClient.SqlParameter("@CVR", cvr)
            });
    }

    public async Task BulkInsertCompaniesAsync(List<CompanyModel> companies)
    {
        foreach (var company in companies)
        {
            await InsertCompanyAsync(company.CompanyID, company.CompanyName, company.CVR);
        }
    }

    public async Task GenerateSampleCompaniesAsync()
    {
        var random = new Random();
        var sampleCompanies = new List<CompanyModel>();

        // Generer 10 tilfældige virksomheder
        for (int i = 1; i <= 3; i++)
        {
            sampleCompanies.Add(new CompanyModel
            {
                CompanyID = i, // 1, 2, 3, osv.
                CompanyName = $"Company {i}",
                CVR = random.Next(10000000, 99999999).ToString() // Tilfældig 8-cifret CVR-nummer
            });
        }

        // Indsæt data i databasen
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