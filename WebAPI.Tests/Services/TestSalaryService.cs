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
        private ApplicationDbContext _dbContext;
        private SalaryService _service;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);
            // Seed the database with test data (from HEAD)
            _dbContext.Employee.AddRange(new List<EmployeeModel>
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
            _dbContext.Salaries.AddRange(new List<SalaryModel>
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
            _dbContext.SaveChanges();
            _service = new SalaryService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task GetLatestSalaryForEmployeeAsync_ReturnsLatestSalary()
        {
            // Act
            var result = await _service.GetLatestSalaryForEmployeeAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Salary, Is.EqualTo(5500)); // Latest salary for EmployeeID 1
            Assert.That(result.Timestamp, Is.EqualTo(new DateTime(2025, 2, 1)));
        }

        [Test]
        public async Task GetLatestSalaryForEmployeeAsync_ReturnsNullForNonExistentEmployee()
        {
            // Act
            var result = await _service.GetLatestSalaryForEmployeeAsync(999);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetSalaryHistoryAsync_ReturnsSalaryHistory()
        {
            // Act
            var result = await _service.GetSalaryHistoryAsync(1);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2)); // Two salary records for EmployeeID 1
            Assert.That(result[0].Salary, Is.EqualTo(5500)); // Latest salary first
            Assert.That(result[1].Salary, Is.EqualTo(5000)); // Older salary second
        }

        [Test]
        public async Task GetSalaryHistoryAsync_ReturnsEmptyListForNonExistentEmployee()
        {
            // Act
            var result = await _service.GetSalaryHistoryAsync(999);

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
            var result = await _service.AddSalaryAsync(newSalary);
            var latestSalary = await _service.GetLatestSalaryForEmployeeAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Salary, Is.EqualTo(6000));
            Assert.That(latestSalary, Is.Not.Null, "Latest salary should not be null");
            Assert.That(latestSalary!.Salary, Is.EqualTo(6000)); // Ensure it's the latest salary
        }

        [Test]
        public async Task GetLatestSalaryForEmployeeAsync_ReturnsSalary_WhenOnlyOneSalaryExists()
        {
            // Arrange
            var employeeId = 2;
            var salary = new SalaryModel { EmployeeID = employeeId, Salary = 42000, Timestamp = DateTime.UtcNow };
            _dbContext.Salaries.Add(salary);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetLatestSalaryForEmployeeAsync(employeeId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Salary, Is.EqualTo(42000));
        }

        [Test]
        public async Task GetLatestSalaryForEmployeeAsync_IgnoresSalariesOfOtherEmployees()
        {
            // Arrange
            var employeeId = 3;
            var otherEmployeeId = 4;
            var salary1 = new SalaryModel { EmployeeID = employeeId, Salary = 30000, Timestamp = DateTime.UtcNow.AddDays(-1) };
            var salary2 = new SalaryModel { EmployeeID = otherEmployeeId, Salary = 99999, Timestamp = DateTime.UtcNow };
            _dbContext.Salaries.AddRange(salary1, salary2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetLatestSalaryForEmployeeAsync(employeeId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Salary, Is.EqualTo(30000));
            Assert.That(result.EmployeeID, Is.EqualTo(employeeId));
        }

        [Test]
        public async Task GetLatestSalaryForEmployeeAsync_ReturnsFirst_WhenMultipleWithSameTimestamp()
        {
            // Arrange
            var employeeId = 5;
            var timestamp = DateTime.UtcNow;
            var salary1 = new SalaryModel { EmployeeID = employeeId, Salary = 10000, Timestamp = timestamp };
            var salary2 = new SalaryModel { EmployeeID = employeeId, Salary = 20000, Timestamp = timestamp };
            _dbContext.Salaries.AddRange(salary1, salary2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetLatestSalaryForEmployeeAsync(employeeId);

            // Assert
            Assert.That(result, Is.Not.Null);
            // Should return the first one it finds with the latest timestamp (implementation detail)
            Assert.That(result.Timestamp, Is.EqualTo(timestamp));
            Assert.That(new[] { 10000.0, 20000.0 }, Does.Contain(result.Salary));
        }

        [Test]
        public async Task GetSalaryHistoryAsync_ReturnsAllSalariesOrderedByTimestamp()
        {
            // Arrange
            var employeeId = 10;
            var salary1 = new SalaryModel { EmployeeID = employeeId, Salary = 40000, Timestamp = DateTime.UtcNow.AddDays(-3) };
            var salary2 = new SalaryModel { EmployeeID = employeeId, Salary = 45000, Timestamp = DateTime.UtcNow.AddDays(-2) };
            var salary3 = new SalaryModel { EmployeeID = employeeId, Salary = 47000, Timestamp = DateTime.UtcNow.AddDays(-1) };
            _dbContext.Salaries.AddRange(salary1, salary2, salary3);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetSalaryHistoryAsync(employeeId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].Salary, Is.EqualTo(47000)); // Most recent first
            Assert.That(result[1].Salary, Is.EqualTo(45000));
            Assert.That(result[2].Salary, Is.EqualTo(40000));
        }

        [Test]
        public async Task GetSalaryHistoryAsync_ReturnsEmptyList_WhenNoSalaries()
        {
            // Act
            var result = await _service.GetSalaryHistoryAsync(999);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetSalaryHistoryAsync_ReturnsOnlySalariesForSpecifiedEmployee()
        {
            // Arrange
            var employeeId = 20;
            var otherEmployeeId = 21;
            var salary1 = new SalaryModel { EmployeeID = employeeId, Salary = 30000, Timestamp = DateTime.UtcNow.AddDays(-2) };
            var salary2 = new SalaryModel { EmployeeID = otherEmployeeId, Salary = 99999, Timestamp = DateTime.UtcNow.AddDays(-1) };
            var salary3 = new SalaryModel { EmployeeID = employeeId, Salary = 35000, Timestamp = DateTime.UtcNow };
            _dbContext.Salaries.AddRange(salary1, salary2, salary3);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetSalaryHistoryAsync(employeeId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(s => s.EmployeeID == employeeId), Is.True);
            Assert.That(result[0].Salary, Is.EqualTo(35000));
            Assert.That(result[1].Salary, Is.EqualTo(30000));
        }

        [Test]
        public async Task GetSalaryHistoryAsync_ReturnsSingleSalary_WhenOnlyOneExists()
        {
            // Arrange
            var employeeId = 30;
            var salary = new SalaryModel { EmployeeID = employeeId, Salary = 12345, Timestamp = DateTime.UtcNow };
            _dbContext.Salaries.Add(salary);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetSalaryHistoryAsync(employeeId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Salary, Is.EqualTo(12345));
            Assert.That(result[0].EmployeeID, Is.EqualTo(employeeId));
        }

        [Test]
        public async Task GetSalaryHistoryAsync_ReturnsSalariesWithSameTimestamp_InAnyOrder()
        {
            // Arrange
            var employeeId = 40;
            var timestamp = DateTime.UtcNow;
            var salary1 = new SalaryModel { EmployeeID = employeeId, Salary = 10000, Timestamp = timestamp };
            var salary2 = new SalaryModel { EmployeeID = employeeId, Salary = 20000, Timestamp = timestamp };
            _dbContext.Salaries.AddRange(salary1, salary2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetSalaryHistoryAsync(employeeId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Timestamp, Is.EqualTo(timestamp));
            Assert.That(result[1].Timestamp, Is.EqualTo(timestamp));
            Assert.That(new[] { 10000.0, 20000.0 }, Does.Contain(result[0].Salary));
            Assert.That(new[] { 10000.0, 20000.0 }, Does.Contain(result[1].Salary));
        }

        [Test]
        public async Task AddSalaryAsync_AddsSalaryAndSetsTimestamp()
        {
            // Arrange
            var salary = new SalaryModel { EmployeeID = 100, Salary = 55555 };

            // Act
            var result = await _service.AddSalaryAsync(salary);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SalaryID, Is.Not.EqualTo(0));
            Assert.That(result.Timestamp, Is.Not.EqualTo(default(DateTime)));
            var dbSalary = await _dbContext.Salaries.FindAsync(result.SalaryID);
            Assert.That(dbSalary, Is.Not.Null);
            Assert.That(dbSalary.Salary, Is.EqualTo(55555));
            Assert.That(dbSalary.EmployeeID, Is.EqualTo(100));
        }

        [Test]
        public async Task AddSalaryAsync_AllowsZeroSalary()
        {
            // Arrange
            var salary = new SalaryModel { EmployeeID = 101, Salary = 0 };

            // Act
            var result = await _service.AddSalaryAsync(salary);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Salary, Is.EqualTo(0));
            Assert.That(result.EmployeeID, Is.EqualTo(101));
        }

        [Test]
        public async Task AddSalaryAsync_AllowsNegativeSalary()
        {
            // Arrange
            var salary = new SalaryModel { EmployeeID = 102, Salary = -12345 };

            // Act
            var result = await _service.AddSalaryAsync(salary);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Salary, Is.EqualTo(-12345));
            Assert.That(result.EmployeeID, Is.EqualTo(102));
        }

        [Test]
        public async Task AddSalaryAsync_SetsTimestampToUtcNow()
        {
            // Arrange
            var salary = new SalaryModel { EmployeeID = 103, Salary = 12345 };
            var before = DateTime.UtcNow.AddSeconds(-1);

            // Act
            var result = await _service.AddSalaryAsync(salary);

            // Assert
            var after = DateTime.UtcNow.AddSeconds(1);
            Assert.That(result.Timestamp, Is.GreaterThanOrEqualTo(before));
            Assert.That(result.Timestamp, Is.LessThanOrEqualTo(after));
        }

        [Test]
        public async Task AddSalaryAsync_AddsMultipleSalariesForSameEmployee()
        {
            // Arrange
            var employeeId = 104;
            var salary1 = new SalaryModel { EmployeeID = employeeId, Salary = 1000 };
            var salary2 = new SalaryModel { EmployeeID = employeeId, Salary = 2000 };

            // Act
            var result1 = await _service.AddSalaryAsync(salary1);
            var result2 = await _service.AddSalaryAsync(salary2);

            // Assert
            Assert.That(result1.SalaryID, Is.Not.EqualTo(result2.SalaryID));
            Assert.That(result1.EmployeeID, Is.EqualTo(employeeId));
            Assert.That(result2.EmployeeID, Is.EqualTo(employeeId));
            Assert.That((await _dbContext.Salaries.Where(s => s.EmployeeID == employeeId).CountAsync()), Is.EqualTo(2));
        }
    }
}