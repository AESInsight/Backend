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
    public class TestSalaryService
    {
        private SalaryService _salaryService;

        [SetUp]
        public void SetUp()
        {
            // Use a unique InMemoryDatabase for each test
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new ApplicationDbContext(options);

            // Seed the database with test data
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
            _salaryService = new SalaryService(dbContext);
        }

        [Test]
        public async Task GetLatestSalaryForEmployeeAsync_ReturnsLatestSalary()
        {
            // Act
            var result = await _salaryService.GetLatestSalaryForEmployeeAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Salary, Is.EqualTo(5500)); // Latest salary for EmployeeID 1
            Assert.That(result.Timestamp, Is.EqualTo(new DateTime(2025, 2, 1)));
        }

        [Test]
        public async Task GetLatestSalaryForEmployeeAsync_ReturnsNullForNonExistentEmployee()
        {
            // Act
            var result = await _salaryService.GetLatestSalaryForEmployeeAsync(999);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetSalaryHistoryAsync_ReturnsSalaryHistory()
        {
            // Act
            var result = await _salaryService.GetSalaryHistoryAsync(1);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2)); // Two salary records for EmployeeID 1
            Assert.That(result[0].Salary, Is.EqualTo(5500)); // Latest salary first
            Assert.That(result[1].Salary, Is.EqualTo(5000)); // Older salary second
        }

        [Test]
        public async Task GetSalaryHistoryAsync_ReturnsEmptyListForNonExistentEmployee()
        {
            // Act
            var result = await _salaryService.GetSalaryHistoryAsync(999);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task AddSalaryAsync_AddsNewSalary()
        {
            // Arrange
            var newSalary = new SalaryModel
            {
                EmployeeID = 1,
                Salary = 6000
            };

            // Act
            var result = await _salaryService.AddSalaryAsync(newSalary);
            var latestSalary = await _salaryService.GetLatestSalaryForEmployeeAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Salary, Is.EqualTo(6000));
            Assert.That(latestSalary, Is.Not.Null, "Latest salary should not be null");
            Assert.That(latestSalary!.Salary, Is.EqualTo(6000)); // Ensure it's the latest salary
        }
    }
}