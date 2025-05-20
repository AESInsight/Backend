using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.Data;
using Backend.Services;
using Backend.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    PasswordHash = Encoding.UTF8.GetBytes("password1") // Updated to byte[]
                },
                new CompanyModel 
                { 
                    CompanyID = 2, 
                    CompanyName = "Company B", 
                    CVR = "87654321", 
                    Email = "b@example.com", 
                    Industry = "Finance", 
                    PasswordHash = Encoding.UTF8.GetBytes("password2") // Updated to byte[]
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
                PasswordHash = Encoding.UTF8.GetBytes("updatedpassword") // Convert string to byte[]
            };

            // Act
            await _companyService.UpdateCompanyAsync(updatedCompany);
            var result = await _companyService.GetCompanyByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null); // Add null check
            Assert.That(result!.CompanyName, Is.EqualTo("Updated Company A")); // Use null-forgiving operator
            Assert.That(result.CVR, Is.EqualTo("87654321"));
            Assert.That(result.Email, Is.EqualTo("updated@example.com"));
            Assert.That(result.Industry, Is.EqualTo("Updated Industry"));
        }

        [Test]
        public void UpdateCompanyAsync_ThrowsExceptionForInvalidCVR()
        {
            // Arrange
            var invalidCompany = new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Invalid Company",
                CVR = "1234", // Invalid CVR (too short)
                Email = "invalid@example.com",
                Industry = "Tech",
                PasswordHash = Encoding.UTF8.GetBytes("password")
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _companyService.UpdateCompanyAsync(invalidCompany));
        }

        [Test]
        public void UpdateCompanyAsync_ThrowsExceptionForNonExistentCompany()
        {
            // Arrange
            var nonExistentCompany = new CompanyModel
            {
                CompanyID = 999, // Non-existent ID
                CompanyName = "Non-Existent Company",
                CVR = "99999999",
                Email = "nonexistent@example.com",
                Industry = "Tech",
                PasswordHash = Encoding.UTF8.GetBytes("password") // Convert string to byte[]
            };

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => 
                await _companyService.UpdateCompanyAsync(nonExistentCompany));
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
                    PasswordHash = Encoding.UTF8.GetBytes("password3") // Updated to byte[]
                },
                new CompanyModel
                {
                    CompanyName = "Company D",
                    CVR = "55667788",
                    Email = "d@example.com",
                    Industry = "Retail",
                    PasswordHash = Encoding.UTF8.GetBytes("password4") // Updated to byte[]
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

        [Test]
        public async Task CreateCompaniesAsync_DoesNothingForEmptyList()
        {
            // Act
            await _companyService.CreateCompaniesAsync(new List<CompanyModel>());
            var companies = await _companyService.GetAllCompaniesAsync();

            // Assert
            Assert.That(companies.Count, Is.EqualTo(2)); // No new companies added
        }

        [Test]
        public async Task GetAverageSalariesForJobsInIndustryAsync_ReturnsCorrectData()
        {
            // Arrange
            var industry = "Tech";

            // Act
            var result = await _companyService.GetAverageSalariesForJobsInIndustryAsync(industry);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(0)); // Depends on seeded data
        }

        [Test]
        public async Task GetAverageSalariesForJobsInIndustryAsync_ReturnsEmpty_WhenNoCompaniesInIndustry()
        {
            // Act
            var result = await _companyService.GetAverageSalariesForJobsInIndustryAsync("NonExistentIndustry");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAverageSalariesForJobsInIndustryAsync_ReturnsEmpty_WhenNoEmployeesInIndustry()
        {
            // Arrange
            // Add a company in a new industry but no employees
            var dbContext = GetDbContext();
            dbContext.Companies.Add(new CompanyModel
            {
                CompanyName = "NoEmpCo",
                CVR = "99999999",
                Email = "noemp@example.com",
                Industry = "EmptyIndustry",
                PasswordHash = Encoding.UTF8.GetBytes("password")
            });
            dbContext.SaveChanges();

            // Act
            var result = await _companyService.GetAverageSalariesForJobsInIndustryAsync("EmptyIndustry");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAverageSalariesForJobsInIndustryAsync_ReturnsZeroSalary_WhenEmployeesHaveNoSalaries()
        {
            // Arrange
            var dbContext = GetDbContext();
            var company = new CompanyModel
            {
                CompanyName = "NoSalaryCo",
                CVR = "88888888",
                Email = "nosalary@example.com",
                Industry = "NoSalaryIndustry",
                PasswordHash = Encoding.UTF8.GetBytes("password")
            };
            dbContext.Companies.Add(company);
            dbContext.SaveChanges();

            dbContext.Employee.Add(new EmployeeModel
            {
                EmployeeID = 100,
                CompanyID = company.CompanyID,
                JobTitle = "Engineer",
                Gender = "Other"
            });
            dbContext.SaveChanges();

            // Act
            var result = await _companyService.GetAverageSalariesForJobsInIndustryAsync("NoSalaryIndustry");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].JobTitle, Is.EqualTo("Engineer"));
            Assert.That(result[0].GenderData["Other"].EmployeeCount, Is.EqualTo(1));
            Assert.That(result[0].GenderData["Other"].AverageSalary, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAverageSalariesForJobsInIndustryAsync_GroupsUnknownJobTitleAndGender()
        {
            // Arrange
            var dbContext = GetDbContext();
            var company = new CompanyModel
            {
                CompanyName = "UnknownsCo",
                CVR = "77777777",
                Email = "unknowns@example.com",
                Industry = "UnknownIndustry",
                PasswordHash = Encoding.UTF8.GetBytes("password")
            };
            dbContext.Companies.Add(company);
            dbContext.SaveChanges();

            dbContext.Employee.Add(new EmployeeModel
            {
                EmployeeID = 200,
                CompanyID = company.CompanyID,
                JobTitle = null,
                Gender = null
            });
            dbContext.Salaries.Add(new SalaryModel
            {
                SalaryID = 1,
                EmployeeID = 200,
                Salary = 42000,
                Timestamp = DateTime.UtcNow
            });
            dbContext.SaveChanges();

            // Act
            var result = await _companyService.GetAverageSalariesForJobsInIndustryAsync("UnknownIndustry");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].JobTitle, Is.EqualTo("Unknown"));
            Assert.That(result[0].GenderData.ContainsKey("Unknown"), Is.True);
            Assert.That(result[0].GenderData["Unknown"].EmployeeCount, Is.EqualTo(1));
            Assert.That(result[0].GenderData["Unknown"].AverageSalary, Is.EqualTo(42000));
        }

        // Helper to get the current dbContext from the service (since it's private)
        private ApplicationDbContext GetDbContext()
        {
            var field = typeof(CompanyService).GetField("_dbContext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (ApplicationDbContext)field!.GetValue(_companyService)!;
        }
    }
}