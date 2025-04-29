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
            // Use an InMemoryDatabase for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
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
                    PasswordHash = "hashedpassword1" 
                },
                new CompanyModel 
                { 
                    CompanyID = 2, 
                    CompanyName = "Company B", 
                    CVR = "87654321", 
                    Email = "b@example.com", 
                    Industry = "Finance", 
                    PasswordHash = "hashedpassword2" 
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
            Assert.That(result[0].PasswordHash, Is.EqualTo("hashedpassword1"));
            Assert.That(result[1].PasswordHash, Is.EqualTo("hashedpassword2"));
        }
    }
}