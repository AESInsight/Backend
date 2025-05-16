using Backend.Controllers;
using Backend.Models;
using Backend.Models.DTO;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Tests.Controllers
{
    [TestFixture]
    public class TestEmployeeController
    {
        private Mock<IEmployeeService> _employeeServiceMock;
        private EmployeeController _controller;

        [SetUp]
        public void SetUp()
        {
            _employeeServiceMock = new Mock<IEmployeeService>();
            _controller = new EmployeeController(_employeeServiceMock.Object);
        }

        [Test]
        public async Task GetEmployeeById_ReturnsOkResultWithEmployee()
        {
            // Arrange
            var employee = new EmployeeModel
            {
                EmployeeID = 1,
                JobTitle = "Software Engineer",
                Experience = 5,
                Gender = "Male",
                CompanyID = 1
            };
            _employeeServiceMock.Setup(s => s.GetEmployeeByIdAsync(1)).ReturnsAsync(employee);

            // Act
            var result = await _controller.GetEmployeeById(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("data"), Is.True);

            var employeeDto = response["data"] as EmployeeDto;
            Assert.That(employeeDto, Is.Not.Null);
            Assert.That(employeeDto.EmployeeID, Is.EqualTo(1));
            Assert.That(employeeDto.JobTitle, Is.EqualTo("Software Engineer"));
            Assert.That(employeeDto.Experience, Is.EqualTo(5));
            Assert.That(employeeDto.Gender, Is.EqualTo("Male"));
            Assert.That(employeeDto.CompanyID, Is.EqualTo(1));
        }

        [Test]
        public async Task GetEmployeeById_ReturnsNotFoundWhenEmployeeIsNull()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetEmployeeByIdAsync(1)).ReturnsAsync((EmployeeModel)null!);

            // Act
            var result = await _controller.GetEmployeeById(1) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response["message"], Is.EqualTo("Employee with ID 1 not found"));
        }

        [Test]
        public async Task GetEmployeeById_ReturnsNotFoundWhenKeyNotFoundExceptionIsThrown()
        {
            // Arrange
            _employeeServiceMock
                .Setup(s => s.GetEmployeeByIdAsync(1))
                .ThrowsAsync(new KeyNotFoundException("Employee not found"));

            // Act
            var result = await _controller.GetEmployeeById(1) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("error"), Is.True);
            Assert.That(response["error"], Is.EqualTo("Employee not found"));
        }

        [Test]
        public async Task GetEmployeeById_ReturnsInternalServerErrorWhenExceptionIsThrown()
        {
            // Arrange
            _employeeServiceMock
                .Setup(s => s.GetEmployeeByIdAsync(1))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetEmployeeById(1) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("error"), Is.True);
            Assert.That(response["error"], Is.EqualTo("An error occurred while retrieving the employee"));
            Assert.That(response.ContainsKey("details"), Is.True);
            Assert.That(response["details"], Is.EqualTo("Unexpected error"));
        }
        [Test]
        public async Task GetEmployeesByCompanyId_ReturnsOkResultWithEmployees()
        {
            // Arrange
            var companyId = 1;
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel
                {
                    EmployeeID = 1,
                    JobTitle = "Software Engineer",
                    Experience = 5,
                    Gender = "Male",
                    CompanyID = companyId
                },
                new EmployeeModel
                {
                    EmployeeID = 2,
                    JobTitle = "Data Analyst",
                    Experience = 3,
                    Gender = "Female",
                    CompanyID = companyId
                }
            };

            _employeeServiceMock.Setup(s => s.GetAllEmployeesAsync()).ReturnsAsync(employees);

            // Act
            var result = await _controller.GetEmployeesByCompanyId(companyId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("data"), Is.True);

            var employeeDtos = response["data"] as List<EmployeeDto>;
            Assert.That(employeeDtos, Is.Not.Null);
            Assert.That(employeeDtos.Count, Is.EqualTo(2));
            Assert.That(employeeDtos.Any(e => e.JobTitle == "Software Engineer"), Is.True);
            Assert.That(employeeDtos.Any(e => e.JobTitle == "Data Analyst"), Is.True);
        }

        [Test]
        public async Task GetEmployeesByCompanyId_ReturnsNotFoundWhenNoEmployeesExist()
        {
            // Arrange
            var companyId = 1;
            var employees = new List<EmployeeModel>(); // No employees for the company

            _employeeServiceMock.Setup(s => s.GetAllEmployeesAsync()).ReturnsAsync(employees);

            // Act
            var result = await _controller.GetEmployeesByCompanyId(companyId) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response["message"], Is.EqualTo($"No employees found for CompanyID {companyId}"));
        }

        [Test]
        public async Task GetEmployeesByCompanyId_ReturnsInternalServerErrorWhenExceptionIsThrown()
        {
            // Arrange
            var companyId = 1;

            _employeeServiceMock
                .Setup(s => s.GetAllEmployeesAsync())
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetEmployeesByCompanyId(companyId) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("error"), Is.True);
            Assert.That(response["error"], Is.EqualTo("An error occurred while retrieving employees"));
            Assert.That(response.ContainsKey("details"), Is.True);
            Assert.That(response["details"], Is.EqualTo("Unexpected error"));
        }

        [Test]
        public async Task BulkUploadEmployees_ReturnsOkResultWithProcessedCount()
        {
            // Arrange
            var employees = new List<EmployeeModel>
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
                    JobTitle = "Data Analyst",
                    Experience = 3,
                    Gender = "Female",
                    CompanyID = 1
                }
            };

            _employeeServiceMock
                .Setup(s => s.BulkCreateEmployeesAsync(employees))
                .ReturnsAsync(employees);

            // Act
            var result = await _controller.BulkUploadEmployees(employees) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response.ContainsKey("processedCount"), Is.True);
            Assert.That(response["processedCount"], Is.EqualTo(2));
        }

        [Test]
        public async Task BulkUploadEmployees_ReturnsBadRequestWhenInputIsNull()
        {
            // Arrange
            List<EmployeeModel>? employees = null;

            // Act
            var result = await _controller.BulkUploadEmployees(employees!) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response["message"], Is.EqualTo("No employees provided"));
        }

        [Test]
        public async Task BulkUploadEmployees_ReturnsBadRequestWhenInputIsEmpty()
        {
            // Arrange
            var employees = new List<EmployeeModel>();

            // Act
            var result = await _controller.BulkUploadEmployees(employees) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response["message"], Is.EqualTo("No employees provided"));
        }

        [Test]
        public async Task BulkUploadEmployees_ReturnsInternalServerErrorWhenExceptionIsThrown()
        {
            // Arrange
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel
                {
                    EmployeeID = 1,
                    JobTitle = "Software Engineer",
                    Experience = 5,
                    Gender = "Male",
                    CompanyID = 1
                }
            };

            _employeeServiceMock
                .Setup(s => s.BulkCreateEmployeesAsync(employees))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.BulkUploadEmployees(employees) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("error"), Is.True);
            Assert.That(response["error"], Is.EqualTo("An error occurred while processing the bulk upload"));
            Assert.That(response.ContainsKey("details"), Is.True);
            Assert.That(response["details"], Is.EqualTo("Unexpected error"));
        }

        [Test]
        public async Task UpdateEmployee_ReturnsOkResultWhenUpdateIsSuccessful()
        {
            // Arrange
            var employeeId = 1;
            var updatedEmployee = new EmployeeModel
            {
                EmployeeID = employeeId,
                JobTitle = "Senior Software Engineer",
                Experience = 7,
                Gender = "Male",
                CompanyID = 1
            };

            _employeeServiceMock
                .Setup(s => s.UpdateEmployeeAsync(employeeId, updatedEmployee))
                .ReturnsAsync(updatedEmployee);

            // Act
            var result = await _controller.UpdateEmployee(employeeId, updatedEmployee) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response["message"], Is.EqualTo("Employee updated successfully"));
            Assert.That(response.ContainsKey("employee"), Is.True);

            var employee = response["employee"] as EmployeeModel;
            Assert.That(employee, Is.Not.Null);
            Assert.That(employee.EmployeeID, Is.EqualTo(employeeId));
            Assert.That(employee.JobTitle, Is.EqualTo("Senior Software Engineer"));
            Assert.That(employee.Experience, Is.EqualTo(7));
            Assert.That(employee.Gender, Is.EqualTo("Male"));
            Assert.That(employee.CompanyID, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateEmployee_ReturnsBadRequestWhenInputIsInvalid()
        {
            // Arrange
            var employeeId = 0; // Invalid ID
            EmployeeModel? updatedEmployee = null; // Null input

            // Act
            var result = await _controller.UpdateEmployee(employeeId, updatedEmployee!) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response["message"], Is.EqualTo("Invalid employee data or ID"));
        }

        [Test]
        public async Task UpdateEmployee_ReturnsNotFoundWhenEmployeeDoesNotExist()
        {
            // Arrange
            var employeeId = 1;
            var updatedEmployee = new EmployeeModel
            {
                EmployeeID = employeeId,
                JobTitle = "Senior Software Engineer",
                Experience = 7,
                Gender = "Male",
                CompanyID = 1
            };

            _employeeServiceMock
                .Setup(s => s.UpdateEmployeeAsync(employeeId, updatedEmployee))
                .ThrowsAsync(new KeyNotFoundException("Employee not found"));

            // Act
            var result = await _controller.UpdateEmployee(employeeId, updatedEmployee) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("error"), Is.True);
            Assert.That(response["error"], Is.EqualTo("Employee not found"));
        }

        [Test]
        public async Task UpdateEmployee_ReturnsInternalServerErrorWhenExceptionIsThrown()
        {
            // Arrange
            var employeeId = 1;
            var updatedEmployee = new EmployeeModel
            {
                EmployeeID = employeeId,
                JobTitle = "Senior Software Engineer",
                Experience = 7,
                Gender = "Male",
                CompanyID = 1
            };

            _employeeServiceMock
                .Setup(s => s.UpdateEmployeeAsync(employeeId, updatedEmployee))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.UpdateEmployee(employeeId, updatedEmployee) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("error"), Is.True);
            Assert.That(response["error"], Is.EqualTo("An error occurred while updating the employee"));
            Assert.That(response.ContainsKey("details"), Is.True);
            Assert.That(response["details"], Is.EqualTo("Unexpected error"));
        }

        [Test]
        public async Task DeleteEmployee_ReturnsOkResultWhenDeletionIsSuccessful()
        {
            // Arrange
            var employeeId = 1;
            var deletedEmployee = new EmployeeModel
            {
                EmployeeID = employeeId,
                JobTitle = "Software Engineer",
                Experience = 5,
                Gender = "Male",
                CompanyID = 1
            };

            _employeeServiceMock
                .Setup(s => s.DeleteEmployeeAsync(employeeId))
                .ReturnsAsync(deletedEmployee);

            // Act
            var result = await _controller.DeleteEmployee(employeeId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response["message"], Is.EqualTo("Employee deleted successfully"));
            Assert.That(response.ContainsKey("employee"), Is.True);

            var employee = response["employee"] as EmployeeModel;
            Assert.That(employee, Is.Not.Null);
            Assert.That(employee.EmployeeID, Is.EqualTo(employeeId));
            Assert.That(employee.JobTitle, Is.EqualTo("Software Engineer"));
            Assert.That(employee.Experience, Is.EqualTo(5));
            Assert.That(employee.Gender, Is.EqualTo("Male"));
            Assert.That(employee.CompanyID, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteEmployee_ReturnsBadRequestWhenIdIsInvalid()
        {
            // Arrange
            var employeeId = 0; // Invalid ID

            // Act
            var result = await _controller.DeleteEmployee(employeeId) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response["message"], Is.EqualTo("Invalid employee ID"));
        }

        [Test]
        public async Task DeleteEmployee_ReturnsNotFoundWhenEmployeeDoesNotExist()
        {
            // Arrange
            var employeeId = 1;

            _employeeServiceMock
                .Setup(s => s.DeleteEmployeeAsync(employeeId))
                .ThrowsAsync(new KeyNotFoundException("Employee not found"));

            // Act
            var result = await _controller.DeleteEmployee(employeeId) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("error"), Is.True);
            Assert.That(response["error"], Is.EqualTo("Employee not found"));
        }

        [Test]
        public async Task DeleteEmployee_ReturnsInternalServerErrorWhenExceptionIsThrown()
        {
            // Arrange
            var employeeId = 1;

            _employeeServiceMock
                .Setup(s => s.DeleteEmployeeAsync(employeeId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.DeleteEmployee(employeeId) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("error"), Is.True);
            Assert.That(response["error"], Is.EqualTo("An error occurred while deleting the employee"));
            Assert.That(response.ContainsKey("details"), Is.True);
            Assert.That(response["details"], Is.EqualTo("Unexpected error"));
        }

        [Test]
        public async Task DeleteAllEmployees_ReturnsOkResultWhenDeletionIsSuccessful()
        {
            // Arrange
            _employeeServiceMock
                .Setup(s => s.DeleteAllEmployeesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteAllEmployees() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response["message"], Is.EqualTo("All employees have been deleted successfully."));
        }

        [Test]
        public async Task DeleteAllEmployees_ReturnsInternalServerErrorWhenExceptionIsThrown()
        {
            // Arrange
            _employeeServiceMock
                .Setup(s => s.DeleteAllEmployeesAsync())
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.DeleteAllEmployees() as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("error"), Is.True);
            Assert.That(response["error"], Is.EqualTo("An error occurred while deleting employees"));
            Assert.That(response.ContainsKey("details"), Is.True);
            Assert.That(response["details"], Is.EqualTo("Unexpected error"));
        }

        [Test]
        public async Task GetAllJobTitles_ReturnsOkResultWithJobTitles()
        {
            // Arrange
            var jobTitles = new List<string> { "Software Engineer", "Data Analyst" };

            _employeeServiceMock
                .Setup(s => s.GetAllJobTitlesAsync())
                .ReturnsAsync(jobTitles);

            // Act
            var result = await _controller.GetAllJobTitles() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("data"), Is.True);

            var jobTitlesResult = response["data"] as List<string>;
            Assert.That(jobTitlesResult, Is.Not.Null);
            Assert.That(jobTitlesResult.Count, Is.EqualTo(2));
            Assert.That(jobTitlesResult.Contains("Software Engineer"), Is.True);
            Assert.That(jobTitlesResult.Contains("Data Analyst"), Is.True);
        }

        [Test]
        public async Task GetAllJobTitles_ReturnsNotFoundWhenNoJobTitlesExist()
        {
            // Arrange
            var jobTitles = new List<string>(); // No job titles

            _employeeServiceMock
                .Setup(s => s.GetAllJobTitlesAsync())
                .ReturnsAsync(jobTitles);

            // Act
            var result = await _controller.GetAllJobTitles() as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response["message"], Is.EqualTo("No job titles found"));
        }

        [Test]
        public async Task GetAllJobTitles_ReturnsInternalServerErrorWhenExceptionIsThrown()
        {
            // Arrange
            _employeeServiceMock
                .Setup(s => s.GetAllJobTitlesAsync())
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetAllJobTitles() as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("error"), Is.True);
            Assert.That(response["error"], Is.EqualTo("An error occurred while retrieving job titles"));
            Assert.That(response.ContainsKey("details"), Is.True);
            Assert.That(response["details"], Is.EqualTo("Unexpected error"));
        }

        [Test]
        public async Task GetEmployeesByJobTitle_ReturnsOkResultWithEmployees()
        {
            // Arrange
            var jobTitle = "Software Engineer";
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel
                {
                    EmployeeID = 1,
                    JobTitle = jobTitle,
                    Experience = 5,
                    Gender = "Male",
                    CompanyID = 1
                },
                new EmployeeModel
                {
                    EmployeeID = 2,
                    JobTitle = jobTitle,
                    Experience = 3,
                    Gender = "Female",
                    CompanyID = 1
                }
            };

            _employeeServiceMock
                .Setup(s => s.GetEmployeesByJobTitleAsync(jobTitle))
                .ReturnsAsync(employees);

            // Act
            var result = await _controller.GetEmployeesByJobTitle(jobTitle) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("data"), Is.True);

            var employeeDtos = response["data"] as List<EmployeeModel>; // Adjusted to EmployeeModel
            Assert.That(employeeDtos, Is.Not.Null);
            Assert.That(employeeDtos.Count, Is.EqualTo(2));
            Assert.That(employeeDtos.Any(e => e.JobTitle == "Software Engineer"), Is.True);
        }

        [Test]
        public async Task GetEmployeesByJobTitle_ReturnsNotFoundWhenNoEmployeesExist()
        {
            // Arrange
            var jobTitle = "Nonexistent Job Title";
            var employees = new List<EmployeeModel>(); // No employees with the given job title

            _employeeServiceMock
                .Setup(s => s.GetEmployeesByJobTitleAsync(jobTitle))
                .ReturnsAsync(employees);

            // Act
            var result = await _controller.GetEmployeesByJobTitle(jobTitle) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response["message"], Is.EqualTo($"No employees found with job title: {jobTitle}"));
        }

        [Test]
        public async Task GetEmployeesByJobTitle_ReturnsInternalServerErrorWhenExceptionIsThrown()
        {
            // Arrange
            var jobTitle = "Software Engineer";

            _employeeServiceMock
                .Setup(s => s.GetEmployeesByJobTitleAsync(jobTitle))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetEmployeesByJobTitle(jobTitle) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("error"), Is.True);
            Assert.That(response["error"], Is.EqualTo("An error occurred while retrieving employees"));
            Assert.That(response.ContainsKey("details"), Is.True);
            Assert.That(response["details"], Is.EqualTo("Unexpected error"));
        }

        [Test]
        public async Task GetSalaryDifferencesByGender_ReturnsOkResultWithSalaryDifferences()
        {
            // Arrange
            var jobTitle = "Software Engineer";
            var salaryDifferences = new Dictionary<string, List<SalaryDifferenceDTO>>
            {
                { "Male", new List<SalaryDifferenceDTO> { new SalaryDifferenceDTO { AverageSalary = 80000 } } },
                { "Female", new List<SalaryDifferenceDTO> { new SalaryDifferenceDTO { AverageSalary = 75000 } } }
            };

            _employeeServiceMock
                .Setup(s => s.GetSalaryDifferencesByGenderAsync(jobTitle))
                .ReturnsAsync(salaryDifferences);

            // Act
            var result = await _controller.GetSalaryDifferencesByGender(jobTitle) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("data"), Is.True);

            var data = response["data"] as Dictionary<string, List<SalaryDifferenceDTO>>;
            Assert.That(data, Is.Not.Null);
            Assert.That(data.ContainsKey("Male"), Is.True);
            Assert.That(data.ContainsKey("Female"), Is.True);
            Assert.That(data["Male"].First().AverageSalary, Is.EqualTo(80000));
            Assert.That(data["Female"].First().AverageSalary, Is.EqualTo(75000));
        }

        [Test]
        public async Task GetSalaryDifferencesByGender_ReturnsNotFoundWhenNoSalaryDataExists()
        {
            // Arrange
            var jobTitle = "Nonexistent Job Title";
            var salaryDifferences = new Dictionary<string, List<SalaryDifferenceDTO>>
            {
                { "Male", new List<SalaryDifferenceDTO>() },
                { "Female", new List<SalaryDifferenceDTO>() }
            };

            _employeeServiceMock
                .Setup(s => s.GetSalaryDifferencesByGenderAsync(jobTitle))
                .ReturnsAsync(salaryDifferences);

            // Act
            var result = await _controller.GetSalaryDifferencesByGender(jobTitle) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response["message"], Is.EqualTo($"No salary data found for job title: {jobTitle}"));
        }

        [Test]
        public async Task GetSalaryDifferencesByGender_ReturnsInternalServerErrorWhenExceptionIsThrown()
        {
            // Arrange
            var jobTitle = "Software Engineer";

            _employeeServiceMock
                .Setup(s => s.GetSalaryDifferencesByGenderAsync(jobTitle))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetSalaryDifferencesByGender(jobTitle) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("error"), Is.True);
            Assert.That(response["error"], Is.EqualTo("An error occurred while retrieving salary differences"));
            Assert.That(response.ContainsKey("details"), Is.True);
            Assert.That(response["details"], Is.EqualTo("Unexpected error"));
        }

        [Test]
        public async Task GetAllSalaryDifferencesByGender_ReturnsOkResultWithSalaryDifferences()
        {
            // Arrange
            var salaryDifferences = new Dictionary<string, List<SalaryDifferenceDTO>>
            {
                { "Male", new List<SalaryDifferenceDTO> { new SalaryDifferenceDTO { AverageSalary = 80000 } } },
                { "Female", new List<SalaryDifferenceDTO> { new SalaryDifferenceDTO { AverageSalary = 75000 } } }
            };

            _employeeServiceMock
                .Setup(s => s.GetAllSalaryDifferencesByGenderAsync())
                .ReturnsAsync(salaryDifferences);

            // Act
            var result = await _controller.GetAllSalaryDifferencesByGender() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("data"), Is.True);

            var data = response["data"] as Dictionary<string, List<SalaryDifferenceDTO>>;
            Assert.That(data, Is.Not.Null);
            Assert.That(data.ContainsKey("Male"), Is.True);
            Assert.That(data.ContainsKey("Female"), Is.True);
            Assert.That(data["Male"].First().AverageSalary, Is.EqualTo(80000));
            Assert.That(data["Female"].First().AverageSalary, Is.EqualTo(75000));
        }

        [Test]
        public async Task GetAllSalaryDifferencesByGender_ReturnsNotFoundWhenNoSalaryDataExists()
        {
            // Arrange
            var salaryDifferences = new Dictionary<string, List<SalaryDifferenceDTO>>
            {
                { "Male", new List<SalaryDifferenceDTO>() },
                { "Female", new List<SalaryDifferenceDTO>() }
            };

            _employeeServiceMock
                .Setup(s => s.GetAllSalaryDifferencesByGenderAsync())
                .ReturnsAsync(salaryDifferences);

            // Act
            var result = await _controller.GetAllSalaryDifferencesByGender() as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("message"), Is.True);
            Assert.That(response["message"], Is.EqualTo("No salary data found"));
        }

        [Test]
        public async Task GetAllSalaryDifferencesByGender_ReturnsInternalServerErrorWhenExceptionIsThrown()
        {
            // Arrange
            _employeeServiceMock
                .Setup(s => s.GetAllSalaryDifferencesByGenderAsync())
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetAllSalaryDifferencesByGender() as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("error"), Is.True);
            Assert.That(response["error"], Is.EqualTo("An error occurred while retrieving salary differences"));
            Assert.That(response.ContainsKey("details"), Is.True);
            Assert.That(response["details"], Is.EqualTo("Unexpected error"));
        }
    }
}