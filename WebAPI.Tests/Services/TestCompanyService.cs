using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.Data;
using Backend.Services;
using Backend.Models;
using NUnit.Framework;
using System.Collections.Generic;

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
    }
}