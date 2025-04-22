using Backend.Models;
using Backend.Services;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Tests.Services
{
    [TestFixture]
    public class TestCompanyService
    {
        private CompanyService _companyService;
        private Mock<ILogger<CompanyService>> _loggerMock;
        private ApplicationDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            // Set up an in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _loggerMock = new Mock<ILogger<CompanyService>>();
            _companyService = new CompanyService(_dbContext, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task GetAllCompaniesAsync_ShouldReturnAllCompanies()
        {
            // Arrange
            var companies = new List<CompanyModel>
            {
                new CompanyModel { CompanyName = "Company A", Industry = "Tech", CVR = "12345678", Email = "a@example.com", PasswordHash = "hash1" },
                new CompanyModel { CompanyName = "Company B", Industry = "Finance", CVR = "87654321", Email = "b@example.com", PasswordHash = "hash2" }
            };
            await _dbContext.Companies.AddRangeAsync(companies);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _companyService.GetAllCompaniesAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(c => c.CompanyName == "Company A"));
            Assert.IsTrue(result.Any(c => c.CompanyName == "Company B"));
        }

        [Test]
        public async Task GetCompanyByIdAsync_ShouldReturnCorrectCompany()
        {
            // Arrange
            var company = new CompanyModel { CompanyID = 1, CompanyName = "Company A", Industry = "Tech", CVR = "12345678", Email = "a@example.com", PasswordHash = "hash1" };
            await _dbContext.Companies.AddAsync(company);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _companyService.GetCompanyByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Company A", result.CompanyName);
        }

        [Test]
        public void GetCompanyByIdAsync_ShouldThrowException_WhenCompanyNotFound()
        {
            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _companyService.GetCompanyByIdAsync(999));
        }

        [Test]
        public async Task CreateCompaniesAsync_ShouldAddCompanies()
        {
            // Arrange
            var companies = new List<CompanyModel>
            {
                new CompanyModel { CompanyName = "Company A", Industry = "Tech", CVR = "12345678", Email = "a@example.com", PasswordHash = "hash1" },
                new CompanyModel { CompanyName = "Company B", Industry = "Finance", CVR = "87654321", Email = "b@example.com", PasswordHash = "hash2" }
            };

            // Act
            await _companyService.CreateCompaniesAsync(companies);

            // Assert
            var result = await _dbContext.Companies.ToListAsync();
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task UpdateCompanyAsync_ShouldUpdateExistingCompany()
        {
            // Arrange
            var company = new CompanyModel { CompanyID = 1, CompanyName = "Company A", Industry = "Tech", CVR = "12345678", Email = "a@example.com", PasswordHash = "hash1" };
            await _dbContext.Companies.AddAsync(company);
            await _dbContext.SaveChangesAsync();

            var updatedCompany = new CompanyModel { CompanyID = 1, CompanyName = "Updated Company A", Industry = "Tech", CVR = "12345678", Email = "updated@example.com", PasswordHash = "hash1" };

            // Act
            await _companyService.UpdateCompanyAsync(updatedCompany);

            // Assert
            var result = await _dbContext.Companies.FindAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Company A", result.CompanyName);
            Assert.AreEqual("updated@example.com", result.Email);
        }

        [Test]
        public async Task DeleteCompanyAsync_ShouldRemoveCompany()
        {
            // Arrange
            var company = new CompanyModel { CompanyID = 1, CompanyName = "Company A", Industry = "Tech", CVR = "12345678", Email = "a@example.com", PasswordHash = "hash1" };
            await _dbContext.Companies.AddAsync(company);
            await _dbContext.SaveChangesAsync();

            // Act
            await _companyService.DeleteCompanyAsync(1);

            // Assert
            var result = await _dbContext.Companies.FindAsync(1);
            Assert.IsNull(result);
        }

        [Test]
        public async Task DeleteAllCompaniesAsync_ShouldRemoveAllCompanies()
        {
            // Arrange
            var companies = new List<CompanyModel>
            {
                new CompanyModel { CompanyName = "Company A", Industry = "Tech", CVR = "12345678", Email = "a@example.com", PasswordHash = "hash1" },
                new CompanyModel { CompanyName = "Company B", Industry = "Finance", CVR = "87654321", Email = "b@example.com", PasswordHash = "hash2" }
            };
            await _dbContext.Companies.AddRangeAsync(companies);
            await _dbContext.SaveChangesAsync();

            // Act
            await _companyService.DeleteAllCompaniesAsync();

            // Assert
            var result = await _dbContext.Companies.ToListAsync();
            Assert.AreEqual(0, result.Count);
        }
    }
}