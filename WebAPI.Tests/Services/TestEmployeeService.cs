using Backend.Data;
using Backend.Models;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Tests.Services
{
    [TestFixture]
    public class TestEmployeeService
    {
        private ApplicationDbContext _dbContext;
        private EmployeeService _service;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _service = new EmployeeService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            if (_dbContext != null)
            {
                _dbContext.Database.EnsureDeleted();
                _dbContext.Dispose();
                _dbContext = null!;
            }
        }

        [Test]
        public async Task BulkCreateEmployeesAsync_AddsEmployees_WhenDataIsValid()
        {
            // Arrange
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel { JobTitle = "Developer", Experience = 2, Gender = "Male", CompanyID = 1 },
                new EmployeeModel { JobTitle = "Designer", Experience = 3, Gender = "Female", CompanyID = 1 }
            };

            // Act
            var result = await _service.BulkCreateEmployeesAsync(employees);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            var dbEmployees = await _dbContext.Employee.ToListAsync();
            Assert.That(dbEmployees.Count, Is.EqualTo(2));
            Assert.That(dbEmployees.Any(e => e.JobTitle == "Developer" && e.Gender == "Male"));
            Assert.That(dbEmployees.Any(e => e.JobTitle == "Designer" && e.Gender == "Female"));
        }

        [Test]
        public async Task BulkCreateEmployeesAsync_ReturnsEmptyList_WhenInputIsEmpty()
        {
            // Arrange
            var employees = new List<EmployeeModel>();

            // Act
            var result = await _service.BulkCreateEmployeesAsync(employees);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            var dbEmployees = await _dbContext.Employee.ToListAsync();
            Assert.That(dbEmployees.Count, Is.EqualTo(0));
        }

        [Test]
        public void BulkCreateEmployeesAsync_ThrowsException_WhenDbContextFails()
        {
            // Arrange
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel { JobTitle = "Developer", Experience = 2, Gender = "Male", CompanyID = 1 }
            };

            // Simulate a disposed context to force an exception
            _dbContext.Dispose();
            _dbContext = null!; // <-- Add this line

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await _service.BulkCreateEmployeesAsync(employees);
            });
            Assert.That(ex!.Message, Does.Contain("Error during employee creation"));
        }

        [Test]
        public async Task GetAllEmployeesAsync_ReturnsAllEmployees()
        {
            // Arrange
            var employee1 = new EmployeeModel
            {
                JobTitle = "Developer",
                Experience = 2,
                Gender = "Male",
                CompanyID = 1
            };
            var employee2 = new EmployeeModel
            {
                JobTitle = "Designer",
                Experience = 3,
                Gender = "Female",
                CompanyID = 1
            };
            _dbContext.Employee.AddRange(employee1, employee2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetAllEmployeesAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(e => e.JobTitle == "Developer" && e.Gender == "Male"));
            Assert.That(result.Any(e => e.JobTitle == "Designer" && e.Gender == "Female"));
        }

        [Test]
        public async Task GetAllEmployeesAsync_ReturnsEmptyList_WhenNoEmployeesExist()
        {
            // Act
            var result = await _service.GetAllEmployeesAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteAllEmployeesAsync_RemovesAllEmployees_WhenEmployeesExist()
        {
            // Arrange
            var employee1 = new EmployeeModel
            {
                JobTitle = "Developer",
                Experience = 2,
                Gender = "Male",
                CompanyID = 1
            };
            var employee2 = new EmployeeModel
            {
                JobTitle = "Designer",
                Experience = 3,
                Gender = "Female",
                CompanyID = 1
            };
            _dbContext.Employee.AddRange(employee1, employee2);
            await _dbContext.SaveChangesAsync();

            // Act
            await _service.DeleteAllEmployeesAsync();

            // Assert
            var count = await _dbContext.Employee.CountAsync();
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteAllEmployeesAsync_DoesNothing_WhenNoEmployeesExist()
        {
            // Arrange
            var initialCount = await _dbContext.Employee.CountAsync();

            // Act
            await _service.DeleteAllEmployeesAsync();

            // Assert
            var finalCount = await _dbContext.Employee.CountAsync();
            Assert.That(finalCount, Is.EqualTo(initialCount));
        }

        [Test]
        public async Task GetEmployeeByIdAsync_ReturnsEmployee_WhenEmployeeExists()
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
                JobTitle = "Developer",
                Experience = 5,
                Gender = "Male",
                CompanyID = company.CompanyID
            };
            _dbContext.Employee.Add(employee);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetEmployeeByIdAsync(employee.EmployeeID);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.EmployeeID, Is.EqualTo(employee.EmployeeID));
            Assert.That(result.JobTitle, Is.EqualTo("Developer"));
            Assert.That(result.Experience, Is.EqualTo(5));
            Assert.That(result.Gender, Is.EqualTo("Male"));
            Assert.That(result.CompanyID, Is.EqualTo(company.CompanyID));
            Assert.That(result.Company, Is.Not.Null);
            Assert.That(result.Company.CompanyName, Is.EqualTo("TestCompany"));
        }

        [Test]
        public void GetEmployeeByIdAsync_ThrowsKeyNotFoundException_WhenEmployeeDoesNotExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _service.GetEmployeeByIdAsync(9999);
            });
            Assert.That(ex!.Message, Is.EqualTo("Employee with ID 9999 was not found."));
        }

        [Test]
        public async Task GetMaxEmployeeIdAsync_ReturnsMaxId_WhenEmployeesExist()
        {
            // Arrange
            var employee1 = new EmployeeModel
            {
                JobTitle = "Developer",
                Experience = 2,
                Gender = "Male",
                CompanyID = 1
            };
            var employee2 = new EmployeeModel
            {
                JobTitle = "Designer",
                Experience = 3,
                Gender = "Female",
                CompanyID = 1
            };
            _dbContext.Employee.AddRange(employee1, employee2);
            await _dbContext.SaveChangesAsync();

            // Act
            var maxId = await _service.GetMaxEmployeeIdAsync();

            // Assert
            var expectedMaxId = await _dbContext.Employee.MaxAsync(e => e.EmployeeID);
            Assert.That(maxId, Is.EqualTo(expectedMaxId));
        }

        [Test]
        public async Task GetMaxEmployeeIdAsync_ReturnsZero_WhenNoEmployeesExist()
        {
            // Act
            var maxId = await _service.GetMaxEmployeeIdAsync();

            // Assert
            Assert.That(maxId, Is.EqualTo(0));
        }

        [Test]
        public async Task UpdateEmployeeAsync_UpdatesEmployee_WhenDataIsValid()
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
                JobTitle = "Developer",
                Experience = 2,
                Gender = "Male",
                CompanyID = company.CompanyID
            };
            _dbContext.Employee.Add(employee);
            await _dbContext.SaveChangesAsync();

            var updatedEmployee = new EmployeeModel
            {
                JobTitle = "Senior Developer",
                Experience = 5,
                Gender = "Female",
                CompanyID = company.CompanyID
            };

            // Act
            var result = await _service.UpdateEmployeeAsync(employee.EmployeeID, updatedEmployee);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.EmployeeID, Is.EqualTo(employee.EmployeeID));
            Assert.That(result.JobTitle, Is.EqualTo("Senior Developer"));
            Assert.That(result.Experience, Is.EqualTo(5));
            Assert.That(result.Gender, Is.EqualTo("Female"));
            Assert.That(result.CompanyID, Is.EqualTo(company.CompanyID));
        }

        [Test]
        public void UpdateEmployeeAsync_ThrowsKeyNotFoundException_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var updatedEmployee = new EmployeeModel
            {
                JobTitle = "Developer",
                Experience = 2,
                Gender = "Male",
                CompanyID = 1
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _service.UpdateEmployeeAsync(9999, updatedEmployee);
            });
            Assert.That(ex!.Message, Is.EqualTo("Employee with ID 9999 not found."));
        }

        [Test]
        public async Task DeleteEmployeeAsync_RemovesEmployee_WhenEmployeeExists()
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
                JobTitle = "Developer",
                Experience = 2,
                Gender = "Male",
                CompanyID = company.CompanyID
            };
            _dbContext.Employee.Add(employee);
            await _dbContext.SaveChangesAsync();

            // Act
            var deleted = await _service.DeleteEmployeeAsync(employee.EmployeeID);

            // Assert
            Assert.That(deleted, Is.Not.Null);
            Assert.That(deleted.EmployeeID, Is.EqualTo(employee.EmployeeID));
            var dbEmployee = await _dbContext.Employee.FindAsync(employee.EmployeeID);
            Assert.That(dbEmployee, Is.Null);
        }

        [Test]
        public void DeleteEmployeeAsync_ThrowsKeyNotFoundException_WhenEmployeeDoesNotExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _service.DeleteEmployeeAsync(9999);
            });
            Assert.That(ex!.Message, Is.EqualTo("Employee with ID 9999 not found."));
        }

        [Test]
        public async Task GetEmployeesByJobTitleAsync_ReturnsEmployees_WithMatchingJobTitle()
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
                JobTitle = "Developer",
                Experience = 2,
                Gender = "Male",
                CompanyID = company.CompanyID
            };
            var employee2 = new EmployeeModel
            {
                JobTitle = "Developer",
                Experience = 3,
                Gender = "Female",
                CompanyID = company.CompanyID
            };
            var employee3 = new EmployeeModel
            {
                JobTitle = "Designer",
                Experience = 4,
                Gender = "Male",
                CompanyID = company.CompanyID
            };
            _dbContext.Employee.AddRange(employee1, employee2, employee3);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetEmployeesByJobTitleAsync("Developer");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(e => e.JobTitle == "Developer"));
            Assert.That(result.Any(e => e.Gender == "Male"));
            Assert.That(result.Any(e => e.Gender == "Female"));
            Assert.That(result.All(e => e.Company != null && e.Company.CompanyName == "TestCompany"));
        }

        [Test]
        public async Task GetEmployeesByJobTitleAsync_ReturnsEmptyList_WhenNoEmployeesWithJobTitle()
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
                JobTitle = "Designer",
                Experience = 4,
                Gender = "Male",
                CompanyID = company.CompanyID
            };
            _dbContext.Employee.Add(employee);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetEmployeesByJobTitleAsync("Developer");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetEmployeesByJobTitleAsync_ReturnsEmptyList_WhenNoEmployeesExist()
        {
            // Act
            var result = await _service.GetEmployeesByJobTitleAsync("Developer");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllJobTitlesAsync_ReturnsDistinctJobTitlesSorted()
        {
            // Arrange
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel { JobTitle = "Developer", Experience = 2, Gender = "Male", CompanyID = 1 },
                new EmployeeModel { JobTitle = "Designer", Experience = 3, Gender = "Female", CompanyID = 1 },
                new EmployeeModel { JobTitle = "Developer", Experience = 4, Gender = "Female", CompanyID = 1 },
                new EmployeeModel { JobTitle = "Analyst", Experience = 1, Gender = "Male", CompanyID = 1 }
            };
            _dbContext.Employee.AddRange(employees);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetAllJobTitlesAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result, Is.EquivalentTo(new[] { "Analyst", "Designer", "Developer" }));
            Assert.That(result, Is.Ordered); // Should be sorted alphabetically
        }

        [Test]
        public async Task GetAllJobTitlesAsync_ReturnsEmptyList_WhenNoEmployeesExist()
        {
            // Act
            var result = await _service.GetAllJobTitlesAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetSalaryDifferencesByGenderAsync_ReturnsCorrectSalaryDifferences_ForJobTitle()
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
                Timestamp = new DateTime(2024, 1, 1)
            };
            var salary2 = new SalaryModel
            {
                EmployeeID = employee1.EmployeeID,
                Salary = 65000,
                Timestamp = new DateTime(2024, 2, 1)
            };
            var salary3 = new SalaryModel
            {
                EmployeeID = employee2.EmployeeID,
                Salary = 70000,
                Timestamp = new DateTime(2024, 2, 1)
            };
            var salary4 = new SalaryModel
            {
                EmployeeID = employee3.EmployeeID,
                Salary = 55000,
                Timestamp = new DateTime(2024, 2, 1)
            };
            _dbContext.Salaries.AddRange(salary1, salary2, salary3, salary4);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetSalaryDifferencesByGenderAsync("Developer");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContainsKey("Male"), Is.True);
            Assert.That(result.ContainsKey("Female"), Is.True);

            var maleList = result["Male"];
            var femaleList = result["Female"];

            Assert.That(maleList.Count, Is.EqualTo(2)); // Jan and Feb for male developer
            Assert.That(femaleList.Count, Is.EqualTo(1)); // Feb for female developer

            Assert.That(maleList[0].Month, Is.EqualTo(new DateTime(2024, 1, 1)));
            Assert.That(maleList[0].AverageSalary, Is.EqualTo(60000));
            Assert.That(maleList[1].Month, Is.EqualTo(new DateTime(2024, 2, 1)));
            Assert.That(maleList[1].AverageSalary, Is.EqualTo(65000));
            Assert.That(femaleList[0].Month, Is.EqualTo(new DateTime(2024, 2, 1)));
            Assert.That(femaleList[0].AverageSalary, Is.EqualTo(70000));
        }

        [Test]
        public async Task GetSalaryDifferencesByGenderAsync_ReturnsEmptyLists_WhenNoEmployeesWithJobTitle()
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
            var result = await _service.GetSalaryDifferencesByGenderAsync("NonExistentJob");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result["Male"], Is.Empty);
            Assert.That(result["Female"], Is.Empty);
        }

        [Test]
        public async Task GetSalaryDifferencesByGenderAsync_ReturnsEmptyLists_WhenEmployeesHaveNoSalaries()
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
            var result = await _service.GetSalaryDifferencesByGenderAsync("Developer");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result["Male"], Is.Empty);
            Assert.That(result["Female"], Is.Empty);
        }

        [Test]
        public async Task GetAllSalaryDifferencesByGenderAsync_ReturnsCorrectSalaryDifferences_ForAllJobs()
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
                Timestamp = new DateTime(2024, 1, 1)
            };
            var salary2 = new SalaryModel
            {
                EmployeeID = employee1.EmployeeID,
                Salary = 65000,
                Timestamp = new DateTime(2024, 2, 1)
            };
            var salary3 = new SalaryModel
            {
                EmployeeID = employee2.EmployeeID,
                Salary = 70000,
                Timestamp = new DateTime(2024, 2, 1)
            };
            var salary4 = new SalaryModel
            {
                EmployeeID = employee3.EmployeeID,
                Salary = 55000,
                Timestamp = new DateTime(2024, 2, 1)
            };
            _dbContext.Salaries.AddRange(salary1, salary2, salary3, salary4);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetAllSalaryDifferencesByGenderAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContainsKey("Male"), Is.True);
            Assert.That(result.ContainsKey("Female"), Is.True);

            var maleList = result["Male"];
            var femaleList = result["Female"];

            Assert.That(maleList.Count, Is.EqualTo(2));
            Assert.That(femaleList.Count, Is.EqualTo(1));

            Assert.That(maleList.Any(x => x.Month == new DateTime(2024, 1, 1) && x.AverageSalary == 60000));
            Assert.That(maleList.Any(x => x.Month == new DateTime(2024, 2, 1) && x.AverageSalary == 60000)); // (65000+55000)/2
            Assert.That(femaleList.Any(x => x.Month == new DateTime(2024, 2, 1) && x.AverageSalary == 70000));
        }

        [Test]
        public async Task GetAllSalaryDifferencesByGenderAsync_ReturnsEmptyLists_WhenNoSalariesExist()
        {
            // Act
            var result = await _service.GetAllSalaryDifferencesByGenderAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result["Male"], Is.Empty);
            Assert.That(result["Female"], Is.Empty);
        }

        [Test]
        public async Task GetAllSalaryDifferencesByGenderAsync_ReturnsEmptyLists_WhenEmployeesHaveNoSalaries()
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
            var result = await _service.GetAllSalaryDifferencesByGenderAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result["Male"], Is.Empty);
            Assert.That(result["Female"], Is.Empty);
        }

        [Test]
        public async Task GetEmployeeIndustryByIdAsync_ReturnsIndustry_WhenEmployeeAndCompanyExist()
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
                JobTitle = "Developer",
                Experience = 2,
                Gender = "Male",
                CompanyID = company.CompanyID
            };
            _dbContext.Employee.Add(employee);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetEmployeeIndustryByIdAsync(employee.EmployeeID);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.EmployeeID, Is.EqualTo(employee.EmployeeID));
            Assert.That(result.CompanyID, Is.EqualTo(company.CompanyID));
            Assert.That(result.Industry, Is.EqualTo("Tech"));
        }

        [Test]
        public async Task GetEmployeeIndustryByIdAsync_ReturnsNull_WhenEmployeeDoesNotExist()
        {
            // Act
            var result = await _service.GetEmployeeIndustryByIdAsync(9999);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEmployeeIndustryByIdAsync_ReturnsNull_WhenEmployeeHasNoCompany()
        {
            // Arrange
            var employee = new EmployeeModel
            {
                JobTitle = "Developer",
                Experience = 2,
                Gender = "Male",
                CompanyID = 9999 // Non-existent company
            };
            _dbContext.Employee.Add(employee);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetEmployeeIndustryByIdAsync(employee.EmployeeID);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEmployeesByCompanyIdAsync_ReturnsEmployees_WhenCompanyHasEmployees()
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

            var employee1 = new EmployeeModel
            {
                JobTitle = "Developer",
                Experience = 2,
                Gender = "Male",
                CompanyID = company1.CompanyID
            };
            var employee2 = new EmployeeModel
            {
                JobTitle = "Designer",
                Experience = 3,
                Gender = "Female",
                CompanyID = company1.CompanyID
            };
            var employee3 = new EmployeeModel
            {
                JobTitle = "Analyst",
                Experience = 1,
                Gender = "Male",
                CompanyID = company2.CompanyID
            };
            _dbContext.Employee.AddRange(employee1, employee2, employee3);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetEmployeesByCompanyIdAsync(company1.CompanyID);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(e => e.CompanyID == company1.CompanyID));
            Assert.That(result.Any(e => e.JobTitle == "Developer"));
            Assert.That(result.Any(e => e.JobTitle == "Designer"));
            Assert.That(result.All(e => e.Company != null && e.Company.CompanyName == "Company1"));
        }

        [Test]
        public async Task GetEmployeesByCompanyIdAsync_ReturnsEmptyList_WhenCompanyHasNoEmployees()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "Company1",
                Industry = "Tech",
                CVR = "12345678",
                Email = "company1@test.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetEmployeesByCompanyIdAsync(company.CompanyID);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetEmployeesByCompanyIdAsync_ReturnsEmptyList_WhenCompanyDoesNotExist()
        {
            // Act
            var result = await _service.GetEmployeesByCompanyIdAsync(9999);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }
    }
}