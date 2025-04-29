using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.Data;
using Backend.Services;
using Backend.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Tests.Services
{
    [TestFixture]
    public class TestCompanyService
    {
        private Mock<ILogger<CompanyService>> _loggerMock;
        private CompanyService _companyService;

        [SetUp]
        public void SetUp()
        {
            // Use a unique InMemoryDatabase for each test
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new ApplicationDbContext(options);

            // Seed the database with test data
            dbContext.Companies.AddRange(new List<CompanyModel>
            {
                new CompanyModel 
                { 
                    CompanyID = 1, 
                    CompanyName = "Company A", 
                    CVR = "12345678", 
                    Email = "a@example.com", 
                    Industry = "Tech", 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password1")
                },
                new CompanyModel 
                { 
                    CompanyID = 2, 
                    CompanyName = "Company B", 
                    CVR = "87654321", 
                    Email = "b@example.com", 
                    Industry = "Finance", 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password2")
                }
            });
            dbContext.SaveChanges();

            // Mock the logger
            _loggerMock = new Mock<ILogger<CompanyService>>();

            // Initialize the service
            _companyService = new CompanyService(dbContext, _loggerMock.Object);
        }

        [Test]
        public async Task GetAllCompaniesAsync_ReturnsAllCompanies()
        {
            // Act
            var result = await _companyService.GetAllCompaniesAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].CompanyName, Is.EqualTo("Company A"));
            Assert.That(result[1].CompanyName, Is.EqualTo("Company B"));
            Assert.That(result[0].CVR, Is.EqualTo("12345678"));
            Assert.That(result[1].CVR, Is.EqualTo("87654321"));
        }

        [Test]
        public async Task GetCompanyByIdAsync_ReturnsCorrectCompany()
        {
            // Act
            var result = await _companyService.GetCompanyByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CompanyName, Is.EqualTo("Company A"));
            Assert.That(result.CVR, Is.EqualTo("12345678"));
        }

        [Test]
        public async Task GetCompanyByEmailAsync_ReturnsCorrectCompany()
        {
            // Act
            var result = await _companyService.GetCompanyByEmailAsync("a@example.com");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CompanyName, Is.EqualTo("Company A"));
            Assert.That(result.CVR, Is.EqualTo("12345678"));
        }

        [Test]
        public async Task GetCompanyByEmailAsync_ReturnsNullForNonExistentEmail()
        {
            // Act
            var result = await _companyService.GetCompanyByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task VerifyPasswordAsync_ReturnsTrueForValidPassword()
        {
            // Act
            var result = await _companyService.VerifyPasswordAsync("a@example.com", "password1");

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task VerifyPasswordAsync_ReturnsFalseForInvalidPassword()
        {
            // Act
            var result = await _companyService.VerifyPasswordAsync("a@example.com", "wrongpassword");

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteCompanyAsync_RemovesCompany()
        {
            // Act
            await _companyService.DeleteCompanyAsync(1);
            var companies = await _companyService.GetAllCompaniesAsync();

            // Assert
            Assert.That(companies.Any(c => c.CompanyID == 1), Is.False); // Ensure the company is no longer in the list
        }

        [Test]
        public async Task DeleteCompanyAsync_DoesNothingForNonExistentCompany()
        {
            // Act
            await _companyService.DeleteCompanyAsync(999); // Non-existent ID

            // Assert
            var companies = await _companyService.GetAllCompaniesAsync();
            Assert.That(companies.Count, Is.EqualTo(2)); // Ensure no companies were removed
        }

        [Test]
        public async Task UpdateCompanyAsync_UpdatesCompanyDetails()
        {
            // Arrange
            var updatedCompany = new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Updated Company A",
                CVR = "87654321",
                Email = "updated@example.com",
                Industry = "Updated Industry",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("updatedpassword")
            };

            // Act
            await _companyService.UpdateCompanyAsync(updatedCompany);
            var result = await _companyService.GetCompanyByIdAsync(1);

            // Assert
            Assert.That(result.CompanyName, Is.EqualTo("Updated Company A"));
            Assert.That(result.CVR, Is.EqualTo("87654321"));
            Assert.That(result.Email, Is.EqualTo("updated@example.com"));
            Assert.That(result.Industry, Is.EqualTo("Updated Industry"));
        }

        [Test]
        public async Task GetAllIndustriesAsync_ReturnsUniqueIndustries()
        {
            // Act
            var industries = await _companyService.GetAllIndustriesAsync();

            // Assert
            Assert.That(industries.Count, Is.EqualTo(2));
            Assert.That(industries, Does.Contain("Tech"));
            Assert.That(industries, Does.Contain("Finance"));
        }

        [Test]
        public async Task CreateCompaniesAsync_AddsNewCompanies()
        {
            // Arrange
            var newCompanies = new List<CompanyModel>
            {
                new CompanyModel
                {
                    CompanyName = "Company C",
                    CVR = "11223344",
                    Email = "c@example.com",
                    Industry = "Healthcare",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password3")
                },
                new CompanyModel
                {
                    CompanyName = "Company D",
                    CVR = "55667788",
                    Email = "d@example.com",
                    Industry = "Retail",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password4")
                }
            };

            // Act
            await _companyService.CreateCompaniesAsync(newCompanies);
            var companies = await _companyService.GetAllCompaniesAsync();

            // Assert
            Assert.That(companies.Count, Is.EqualTo(4)); // 2 existing + 2 new
            Assert.That(companies.Any(c => c.CompanyName == "Company C"), Is.True);
            Assert.That(companies.Any(c => c.CompanyName == "Company D"), Is.True);
        }

        [Test]
        public async Task DeleteAllCompaniesAsync_RemovesAllCompanies()
        {
            // Act
            await _companyService.DeleteAllCompaniesAsync();
            var companies = await _companyService.GetAllCompaniesAsync();

            // Assert
            Assert.That(companies.Count, Is.EqualTo(0));
        }
    }
}