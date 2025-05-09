using Moq;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace WebAPI.Tests.Services
{
    [TestFixture]
    public class TestEmployeeService
    {
        private EmployeeService _employeeService;

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
                    CompanyName = "TechCorp",
                    Industry = "Technology",
                    CVR = "12345678",
                    Email = "contact@techcorp.com",
                    PasswordHash = Encoding.UTF8.GetBytes("hashedpassword1") // Fixed to byte[]
                },
                new CompanyModel
                {
                    CompanyID = 2,
                    CompanyName = "DataCorp",
                    Industry = "Data Analytics",
                    CVR = "87654321",
                    Email = "info@datacorp.com",
                    PasswordHash = Encoding.UTF8.GetBytes("hashedpassword2") // Fixed to byte[]
                }
            });

            dbContext.Employee.AddRange(new List<EmployeeModel>
            {
                new EmployeeModel
                {
                    EmployeeID = 1,
                    JobTitle = "Software Engineer",
                    Experience = 5,
                    Gender = "Male",
                    CompanyID = 1
                },
                new EmployeeModel
                {
                    EmployeeID = 2,
                    JobTitle = "Data Scientist",
                    Experience = 3,
                    Gender = "Female",
                    CompanyID = 2
                }
            });

            dbContext.Salaries.AddRange(new List<SalaryModel>
            {
                new SalaryModel
                {
                    EmployeeID = 1,
                    Salary = 5000,
                    Timestamp = new DateTime(2025, 1, 1)
                },
                new SalaryModel
                {
                    EmployeeID = 1,
                    Salary = 5500,
                    Timestamp = new DateTime(2025, 2, 1)
                },
                new SalaryModel
                {
                    EmployeeID = 2,
                    Salary = 4800,
                    Timestamp = new DateTime(2025, 1, 1)
                },
                new SalaryModel
                {
                    EmployeeID = 2,
                    Salary = 5000,
                    Timestamp = new DateTime(2025, 2, 1)
                }
            });

            dbContext.SaveChanges();

            // Initialize the service
            _employeeService = new EmployeeService(dbContext);
        }

        [Test]
        public async Task GetAllEmployeesAsync_ReturnsAllEmployees()
        {
            // Act
            var result = await _employeeService.GetAllEmployeesAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].JobTitle, Is.EqualTo("Software Engineer"));
            Assert.That(result[1].JobTitle, Is.EqualTo("Data Scientist"));
        }

        [Test]
        public async Task GetEmployeeByIdAsync_ReturnsCorrectEmployee()
        {
            // Act
            var result = await _employeeService.GetEmployeeByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.JobTitle, Is.EqualTo("Software Engineer"));
            Assert.That(result.Experience, Is.EqualTo(5));
        }

        [Test]
        public void GetEmployeeByIdAsync_ThrowsExceptionForNonExistentEmployee()
        {
            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _employeeService.GetEmployeeByIdAsync(999));
        }

        [Test]
        public async Task BulkCreateEmployeesAsync_AddsNewEmployees()
        {
            // Arrange
            var newEmployees = new List<EmployeeModel>
            {
                new EmployeeModel
                {
                    JobTitle = "Product Manager",
                    Experience = 4,
                    Gender = "Male",
                    CompanyID = 1
                },
                new EmployeeModel
                {
                    JobTitle = "UX Designer",
                    Experience = 2,
                    Gender = "Female",
                    CompanyID = 2
                }
            };

            // Act
            await _employeeService.BulkCreateEmployeesAsync(newEmployees);
            var employees = await _employeeService.GetAllEmployeesAsync();

            // Assert
            Assert.That(employees.Count, Is.EqualTo(4)); // 2 existing + 2 new
            Assert.That(employees.Any(e => e.JobTitle == "Product Manager"), Is.True);
            Assert.That(employees.Any(e => e.JobTitle == "UX Designer"), Is.True);
        }

        [Test]
        public async Task BulkCreateEmployeesAsync_DoesNothingForEmptyList()
        {
            // Arrange
            var emptyList = new List<EmployeeModel>();

            // Act
            await _employeeService.BulkCreateEmployeesAsync(emptyList);
            var employees = await _employeeService.GetAllEmployeesAsync();

            // Assert
            Assert.That(employees.Count, Is.EqualTo(2)); // No new employees should be added
        }

        [Test]
        public async Task DeleteEmployeeAsync_RemovesEmployee()
        {
            // Act
            await _employeeService.DeleteEmployeeAsync(1);
            var employees = await _employeeService.GetAllEmployeesAsync();

            // Assert
            Assert.That(employees.Any(e => e.EmployeeID == 1), Is.False);
        }

        [Test]
        public void DeleteEmployeeAsync_ThrowsExceptionForNonExistentEmployee()
        {
            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _employeeService.DeleteEmployeeAsync(999));
        }

        [Test]
        public async Task UpdateEmployeeAsync_UpdatesEmployeeDetails()
        {
            // Arrange
            var updatedEmployee = new EmployeeModel
            {
                JobTitle = "Senior Software Engineer",
                Experience = 6,
                Gender = "Male",
                CompanyID = 1
            };

            // Act
            var result = await _employeeService.UpdateEmployeeAsync(1, updatedEmployee);

            // Assert
            Assert.That(result.JobTitle, Is.EqualTo("Senior Software Engineer"));
            Assert.That(result.Experience, Is.EqualTo(6));
        }

        [Test]
        public void UpdateEmployeeAsync_ThrowsExceptionForNonExistentEmployee()
        {
            // Arrange
            var updatedEmployee = new EmployeeModel
            {
                JobTitle = "Non-Existent",
                Experience = 0,
                Gender = "Male",
                CompanyID = 1
            };

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _employeeService.UpdateEmployeeAsync(999, updatedEmployee));
        }

        [Test]
        public void UpdateEmployeeAsync_ThrowsExceptionForNullInput()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _employeeService.UpdateEmployeeAsync(1, null!));
        }

        [Test]
        public async Task GetAllJobTitlesAsync_ReturnsUniqueJobTitles()
        {
            // Act
            var jobTitles = await _employeeService.GetAllJobTitlesAsync();

            // Assert
            Assert.That(jobTitles.Count, Is.EqualTo(2));
            Assert.That(jobTitles, Does.Contain("Software Engineer"));
            Assert.That(jobTitles, Does.Contain("Data Scientist"));
        }

        [Test]
        public async Task GetEmployeesByJobTitleAsync_ReturnsCorrectEmployees()
        {
            // Act
            var employees = await _employeeService.GetEmployeesByJobTitleAsync("Software Engineer");

            // Assert
            Assert.That(employees.Count, Is.EqualTo(1));
            Assert.That(employees[0].JobTitle, Is.EqualTo("Software Engineer"));
        }

        [Test]
        public void GetEmployeesByJobTitleAsync_ThrowsExceptionForNullOrEmptyJobTitle()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _employeeService.GetEmployeesByJobTitleAsync(null!));
            Assert.ThrowsAsync<ArgumentException>(async () => await _employeeService.GetEmployeesByJobTitleAsync(""));
        }

        [Test]
        public async Task DeleteAllEmployeesAsync_RemovesAllEmployees()
        {
            // Act
            await _employeeService.DeleteAllEmployeesAsync();
            var employees = await _employeeService.GetAllEmployeesAsync();

            // Assert
            Assert.That(employees.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllEmployeesAsync_ReturnsEmptyList_WhenDatabaseIsEmpty()
        {
            // Arrange
            await _employeeService.DeleteAllEmployeesAsync(); // Ensure the database is empty

            // Act
            var employees = await _employeeService.GetAllEmployeesAsync();

            // Assert
            Assert.That(employees, Is.Empty);
        }

        [Test]
        public async Task GetAllJobTitlesAsync_ReturnsEmptyList_WhenDatabaseIsEmpty()
        {
            // Arrange
            await _employeeService.DeleteAllEmployeesAsync(); // Ensure the database is empty

            // Act
            var jobTitles = await _employeeService.GetAllJobTitlesAsync();

            // Assert
            Assert.That(jobTitles, Is.Empty);
        }

        [Test]
        public void BulkCreateEmployeesAsync_ThrowsExceptionForNullInput()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _employeeService.BulkCreateEmployeesAsync(null!));
        }

        [Test]
        public async Task DeleteAllEmployeesAsync_ThrowsException_WhenNoEmployeesExist()
        {
            // Arrange
            await _employeeService.DeleteAllEmployeesAsync(); // Ensure the database is empty

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _employeeService.DeleteAllEmployeesAsync());
        }

        [Test]
        public async Task GetSalaryDifferencesByGenderAsync_ReturnsCorrectSalaryDifferences()
        {
            // Arrange
            var jobTitle = "Software Engineer";

            // Act
            var result = await _employeeService.GetSalaryDifferencesByGenderAsync(jobTitle);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContainsKey("Male"), Is.True);
            Assert.That(result["Male"].Count, Is.EqualTo(2)); // Two salary records for the male employee
            Assert.That(result["Male"].Average(s => s.AverageSalary), Is.EqualTo(5250));
        }

        [Test]
        public async Task GetSalaryDifferencesByGenderAsync_ReturnsEmptyForNonExistentJobTitle()
        {
            // Arrange
            var jobTitle = "Non-Existent Job";

            // Act
            var result = await _employeeService.GetSalaryDifferencesByGenderAsync(jobTitle);

            // Assert
            Assert.That(result["Male"].Count, Is.EqualTo(0));
            Assert.That(result["Female"].Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetMaxEmployeeIdAsync_ReturnsCorrectMaxId()
        {
            // Act
            var maxId = await _employeeService.GetMaxEmployeeIdAsync();

            // Assert
            Assert.That(maxId, Is.EqualTo(2)); // The highest EmployeeID in the seeded data
        }

        [Test]
        public async Task GetMaxEmployeeIdAsync_ReturnsZero_WhenNoEmployeesExist()
        {
            // Arrange
            await _employeeService.DeleteAllEmployeesAsync(); // Ensure the database is empty

            // Act
            var maxId = await _employeeService.GetMaxEmployeeIdAsync();

            // Assert
            Assert.That(maxId, Is.EqualTo(0)); // No employees, so max ID should be 0
        }

        [Test]
        public async Task GetAllSalaryDifferencesByGenderAsync_ReturnsCorrectSalaryDifferences()
        {
            // Act
            var result = await _employeeService.GetAllSalaryDifferencesByGenderAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContainsKey("Male"), Is.True);
            Assert.That(result.ContainsKey("Female"), Is.True);

            // Male salary differences
            Assert.That(result["Male"].Count, Is.EqualTo(2)); // Two months of salary records
            Assert.That(result["Male"][0].AverageSalary, Is.EqualTo(5000));
            Assert.That(result["Male"][1].AverageSalary, Is.EqualTo(5500));

            // Female salary differences
            Assert.That(result["Female"].Count, Is.EqualTo(2)); // Two months of salary records
            Assert.That(result["Female"][0].AverageSalary, Is.EqualTo(4800));
            Assert.That(result["Female"][1].AverageSalary, Is.EqualTo(5000));
        }

        [Test]
        public async Task GetAllSalaryDifferencesByGenderAsync_ReturnsEmpty_WhenNoSalariesExist()
        {
            // Arrange
            await _employeeService.DeleteAllEmployeesAsync(); // Ensure the database is empty

            // Act
            var result = await _employeeService.GetAllSalaryDifferencesByGenderAsync();

            // Assert
            Assert.That(result["Male"].Count, Is.EqualTo(0));
            Assert.That(result["Female"].Count, Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteEmployeeAsync_RemovesAssociatedSalaries()
        {
            // Act
            await _employeeService.DeleteEmployeeAsync(1); // Delete the male employee
            var salaries = await _employeeService.GetAllEmployeesAsync();

            // Assert
            Assert.That(salaries.Any(s => s.EmployeeID == 1), Is.False); // Ensure no salaries exist for EmployeeID 1
        }

        [Test]
        public async Task GetEmployeesByJobTitleAsync_ReturnsEmptyList_WhenNoMatchingEmployees()
        {
            // Act
            var employees = await _employeeService.GetEmployeesByJobTitleAsync("Non-Existent Job");

            // Assert
            Assert.That(employees, Is.Empty);
        }

        [Test]
        public async Task GetAllJobTitlesAsync_ReturnsEmptyList_WhenNoEmployeesExist()
        {
            // Arrange
            await _employeeService.DeleteAllEmployeesAsync(); // Ensure the database is empty

            // Act
            var jobTitles = await _employeeService.GetAllJobTitlesAsync();

            // Assert
            Assert.That(jobTitles, Is.Empty);
        }
    }
}