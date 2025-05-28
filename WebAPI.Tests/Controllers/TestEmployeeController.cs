using Backend.Controllers;
using Backend.Models;
using Backend.Models.DTO;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
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
                JobTitle = "Developer",
                Experience = 5,
                Gender = "Male",
                CompanyID = 2
            };
            _employeeServiceMock.Setup(s => s.GetEmployeeByIdAsync(1)).ReturnsAsync(employee);

            // Act
            var result = await _controller.GetEmployeeById(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var employeeDto = result.Value as EmployeeDto;
            Assert.That(employeeDto, Is.Not.Null);
            Assert.That(employeeDto.EmployeeID, Is.EqualTo(1));
            Assert.That(employeeDto.JobTitle, Is.EqualTo("Developer"));
        }

        [Test]
        public async Task GetEmployeeById_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetEmployeeByIdAsync(99)).ReturnsAsync((EmployeeModel?)null!);

            // Act
            var result = await _controller.GetEmployeeById(99) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("Employee with ID 99 not found"));
        }

        [Test]
        public async Task GetEmployeeById_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetEmployeeByIdAsync(1)).ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.GetEmployeeById(1) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("An error occurred while retrieving the employee"));
        }

        [Test]
        public async Task GetEmployeeById_ReturnsNotFound_WhenKeyNotFoundExceptionIsThrown()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetEmployeeByIdAsync(42))
                .ThrowsAsync(new KeyNotFoundException("Employee not found"));

            // Act
            var result = await _controller.GetEmployeeById(42) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("Employee not found"));
        }

        [Test]
        public async Task GetEmployeesByCompanyId_ReturnsOkResultWithEmployees()
        {
            // Arrange
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel
                {
                    EmployeeID = 1,
                    JobTitle = "Developer",
                    Experience = 5,
                    Gender = "Male",
                    CompanyID = 2
                },
                new EmployeeModel
                {
                    EmployeeID = 2,
                    JobTitle = "Tester",
                    Experience = 3,
                    Gender = "Female",
                    CompanyID = 2
                }
            };
            _employeeServiceMock.Setup(s => s.GetEmployeesByCompanyIdAsync(2)).ReturnsAsync(employees);

            // Act
            var result = await _controller.GetEmployeesByCompanyId(2) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var employeeDtos = result.Value as List<EmployeeDto>;
            Assert.That(employeeDtos, Is.Not.Null);
            Assert.That(employeeDtos.Count, Is.EqualTo(2));
            Assert.That(employeeDtos[0].JobTitle, Is.EqualTo("Developer"));
            Assert.That(employeeDtos[1].JobTitle, Is.EqualTo("Tester"));
        }

        [Test]
        public async Task GetEmployeesByCompanyId_ReturnsNotFound_WhenNoEmployeesExist()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetEmployeesByCompanyIdAsync(99)).ReturnsAsync(new List<EmployeeModel>());

            // Act
            var result = await _controller.GetEmployeesByCompanyId(99) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("No employees found for CompanyID 99"));
        }

        [Test]
        public async Task GetEmployeesByCompanyId_ReturnsNotFound_WhenEmployeesIsNull()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetEmployeesByCompanyIdAsync(123))!.ReturnsAsync((List<EmployeeModel>?)null);

            // Act
            var result = await _controller.GetEmployeesByCompanyId(123) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("No employees found for CompanyID 123"));
        }

        [Test]
        public async Task GetEmployeesByCompanyId_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetEmployeesByCompanyIdAsync(2)).ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.GetEmployeesByCompanyId(2) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("An error occurred while retrieving employees"));
        }

        [Test]
        public async Task BulkUploadEmployees_ReturnsOkResult_WhenEmployeesAreUploaded()
        {
            // Arrange
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel { EmployeeID = 1, JobTitle = "Developer", Experience = 5, Gender = "Male", CompanyID = 2 },
                new EmployeeModel { EmployeeID = 2, JobTitle = "Tester", Experience = 3, Gender = "Female", CompanyID = 2 }
            };
            _employeeServiceMock.Setup(s => s.GetMaxEmployeeIdAsync()).ReturnsAsync(0);
            _employeeServiceMock.Setup(s => s.BulkCreateEmployeesAsync(It.IsAny<List<EmployeeModel>>()))
                .ReturnsAsync(employees);

            // Act
            var result = await _controller.BulkUploadEmployees(employees) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            var processedCount = result.Value.GetType().GetProperty("processedCount")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Successfully processed 2 employees"));
            Assert.That(processedCount, Is.EqualTo(2));
        }

        [Test]
        public async Task BulkUploadEmployees_ReturnsBadRequest_WhenNoEmployeesProvided()
        {
            // Arrange
            List<EmployeeModel> employees = null!;

            // Act
            var result = await _controller.BulkUploadEmployees(employees) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("No employees provided"));
        }

        [Test]
        public async Task BulkUploadEmployees_ReturnsBadRequest_WhenEmptyListProvided()
        {
            // Arrange
            var employees = new List<EmployeeModel>();

            // Act
            var result = await _controller.BulkUploadEmployees(employees) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("No employees provided"));
        }

        [Test]
        public async Task BulkUploadEmployees_ReturnsBadRequest_WhenInvalidEmployeeData()
        {
            // Arrange
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel { EmployeeID = 1, JobTitle = "", Experience = 5, Gender = "Male", CompanyID = 2 }
            };
            _employeeServiceMock.Setup(s => s.GetMaxEmployeeIdAsync()).ReturnsAsync(0);

            // Act
            var result = await _controller.BulkUploadEmployees(employees) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Invalid employee data"));
        }

        [Test]
        public async Task BulkUploadEmployees_ReturnsBadRequest_WhenExperienceIsNegative()
        {
            // Arrange
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel { EmployeeID = 1, JobTitle = "Developer", Experience = -1, Gender = "Male", CompanyID = 2 }
            };
            _employeeServiceMock.Setup(s => s.GetMaxEmployeeIdAsync()).ReturnsAsync(0);

            // Act
            var result = await _controller.BulkUploadEmployees(employees) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Invalid employee data: Experience must be >= 0"));
        }

        [Test]
        public async Task BulkUploadEmployees_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel { EmployeeID = 1, JobTitle = "Developer", Experience = 5, Gender = "Male", CompanyID = 2 }
            };
            _employeeServiceMock.Setup(s => s.GetMaxEmployeeIdAsync()).ReturnsAsync(0);
            _employeeServiceMock.Setup(s => s.BulkCreateEmployeesAsync(It.IsAny<List<EmployeeModel>>()))
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.BulkUploadEmployees(employees) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("An error occurred while processing the bulk upload"));
        }

        [Test]
        public async Task UpdateEmployee_ReturnsOkResult_WhenUpdateIsSuccessful()
        {
            // Arrange
            var updatedEmployee = new EmployeeModel
            {
                EmployeeID = 1,
                JobTitle = "Developer",
                Experience = 6,
                Gender = "Male",
                CompanyID = 2
            };
            _employeeServiceMock.Setup(s => s.UpdateEmployeeAsync(1, updatedEmployee)).ReturnsAsync(updatedEmployee);

            // Act
            var result = await _controller.UpdateEmployee(1, updatedEmployee) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Employee updated successfully"));
            var employee = result.Value.GetType().GetProperty("employee")?.GetValue(result.Value, null) as EmployeeModel;
            Assert.That(employee, Is.Not.Null);
            Assert.That(employee!.EmployeeID, Is.EqualTo(1));
            Assert.That(employee.JobTitle, Is.EqualTo("Developer"));
        }

        [Test]
        public async Task UpdateEmployee_ReturnsBadRequest_WhenEmployeeIsNull()
        {
            // Act
            var result = await _controller.UpdateEmployee(1, null!) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Invalid employee data or ID"));
        }

        [Test]
        public async Task UpdateEmployee_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            var updatedEmployee = new EmployeeModel
            {
                EmployeeID = 1,
                JobTitle = "Developer",
                Experience = 6,
                Gender = "Male",
                CompanyID = 2
            };

            // Act
            var result = await _controller.UpdateEmployee(0, updatedEmployee) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Invalid employee data or ID"));
        }

        [Test]
        public async Task UpdateEmployee_ReturnsBadRequest_WhenRequiredFieldsMissing()
        {
            // Arrange
            var updatedEmployee = new EmployeeModel
            {
                EmployeeID = 1,
                JobTitle = "",
                Experience = 6,
                Gender = "Male",
                CompanyID = 2
            };

            // Act
            var result = await _controller.UpdateEmployee(1, updatedEmployee) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Invalid employee data: Missing required fields or invalid CompanyID"));
        }

        [Test]
        public async Task UpdateEmployee_ReturnsBadRequest_WhenExperienceIsNegative()
        {
            // Arrange
            var updatedEmployee = new EmployeeModel
            {
                EmployeeID = 1,
                JobTitle = "Developer",
                Experience = -1,
                Gender = "Male",
                CompanyID = 2
            };

            // Act
            var result = await _controller.UpdateEmployee(1, updatedEmployee) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Invalid employee data: Experience must be >= 0"));
        }

        [Test]
        public async Task UpdateEmployee_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var updatedEmployee = new EmployeeModel
            {
                EmployeeID = 1,
                JobTitle = "Developer",
                Experience = 6,
                Gender = "Male",
                CompanyID = 2
            };
            _employeeServiceMock.Setup(s => s.UpdateEmployeeAsync(1, updatedEmployee))
                .ThrowsAsync(new KeyNotFoundException("Employee not found"));

            // Act
            var result = await _controller.UpdateEmployee(1, updatedEmployee) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("Employee not found"));
        }

        [Test]
        public async Task UpdateEmployee_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var updatedEmployee = new EmployeeModel
            {
                EmployeeID = 1,
                JobTitle = "Developer",
                Experience = 6,
                Gender = "Male",
                CompanyID = 2
            };
            _employeeServiceMock.Setup(s => s.UpdateEmployeeAsync(1, updatedEmployee))
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.UpdateEmployee(1, updatedEmployee) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("An error occurred while updating the employee"));
        }

        [Test]
        public async Task GetAllEmployees_ReturnsOkResultWithEmployees()
        {
            // Arrange
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel
                {
                    EmployeeID = 1,
                    JobTitle = "Developer",
                    Experience = 5,
                    Gender = "Male",
                    CompanyID = 2
                },
                new EmployeeModel
                {
                    EmployeeID = 2,
                    JobTitle = "Tester",
                    Experience = 3,
                    Gender = "Female",
                    CompanyID = 2
                }
            };
            _employeeServiceMock.Setup(s => s.GetAllEmployeesAsync()).ReturnsAsync(employees);

            // Act
            var result = await _controller.GetAllEmployees() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var employeeDtos = result.Value as List<EmployeeDto>;
            Assert.That(employeeDtos, Is.Not.Null);
            Assert.That(employeeDtos.Count, Is.EqualTo(2));
            Assert.That(employeeDtos[0].JobTitle, Is.EqualTo("Developer"));
            Assert.That(employeeDtos[1].JobTitle, Is.EqualTo("Tester"));
        }

        [Test]
        public async Task GetAllEmployees_ReturnsNotFound_WhenNoEmployeesExist()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetAllEmployeesAsync()).ReturnsAsync(new List<EmployeeModel>());

            // Act
            var result = await _controller.GetAllEmployees() as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var status = result.Value.GetType().GetProperty("Status")?.GetValue(result.Value, null);
            var message = result.Value.GetType().GetProperty("Message")?.GetValue(result.Value, null);
            Assert.That(status, Is.EqualTo("NotFound"));
            Assert.That(message, Is.EqualTo("No employees found in the database."));
        }

        [Test]
        public async Task GetAllEmployees_ReturnsNotFound_WhenEmployeesIsNull()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetAllEmployeesAsync())!.ReturnsAsync((List<EmployeeModel>?)null);

            // Act
            var result = await _controller.GetAllEmployees() as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var status = result.Value.GetType().GetProperty("Status")?.GetValue(result.Value, null);
            var message = result.Value.GetType().GetProperty("Message")?.GetValue(result.Value, null);
            Assert.That(status, Is.EqualTo("NotFound"));
            Assert.That(message, Is.EqualTo("No employees found in the database."));
        }

        [Test]
        public async Task GetAllEmployees_ReturnsBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetAllEmployeesAsync()).ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.GetAllEmployees() as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);
            var status = result.Value.GetType().GetProperty("Status")?.GetValue(result.Value, null);
            var message = result.Value.GetType().GetProperty("Message")?.GetValue(result.Value, null);
            Assert.That(status, Is.EqualTo("Error"));
            Assert.That(message, Does.StartWith("Failed to retrieve data:"));
        }

        [Test]
        public async Task GetAllJobTitles_ReturnsOkResultWithJobTitles()
        {
            // Arrange
            var jobTitles = new List<string> { "Developer", "Tester", "Manager" };
            _employeeServiceMock.Setup(s => s.GetAllJobTitlesAsync()).ReturnsAsync(jobTitles);

            // Act
            var result = await _controller.GetAllJobTitles() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var returnedJobTitles = result.Value.GetType().GetProperty("jobTitles")?.GetValue(result.Value, null) as List<string>;
            Assert.That(returnedJobTitles, Is.Not.Null);
            Assert.That(returnedJobTitles.Count, Is.EqualTo(3));
            Assert.That(returnedJobTitles, Is.EquivalentTo(jobTitles));
        }

        [Test]
        public async Task GetAllJobTitles_ReturnsNotFound_WhenNoJobTitlesExist()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetAllJobTitlesAsync()).ReturnsAsync(new List<string>());

            // Act
            var result = await _controller.GetAllJobTitles() as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("No job titles found"));
        }

        [Test]
        public async Task GetAllJobTitles_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetAllJobTitlesAsync()).ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.GetAllJobTitles() as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("An error occurred while retrieving job titles"));
        }

        [Test]
        public async Task GetEmployeesByJobTitle_ReturnsOkResultWithEmployees()
        {
            // Arrange
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel
                {
                    EmployeeID = 1,
                    JobTitle = "Developer",
                    Experience = 5,
                    Gender = "Male",
                    CompanyID = 2
                },
                new EmployeeModel
                {
                    EmployeeID = 2,
                    JobTitle = "Developer",
                    Experience = 3,
                    Gender = "Female",
                    CompanyID = 2
                }
            };
            _employeeServiceMock.Setup(s => s.GetEmployeesByJobTitleAsync("Developer")).ReturnsAsync(employees);

            // Act
            var result = await _controller.GetEmployeesByJobTitle("Developer") as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var returnedEmployees = result.Value as List<EmployeeModel>;
            Assert.That(returnedEmployees, Is.Not.Null);
            Assert.That(returnedEmployees.Count, Is.EqualTo(2));
            Assert.That(returnedEmployees[0].JobTitle, Is.EqualTo("Developer"));
            Assert.That(returnedEmployees[1].JobTitle, Is.EqualTo("Developer"));
        }

        [Test]
        public async Task GetEmployeesByJobTitle_ReturnsNotFound_WhenNoEmployeesExist()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetEmployeesByJobTitleAsync("Unknown")).ReturnsAsync(new List<EmployeeModel>());

            // Act
            var result = await _controller.GetEmployeesByJobTitle("Unknown") as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("No employees found with job title: Unknown"));
        }

        [Test]
        public async Task GetEmployeesByJobTitle_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetEmployeesByJobTitleAsync("Developer"))
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.GetEmployeesByJobTitle("Developer") as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("An error occurred while retrieving employees"));
        }
        [Test]
        public async Task GetSalaryDifferencesByGender_ReturnsOkResultWithDifferences()
        {
            // Arrange
            var salaryDifferences = new Dictionary<string, List<SalaryDifferenceDTO>>
            {
                { "Male", new List<SalaryDifferenceDTO> { new SalaryDifferenceDTO { Month = new DateTime(2024, 1, 1), AverageSalary = 50000 } } },
                { "Female", new List<SalaryDifferenceDTO> { new SalaryDifferenceDTO { Month = new DateTime(2024, 1, 1), AverageSalary = 48000 } } }
            };
            _employeeServiceMock.Setup(s => s.GetSalaryDifferencesByGenderAsync("Developer")).ReturnsAsync(salaryDifferences);

            // Act
            var result = await _controller.GetSalaryDifferencesByGender("Developer") as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var returned = result.Value as Dictionary<string, List<SalaryDifferenceDTO>>;
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned.ContainsKey("Male"));
            Assert.That(returned.ContainsKey("Female"));
            Assert.That(returned["Male"].Count, Is.EqualTo(1));
            Assert.That(returned["Female"].Count, Is.EqualTo(1));
            Assert.That(returned["Male"][0].AverageSalary, Is.EqualTo(50000));
            Assert.That(returned["Female"][0].AverageSalary, Is.EqualTo(48000));
        }

        [Test]
        public async Task GetSalaryDifferencesByGender_ReturnsNotFound_WhenNoSalaryData()
        {
            // Arrange
            var salaryDifferences = new Dictionary<string, List<SalaryDifferenceDTO>>
            {
                { "Male", new List<SalaryDifferenceDTO>() },
                { "Female", new List<SalaryDifferenceDTO>() }
            };
            _employeeServiceMock.Setup(s => s.GetSalaryDifferencesByGenderAsync("Unknown")).ReturnsAsync(salaryDifferences);

            // Act
            var result = await _controller.GetSalaryDifferencesByGender("Unknown") as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("No salary data found for job title: Unknown"));
        }

        [Test]
        public async Task GetSalaryDifferencesByGender_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetSalaryDifferencesByGenderAsync("Developer"))
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.GetSalaryDifferencesByGender("Developer") as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("An error occurred while retrieving salary differences"));
        }

        [Test]
        public async Task GetAllSalaryDifferencesByGender_ReturnsOkResultWithDifferences()
        {
            // Arrange
            var salaryDifferences = new Dictionary<string, List<SalaryDifferenceDTO>>
            {
                { "Male", new List<SalaryDifferenceDTO> { new SalaryDifferenceDTO { Month = new DateTime(2024, 1, 1), AverageSalary = 50000 } } },
                { "Female", new List<SalaryDifferenceDTO> { new SalaryDifferenceDTO { Month = new DateTime(2024, 1, 1), AverageSalary = 48000 } } }
            };
            _employeeServiceMock.Setup(s => s.GetAllSalaryDifferencesByGenderAsync()).ReturnsAsync(salaryDifferences);

            // Act
            var result = await _controller.GetAllSalaryDifferencesByGender() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var returned = result.Value as Dictionary<string, List<SalaryDifferenceDTO>>;
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned.ContainsKey("Male"));
            Assert.That(returned.ContainsKey("Female"));
            Assert.That(returned["Male"].Count, Is.EqualTo(1));
            Assert.That(returned["Female"].Count, Is.EqualTo(1));
            Assert.That(returned["Male"][0].AverageSalary, Is.EqualTo(50000));
            Assert.That(returned["Female"][0].AverageSalary, Is.EqualTo(48000));
        }

        [Test]
        public async Task GetAllSalaryDifferencesByGender_ReturnsNotFound_WhenNoSalaryData()
        {
            // Arrange
            var salaryDifferences = new Dictionary<string, List<SalaryDifferenceDTO>>
            {
                { "Male", new List<SalaryDifferenceDTO>() },
                { "Female", new List<SalaryDifferenceDTO>() }
            };
            _employeeServiceMock.Setup(s => s.GetAllSalaryDifferencesByGenderAsync()).ReturnsAsync(salaryDifferences);

            // Act
            var result = await _controller.GetAllSalaryDifferencesByGender() as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("No salary data found"));
        }

        [Test]
        public async Task GetAllSalaryDifferencesByGender_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetAllSalaryDifferencesByGenderAsync())
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.GetAllSalaryDifferencesByGender() as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("An error occurred while retrieving salary differences"));
        }

        [Test]
        public async Task BulkUploadEmployees_AssignsEmployeeId_WhenEmployeeIdIsZero()
        {
            // Arrange
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel { EmployeeID = 0, JobTitle = "Developer", Experience = 5, Gender = "Male", CompanyID = 2 }
            };
            _employeeServiceMock.Setup(s => s.GetMaxEmployeeIdAsync()).ReturnsAsync(100);
            _employeeServiceMock.Setup(s => s.BulkCreateEmployeesAsync(It.IsAny<List<EmployeeModel>>()))
                .ReturnsAsync((List<EmployeeModel> emps) => emps);

            // Act
            var result = await _controller.BulkUploadEmployees(employees) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            var processedCount = result.Value.GetType().GetProperty("processedCount")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Successfully processed 1 employees"));
            Assert.That(processedCount, Is.EqualTo(1));
            Assert.That(employees[0].EmployeeID, Is.EqualTo(101)); // Should be assigned maxEmployeeId + 1
        }

        [Test]
        public async Task DeleteEmployee_ReturnsOkResult_WhenEmployeeIsDeleted()
        {
            // Arrange
            var employee = new EmployeeModel
            {
                EmployeeID = 1,
                JobTitle = "Developer",
                Experience = 5,
                Gender = "Male",
                CompanyID = 2
            };
            _employeeServiceMock.Setup(s => s.GetEmployeeByIdAsync(1)).ReturnsAsync(employee);
            _employeeServiceMock.Setup(s => s.DeleteEmployeeAsync(1)).Returns(Task.FromResult(employee));

            // Act
            var result = await _controller.DeleteEmployee(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Employee deleted successfully"));
        }

        [Test]
        public async Task DeleteEmployee_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Act
            var result = await _controller.DeleteEmployee(0) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Invalid employee ID"));
        }

        [Test]
        public async Task DeleteEmployee_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetEmployeeByIdAsync(99)).ReturnsAsync((EmployeeModel?)null!);

            // Act
            var result = await _controller.DeleteEmployee(99) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var error = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(error, Is.EqualTo("Employee with ID 99 not found"));
        }

        [Test]
        public async Task DeleteEmployee_ReturnsNotFound_WhenKeyNotFoundExceptionIsThrown()
        {
            // Arrange
            var employee = new EmployeeModel
            {
                EmployeeID = 2,
                JobTitle = "Tester",
                Experience = 3,
                Gender = "Female",
                CompanyID = 2
            };
            _employeeServiceMock.Setup(s => s.GetEmployeeByIdAsync(2)).ReturnsAsync(employee);
            _employeeServiceMock.Setup(s => s.DeleteEmployeeAsync(2))
                .ThrowsAsync(new KeyNotFoundException("Employee not found"));

            // Act
            var result = await _controller.DeleteEmployee(2) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var error = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(error, Is.EqualTo("Employee not found"));
        }

        [Test]
        public async Task DeleteEmployee_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var employee = new EmployeeModel
            {
                EmployeeID = 3,
                JobTitle = "Manager",
                Experience = 10,
                Gender = "Male",
                CompanyID = 2
            };
            _employeeServiceMock.Setup(s => s.GetEmployeeByIdAsync(3)).ReturnsAsync(employee);
            _employeeServiceMock.Setup(s => s.DeleteEmployeeAsync(3))
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.DeleteEmployee(3) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            object? error = null;
            if (result.Value != null)
            {
                error = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            }
            Assert.That(error, Is.EqualTo("An error occurred while deleting the employee"));
        }

        [Test]
        public async Task GetEmployeeIndustryById_ReturnsOkResult_WithIndustryDto()
        {
            // Arrange
            var industryDto = new EmployeeDto
            {
                EmployeeID = 1,
                Industry = "Tech"
            };
            _employeeServiceMock.Setup(s => s.GetEmployeeIndustryByIdAsync(1)).ReturnsAsync(industryDto);

            // Act
            var result = await _controller.GetEmployeeIndustryById(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var returnedDto = result.Value as EmployeeDto;
            Assert.That(returnedDto, Is.Not.Null);
            Assert.That(returnedDto.EmployeeID, Is.EqualTo(1));
            Assert.That(returnedDto.Industry, Is.EqualTo("Tech"));
        }

        [Test]
        public async Task GetEmployeeIndustryById_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Act
            var result = await _controller.GetEmployeeIndustryById(0) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            var error = result.Value?.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(error, Is.EqualTo("Invalid employee ID"));
        }

        [Test]
        public async Task GetEmployeeIndustryById_ReturnsNotFound_WhenIndustryDtoIsNull()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetEmployeeIndustryByIdAsync(99)).ReturnsAsync((EmployeeDto?)null);

            // Act
            var result = await _controller.GetEmployeeIndustryById(99) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var error = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(error, Is.EqualTo("Employee with ID 99 not found or company information is missing"));
        }

        [Test]
        public async Task GetEmployeeIndustryById_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetEmployeeIndustryByIdAsync(1))
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.GetEmployeeIndustryById(1) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            var error = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(error, Is.EqualTo("An error occurred while retrieving the employee's industry"));
        }
    }
}