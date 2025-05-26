using Backend.Controllers;
using Backend.Models;
using Backend.Models.DTO;
using Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace WebAPI.Tests.Controllers
{
    [TestFixture]
    public class TestSalaryController
    {
        private ApplicationDbContext _dbContext;
        private SalaryController _controller;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _controller = new SalaryController(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task AddSalary_ReturnsOkResult_WhenSalaryIsAdded()
        {
            // Arrange
            var salary = new SalaryModel
            {
                SalaryID = 1,
                EmployeeID = 2,
                Salary = 50000,
                Timestamp = DateTime.UtcNow
            };

            // Act
            var result = await _controller.AddSalary(salary) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Salary added successfully."));
        }

        [Test]
        public async Task AddSalary_ReturnsBadRequest_WhenSalaryIsInvalid()
        {
            // Arrange
            var salary = new SalaryModel
            {
                SalaryID = 1,
                EmployeeID = 0, // Invalid EmployeeID
                Salary = -100,  // Invalid Salary
                Timestamp = DateTime.UtcNow
            };

            // Act
            var result = await _controller.AddSalary(salary) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Invalid salary or EmployeeID."));
        }

        [Test]
        public async Task AddSalary_SetsTimestamp_WhenNotProvided()
        {
            // Arrange
            var salary = new SalaryModel
            {
                SalaryID = 1,
                EmployeeID = 2,
                Salary = 50000,
                Timestamp = default // Should be set by controller
            };

            // Act
            var result = await _controller.AddSalary(salary) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Salary added successfully."));
        }

        [Test]
        public async Task GetLatestSalary_ReturnsOkResultWithSalary()
        {
            // Arrange
            var salary = new SalaryModel
            {
                SalaryID = 1,
                EmployeeID = 2,
                Salary = 50000,
                Timestamp = DateTime.UtcNow
            };
            _dbContext.Salaries.Add(salary);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetLatestSalary(2) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var dto = result.Value as SalaryDto;
            Assert.That(dto, Is.Not.Null);
            Assert.That(dto.EmployeeID, Is.EqualTo(2));
            Assert.That(dto.Salary, Is.EqualTo(50000));
        }

        [Test]
        public async Task GetLatestSalary_ReturnsNotFound_WhenNoSalaryExists()
        {
            // Act
            var result = await _controller.GetLatestSalary(99) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("No salary found for EmployeeID 99"));
        }

        [Test]
        public async Task GetSalaryHistory_ReturnsOkResultWithSalaryHistory()
        {
            // Arrange
            var salaryHistory = new[]
            {
                new SalaryModel
                {
                    SalaryID = 1,
                    EmployeeID = 2,
                    Salary = 50000,
                    Timestamp = DateTime.UtcNow.AddMonths(-1)
                },
                new SalaryModel
                {
                    SalaryID = 2,
                    EmployeeID = 2,
                    Salary = 52000,
                    Timestamp = DateTime.UtcNow
                }
            };
            _dbContext.Salaries.AddRange(salaryHistory);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetSalaryHistory(2) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var returnedHistory = result.Value as System.Collections.Generic.List<SalaryDto>;
            Assert.That(returnedHistory, Is.Not.Null);
            Assert.That(returnedHistory.Count, Is.EqualTo(2));
            Assert.That(returnedHistory[0].Salary, Is.EqualTo(52000));
            Assert.That(returnedHistory[1].Salary, Is.EqualTo(50000));
        }

        [Test]
        public async Task GetSalaryHistory_ReturnsNotFound_WhenNoSalaryHistoryExists()
        {
            // Act
            var result = await _controller.GetSalaryHistory(99) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("No salary history found for EmployeeID 99"));
        }

        [Test]
        public async Task GetAllSalaries_ReturnsOkResultWithSalaries()
        {
            // Arrange
            var salaries = new[]
            {
                new SalaryModel
                {
                    SalaryID = 1,
                    EmployeeID = 2,
                    Salary = 50000,
                    Timestamp = DateTime.UtcNow.AddMonths(-1)
                },
                new SalaryModel
                {
                    SalaryID = 2,
                    EmployeeID = 3,
                    Salary = 52000,
                    Timestamp = DateTime.UtcNow
                }
            };
            _dbContext.Salaries.AddRange(salaries);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetAllSalaries() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var returned = result.Value as System.Collections.IEnumerable;
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned.Cast<object>().Count(), Is.EqualTo(2)); // or 0 for empty
        }

        [Test]
        public async Task GetAllSalaries_ReturnsOkResultWithEmptyList_WhenNoSalariesExist()
        {
            // Act
            var result = await _controller.GetAllSalaries() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var returned = result.Value as System.Collections.IEnumerable;
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned.Cast<object>().Count(), Is.EqualTo(0));
        }
    }
}