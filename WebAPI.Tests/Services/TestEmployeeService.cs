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
                    PasswordHash = "hashedpassword1"
                },
                new CompanyModel
                {
                    CompanyID = 2,
                    CompanyName = "DataCorp",
                    Industry = "Data Analytics",
                    CVR = "87654321",
                    Email = "info@datacorp.com",
                    PasswordHash = "hashedpassword2"
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
    }
}