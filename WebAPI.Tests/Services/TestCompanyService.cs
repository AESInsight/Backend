using Backend.Data;
using Backend.Models;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Tests.Services
{
    [TestFixture]
    public class TestCompanyService
    {
        private ApplicationDbContext _dbContext;
        private Mock<ILogger<CompanyService>> _loggerMock;
        private CompanyService _service;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _loggerMock = new Mock<ILogger<CompanyService>>();
            _service = new CompanyService(_dbContext, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task GetAllCompaniesAsync_ReturnsAllCompanies()
        {
            // Arrange
            var company1 = new CompanyModel
            {
                CompanyName = "Company A",
                Industry = "Tech",
                CVR = "12345678",
                Email = "a@company.com"
            };
            var company2 = new CompanyModel
            {
                CompanyName = "Company B",
                Industry = "Finance",
                CVR = "87654321",
                Email = "b@company.com"
            };
            _dbContext.Companies.AddRange(company1, company2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetAllCompaniesAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(c => c.CompanyName == "Company A" && c.Email == "a@company.com"));
            Assert.That(result.Any(c => c.CompanyName == "Company B" && c.Email == "b@company.com"));
        }

        [Test]
        public async Task GetAllCompaniesAsync_ReturnsEmptyList_WhenNoCompaniesExist()
        {
            // Act
            var result = await _service.GetAllCompaniesAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetCompanyByIdAsync_ReturnsCompany_WhenCompanyExists()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "test@company.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetCompanyByIdAsync(company.CompanyID);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CompanyName, Is.EqualTo("TestCompany"));
            Assert.That(result.Industry, Is.EqualTo("Tech"));
            Assert.That(result.CVR, Is.EqualTo("12345678"));
            Assert.That(result.Email, Is.EqualTo("test@company.com"));
        }

        [Test]
        public void GetCompanyByIdAsync_ThrowsKeyNotFoundException_WhenCompanyDoesNotExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _service.GetCompanyByIdAsync(9999);
            });
            Assert.That(ex!.Message, Is.EqualTo("Company with ID 9999 not found."));
        }

        [Test]
        public async Task UpdateCompanyAsync_UpdatesCompany_WhenDataIsValid()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "Original",
                Industry = "Tech",
                CVR = "12345678",
                Email = "original@company.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            // Modify company
            var updatedCompany = new CompanyModel
            {
                CompanyID = company.CompanyID,
                CompanyName = "Updated",
                Industry = "Finance",
                CVR = "87654321",
                Email = "updated@company.com"
            };

            // Act
            await _service.UpdateCompanyAsync(updatedCompany);

            // Assert
            var dbCompany = await _dbContext.Companies.FindAsync(company.CompanyID);
            Assert.That(dbCompany, Is.Not.Null);
            Assert.That(dbCompany.CompanyName, Is.EqualTo("Updated"));
            Assert.That(dbCompany.Industry, Is.EqualTo("Finance"));
            Assert.That(dbCompany.CVR, Is.EqualTo("87654321"));
            Assert.That(dbCompany.Email, Is.EqualTo("updated@company.com"));
        }

        [Test]
        public void UpdateCompanyAsync_ThrowsKeyNotFoundException_WhenCompanyDoesNotExist()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyID = 9999,
                CompanyName = "NonExistent",
                Industry = "Tech",
                CVR = "12345678",
                Email = "nonexistent@company.com"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _service.UpdateCompanyAsync(company);
            });
            Assert.That(ex!.Message, Is.EqualTo("Company with ID 9999 not found."));
        }

        [Test]
        public async Task UpdateCompanyAsync_ThrowsArgumentException_WhenCVRIsInvalid()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "Test",
                Industry = "Tech",
                CVR = "12345678",
                Email = "test@company.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            var invalidCompany = new CompanyModel
            {
                CompanyID = company.CompanyID,
                CompanyName = "Test",
                Industry = "Tech",
                CVR = "1234", // Invalid CVR
                Email = "test@company.com"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _service.UpdateCompanyAsync(invalidCompany);
            });
            Assert.That(ex!.Message, Is.EqualTo("CVR must be exactly 8 digits."));
        }

        [Test]
        public async Task DeleteCompanyAsync_RemovesCompany_WhenCompanyExists()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "DeleteMe",
                Industry = "Tech",
                CVR = "12345678",
                Email = "deleteme@company.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            // Act
            await _service.DeleteCompanyAsync(company.CompanyID);

            // Assert
            var dbCompany = await _dbContext.Companies.FindAsync(company.CompanyID);
            Assert.That(dbCompany, Is.Null);
        }

        [Test]
        public async Task DeleteCompanyAsync_DoesNothing_WhenCompanyDoesNotExist()
        {
            // Arrange
            var initialCount = await _dbContext.Companies.CountAsync();

            // Act
            await _service.DeleteCompanyAsync(9999);

            // Assert
            var finalCount = await _dbContext.Companies.CountAsync();
            Assert.That(finalCount, Is.EqualTo(initialCount));
        }

        [Test]
        public async Task DeleteAllCompaniesAsync_RemovesAllCompanies_WhenCompaniesExist()
        {
            // Arrange
            var company1 = new CompanyModel
            {
                CompanyName = "Company1",
                Industry = "Tech",
                CVR = "12345678",
                Email = "company1@test.com"
            };
            var company2 = new CompanyModel
            {
                CompanyName = "Company2",
                Industry = "Finance",
                CVR = "87654321",
                Email = "company2@test.com"
            };
            _dbContext.Companies.AddRange(company1, company2);
            await _dbContext.SaveChangesAsync();

            // Act
            await _service.DeleteAllCompaniesAsync();

            // Assert
            var count = await _dbContext.Companies.CountAsync();
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteAllCompaniesAsync_DoesNothing_WhenNoCompaniesExist()
        {
            // Arrange
            var initialCount = await _dbContext.Companies.CountAsync();

            // Act
            await _service.DeleteAllCompaniesAsync();

            // Assert
            var finalCount = await _dbContext.Companies.CountAsync();
            Assert.That(finalCount, Is.EqualTo(initialCount));
        }

        [Test]
        public async Task CreateCompaniesAsync_AddsCompanies_WhenDataIsValid()
        {
            // Arrange
            var companies = new List<CompanyModel>
            {
                new CompanyModel
                {
                    CompanyName = "Company1",
                    Industry = "Tech",
                    CVR = "12345678",
                    Email = "company1@test.com"
                },
                new CompanyModel
                {
                    CompanyName = "Company2",
                    Industry = "Finance",
                    CVR = "87654321",
                    Email = "company2@test.com"
                }
            };

            // Act
            await _service.CreateCompaniesAsync(companies);

            // Assert
            var dbCompanies = await _dbContext.Companies.ToListAsync();
            Assert.That(dbCompanies.Count, Is.EqualTo(2));
            Assert.That(dbCompanies.Any(c => c.CompanyName == "Company1" && c.Email == "company1@test.com"));
            Assert.That(dbCompanies.Any(c => c.CompanyName == "Company2" && c.Email == "company2@test.com"));
        }

        [Test]
        public void CreateCompaniesAsync_ThrowsArgumentException_WhenCVRIsInvalid()
        {
            // Arrange
            var companies = new List<CompanyModel>
            {
                new CompanyModel
                {
                    CompanyName = "InvalidCVR",
                    Industry = "Tech",
                    CVR = "1234", // Invalid CVR
                    Email = "invalidcvr@test.com"
                }
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _service.CreateCompaniesAsync(companies);
            });
            Assert.That(ex!.Message, Does.Contain("CVR for company 'InvalidCVR' must be exactly 8 digits."));
        }

        [Test]
        public void CreateCompaniesAsync_ThrowsArgumentException_WhenEmailIsInvalid()
        {
            // Arrange
            var companies = new List<CompanyModel>
            {
                new CompanyModel
                {
                    CompanyName = "InvalidEmail",
                    Industry = "Tech",
                    CVR = "12345678",
                    Email = "invalidemail" // Invalid email
                }
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _service.CreateCompaniesAsync(companies);
            });
            Assert.That(ex!.Message, Does.Contain("Email for company 'InvalidEmail' must be a valid email address."));
        }

        [Test]
        public async Task CreateCompaniesAsync_ThrowsArgumentException_WhenEmailAlreadyExists()
        {
            // Arrange
            var existing = new CompanyModel
            {
                CompanyName = "Existing",
                Industry = "Tech",
                CVR = "12345678",
                Email = "duplicate@test.com"
            };
            _dbContext.Companies.Add(existing);
            await _dbContext.SaveChangesAsync();

            var companies = new List<CompanyModel>
            {
                new CompanyModel
                {
                    CompanyName = "Duplicate",
                    Industry = "Finance",
                    CVR = "87654321",
                    Email = "duplicate@test.com"
                }
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _service.CreateCompaniesAsync(companies);
            });
            Assert.That(ex!.Message, Does.Contain("A company with email 'duplicate@test.com' already exists."));
        }

        [Test]
        public async Task CreateCompaniesAsync_AssignsIncrementingCompanyIDs()
        {
            // Arrange
            var existing = new CompanyModel
            {
                CompanyName = "Existing",
                Industry = "Tech",
                CVR = "12345678",
                Email = "existing@test.com"
            };
            _dbContext.Companies.Add(existing);
            await _dbContext.SaveChangesAsync();

            var companies = new List<CompanyModel>
            {
                new CompanyModel
                {
                    CompanyName = "New1",
                    Industry = "Tech",
                    CVR = "11111111",
                    Email = "new1@test.com"
                },
                new CompanyModel
                {
                    CompanyName = "New2",
                    Industry = "Finance",
                    CVR = "22222222",
                    Email = "new2@test.com"
                }
            };

            // Act
            await _service.CreateCompaniesAsync(companies);

            // Assert
            var dbCompanies = await _dbContext.Companies.OrderBy(c => c.CompanyID).ToListAsync();
            Assert.That(dbCompanies[1].CompanyID, Is.EqualTo(dbCompanies[0].CompanyID + 1));
            Assert.That(dbCompanies[2].CompanyID, Is.EqualTo(dbCompanies[1].CompanyID + 1));
        }

        [Test]
        public async Task GetCompanyByEmailAsync_ReturnsCompany_WhenEmailExists()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "EmailTest",
                Industry = "Tech",
                CVR = "12345678",
                Email = "emailtest@company.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetCompanyByEmailAsync("emailtest@company.com");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CompanyName, Is.EqualTo("EmailTest"));
            Assert.That(result.Email, Is.EqualTo("emailtest@company.com"));
        }

        [Test]
        public async Task GetCompanyByEmailAsync_ReturnsNull_WhenEmailDoesNotExist()
        {
            // Act
            var result = await _service.GetCompanyByEmailAsync("doesnotexist@company.com");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCompanyByEmailAsync_ReturnsNull_WhenEmailIsNullOrEmpty()
        {
            // Act
            var resultNull = await _service.GetCompanyByEmailAsync(null!);
            var resultEmpty = await _service.GetCompanyByEmailAsync("");

            // Assert
            Assert.That(resultNull, Is.Null);
            Assert.That(resultEmpty, Is.Null);
        }

        [Test]
        public async Task GetAllIndustriesAsync_ReturnsDistinctIndustriesSorted()
        {
            // Arrange
            var companies = new List<CompanyModel>
            {
                new CompanyModel { CompanyName = "A", Industry = "Tech", CVR = "12345678", Email = "a@a.com" },
                new CompanyModel { CompanyName = "B", Industry = "Finance", CVR = "87654321", Email = "b@b.com" },
                new CompanyModel { CompanyName = "C", Industry = "Tech", CVR = "23456789", Email = "c@c.com" },
                new CompanyModel { CompanyName = "D", Industry = "Health", CVR = "34567890", Email = "d@d.com" }
            };
            _dbContext.Companies.AddRange(companies);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetAllIndustriesAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result, Is.EquivalentTo(new[] { "Finance", "Health", "Tech" }));
            Assert.That(result, Is.Ordered); // Should be sorted alphabetically
        }

        [Test]
        public async Task GetAllIndustriesAsync_ReturnsEmptyList_WhenNoCompaniesExist()
        {
            // Act
            var result = await _service.GetAllIndustriesAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAverageSalariesForJobsInIndustryAsync_ReturnsCorrectAverages()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "test@company.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            var employee1 = new EmployeeModel
            {
                CompanyID = company.CompanyID,
                JobTitle = "Developer",
                Gender = "Male"
            };
            var employee2 = new EmployeeModel
            {
                CompanyID = company.CompanyID,
                JobTitle = "Developer",
                Gender = "Female"
            };
            var employee3 = new EmployeeModel
            {
                CompanyID = company.CompanyID,
                JobTitle = "Designer",
                Gender = "Male"
            };
            _dbContext.Employee.AddRange(employee1, employee2, employee3);
            await _dbContext.SaveChangesAsync();

            var salary1 = new SalaryModel
            {
                EmployeeID = employee1.EmployeeID,
                Salary = 60000,
                Timestamp = DateTime.UtcNow.AddDays(-2)
            };
            var salary2 = new SalaryModel
            {
                EmployeeID = employee1.EmployeeID,
                Salary = 65000,
                Timestamp = DateTime.UtcNow.AddDays(-1)
            };
            var salary3 = new SalaryModel
            {
                EmployeeID = employee2.EmployeeID,
                Salary = 70000,
                Timestamp = DateTime.UtcNow
            };
            var salary4 = new SalaryModel
            {
                EmployeeID = employee3.EmployeeID,
                Salary = 55000,
                Timestamp = DateTime.UtcNow
            };
            _dbContext.Salaries.AddRange(salary1, salary2, salary3, salary4);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetAverageSalariesForJobsInIndustryAsync("Tech");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2)); // Developer and Designer

            var developer = result.FirstOrDefault(j => j.JobTitle == "Developer");
            Assert.That(developer, Is.Not.Null);
            Assert.That(developer.GenderData.ContainsKey("Male"), Is.True);
            Assert.That(developer.GenderData.ContainsKey("Female"), Is.True);
            Assert.That(developer.GenderData["Male"].EmployeeCount, Is.EqualTo(1));
            Assert.That(developer.GenderData["Male"].AverageSalary, Is.EqualTo(65000)); // Latest salary for employee1
            Assert.That(developer.GenderData["Female"].EmployeeCount, Is.EqualTo(1));
            Assert.That(developer.GenderData["Female"].AverageSalary, Is.EqualTo(70000)); // Only salary for employee2

            var designer = result.FirstOrDefault(j => j.JobTitle == "Designer");
            Assert.That(designer, Is.Not.Null);
            Assert.That(designer.GenderData.ContainsKey("Male"), Is.True);
            Assert.That(designer.GenderData["Male"].EmployeeCount, Is.EqualTo(1));
            Assert.That(designer.GenderData["Male"].AverageSalary, Is.EqualTo(55000));
        }

        [Test]
        public async Task GetAverageSalariesForJobsInIndustryAsync_ReturnsEmptyList_WhenNoCompaniesInIndustry()
        {
            // Act
            var result = await _service.GetAverageSalariesForJobsInIndustryAsync("NonExistentIndustry");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAverageSalariesForJobsInIndustryAsync_ReturnsEmptyList_WhenNoEmployeesInIndustry()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "test@company.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetAverageSalariesForJobsInIndustryAsync("Tech");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAverageSalariesForJobsInIndustryAsync_HandlesEmployeesWithNoSalaries()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "test@company.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            var employee = new EmployeeModel
            {
                CompanyID = company.CompanyID,
                JobTitle = "Developer",
                Gender = "Male"
            };
            _dbContext.Employee.Add(employee);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetAverageSalariesForJobsInIndustryAsync("Tech");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            var developer = result.First();
            Assert.That(developer.JobTitle, Is.EqualTo("Developer"));
            Assert.That(developer.GenderData["Male"].EmployeeCount, Is.EqualTo(1));
            Assert.That(developer.GenderData["Male"].AverageSalary, Is.EqualTo(0));
        }

        // Add this helper class inside your test file (outside the test class)
        public class ThrowingDbContext : ApplicationDbContext
        {
            public ThrowingDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

            public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            {
                throw new Microsoft.EntityFrameworkCore.DbUpdateException("Simulated DB error", new Exception("Inner error"));
            }
        }

        [Test]
        public Task CreateCompaniesAsync_ThrowsException_WhenDbUpdateExceptionOccurs()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var throwingDbContext = new ThrowingDbContext(options);
            var loggerMock = new Mock<ILogger<CompanyService>>();
            var service = new CompanyService(throwingDbContext, loggerMock.Object);

            var companies = new List<CompanyModel>
            {
                new CompanyModel
                {
                    CompanyName = "Test",
                    Industry = "Tech",
                    CVR = "12345678",
                    Email = "test@company.com"
                }
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await service.CreateCompaniesAsync(companies);
            });
            Assert.That(ex!.Message, Does.Contain("Database error while creating companies: Inner error"));
            return Task.CompletedTask;
        }
    }
}