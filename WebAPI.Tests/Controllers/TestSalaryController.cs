using Backend.Controllers;
using Backend.Models;
using Backend.Models.DTO;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Tests.Controllers
{
    [TestFixture]
    public class TestSalaryController
    {
        private Mock<ISalaryService> _salaryServiceMock; // Mock for SalaryService
        private SalaryController _controller;

        [SetUp]
        public void SetUp()
        {
            _salaryServiceMock = new Mock<ISalaryService>();
            _controller = new SalaryController(_salaryServiceMock.Object);
        }

        [Test]
        public async Task AddSalary_ReturnsOkResult()
        {
            // Arrange
            var salary = new SalaryModel
            {
                SalaryID = 1,
                EmployeeID = 1,
                Salary = 50000,
                Timestamp = DateTime.UtcNow
            };
            _salaryServiceMock
                .Setup(s => s.AddSalaryAsync(It.IsAny<SalaryModel>()))
                .ReturnsAsync(salary);

            // Act
            var result = await _controller.AddSalary(salary) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("Salary added successfully."));
            Assert.That(response["data"], Is.Not.Null);

            var data = response["data"] as SalaryModel;
            Assert.That(data, Is.Not.Null);
            Assert.That(data.SalaryID, Is.EqualTo(1));
            Assert.That(data.EmployeeID, Is.EqualTo(1));
            Assert.That(data.Salary, Is.EqualTo(50000));
        }

        [Test]
        public async Task AddSalary_ReturnsBadRequest_WhenSalaryOrEmployeeIdIsInvalid()
        {
            // Arrange
            var invalidSalary = new SalaryModel
            {
                SalaryID = 1,
                EmployeeID = 0, // Invalid EmployeeID
                Salary = -100,  // Invalid Salary
                Timestamp = DateTime.UtcNow
            };

            // Act
            var result = await _controller.AddSalary(invalidSalary) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response["message"], Is.EqualTo("Invalid salary or EmployeeID."));
        }

        [Test]
        public async Task AddSalary_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var salary = new SalaryModel
            {
                SalaryID = 1,
                EmployeeID = 1,
                Salary = 50000,
                Timestamp = DateTime.UtcNow
            };

            _salaryServiceMock
                .Setup(s => s.AddSalaryAsync(It.IsAny<SalaryModel>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            IActionResult? result = null;
            try
            {
                result = await _controller.AddSalary(salary);
            }
            catch
            {
                // If the controller does not handle the exception, the test will fail here.
                Assert.Fail("Exception was not handled in the controller.");
            }

            // Assert
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult.Value, Is.Not.Null);

            var response = objectResult.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("error"), Is.True);
            Assert.That(response["error"], Is.EqualTo("An error occurred while adding the salary"));
            Assert.That(response.ContainsKey("details"), Is.True);
            Assert.That(response["details"], Is.EqualTo("Unexpected error"));
        }

        [Test]
        public async Task GetLatestSalary_ReturnsOkResultWithSalary()
        {
            // Arrange
            var employeeId = 1;
            var salary = new SalaryModel
            {
                SalaryID = 10,
                EmployeeID = employeeId,
                Salary = 60000,
                Timestamp = DateTime.UtcNow
            };
            _salaryServiceMock
                .Setup(s => s.GetLatestSalaryForEmployeeAsync(employeeId))
                .ReturnsAsync(salary);

            // Act
            var result = await _controller.GetLatestSalary(employeeId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("data"), Is.True);

            var dto = response["data"] as SalaryDto;
            Assert.That(dto, Is.Not.Null);
            Assert.That(dto.SalaryID, Is.EqualTo(10));
            Assert.That(dto.EmployeeID, Is.EqualTo(employeeId));
            Assert.That(dto.Salary, Is.EqualTo(60000));
        }

        [Test]
        public async Task GetLatestSalary_ReturnsNotFoundWhenNoSalaryExists()
        {
            // Arrange
            var employeeId = 1;
            _salaryServiceMock
                .Setup(s => s.GetLatestSalaryForEmployeeAsync(employeeId))
                .ReturnsAsync((SalaryModel?)null);

            // Act
            var result = await _controller.GetLatestSalary(employeeId) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response["message"], Is.EqualTo($"No salary found for EmployeeID {employeeId}"));
        }

        [Test]
        public async Task GetLatestSalary_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var employeeId = 1;
            _salaryServiceMock
                .Setup(s => s.GetLatestSalaryForEmployeeAsync(employeeId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            IActionResult? result = null;
            try
            {
                result = await _controller.GetLatestSalary(employeeId);
            }
            catch
            {
                Assert.Fail("Exception was not handled in the controller.");
            }

            // Assert
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult.Value, Is.Not.Null);

            var response = objectResult.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("error"), Is.True);
            Assert.That(response["error"], Is.EqualTo("An error occurred while retrieving the latest salary"));
            Assert.That(response.ContainsKey("details"), Is.True);
            Assert.That(response["details"], Is.EqualTo("Unexpected error"));
        }

        [Test]
        public async Task GetSalaryHistory_ReturnsOkResultWithSalaryHistory()
        {
            // Arrange
            var employeeId = 1;
            var salaryHistory = new List<SalaryModel>
            {
                new SalaryModel
                {
                    SalaryID = 1,
                    EmployeeID = employeeId,
                    Salary = 50000,
                    Timestamp = DateTime.UtcNow.AddMonths(-2)
                },
                new SalaryModel
                {
                    SalaryID = 2,
                    EmployeeID = employeeId,
                    Salary = 55000,
                    Timestamp = DateTime.UtcNow.AddMonths(-1)
                }
            };
            _salaryServiceMock
                .Setup(s => s.GetSalaryHistoryAsync(employeeId))
                .ReturnsAsync(salaryHistory);

            // Act
            var result = await _controller.GetSalaryHistory(employeeId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("data"), Is.True);

            var dtos = response["data"] as List<SalaryDto>;
            Assert.That(dtos, Is.Not.Null);
            Assert.That(dtos.Count, Is.EqualTo(2));
            Assert.That(dtos.Any(d => d.SalaryID == 1 && d.Salary == 50000), Is.True);
            Assert.That(dtos.Any(d => d.SalaryID == 2 && d.Salary == 55000), Is.True);
        }

        [Test]
        public async Task GetSalaryHistory_ReturnsNotFoundWhenNoHistoryExists()
        {
            // Arrange
            var employeeId = 1;
            var salaryHistory = new List<SalaryModel>(); // No history
            _salaryServiceMock
                .Setup(s => s.GetSalaryHistoryAsync(employeeId))
                .ReturnsAsync(salaryHistory);

            // Act
            var result = await _controller.GetSalaryHistory(employeeId) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response["message"], Is.EqualTo($"No salary history found for EmployeeID {employeeId}"));
        }

        [Test]
        public async Task GetSalaryHistory_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var employeeId = 1;
            _salaryServiceMock
                .Setup(s => s.GetSalaryHistoryAsync(employeeId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            IActionResult? result = null;
            try
            {
                result = await _controller.GetSalaryHistory(employeeId);
            }
            catch
            {
                Assert.Fail("Exception was not handled in the controller.");
            }

            // Assert
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult.Value, Is.Not.Null);

            var response = objectResult.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("error"), Is.True);
            Assert.That(response["error"], Is.EqualTo("An error occurred while retrieving salary history"));
            Assert.That(response.ContainsKey("details"), Is.True);
            Assert.That(response["details"], Is.EqualTo("Unexpected error"));
        }
    }
}