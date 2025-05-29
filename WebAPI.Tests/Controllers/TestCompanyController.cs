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
using Backend.Data;

namespace WebAPI.Tests.Controllers
{
    [TestFixture]
    public class TestCompanyController
    {
        private Mock<ICompanyService> _companyServiceMock;
        private CompanyController _controller;

        [SetUp]
        public void SetUp()
        {
            _companyServiceMock = new Mock<ICompanyService>();
            _controller = new CompanyController(_companyServiceMock.Object, null);
        }

        [Test]
        public async Task GetAllCompanies_ReturnsOkResultWithCompanies()
        {
            // Arrange
            var companyDTOs = new List<CompanyDTO>
            {
                new CompanyDTO
                {
                    CompanyID = 1,
                    CompanyName = "Company A",
                    Industry = "Tech",
                    CVR = "12345678",
                    Email = "a@example.com"
                },
                new CompanyDTO
                {
                    CompanyID = 2,
                    CompanyName = "Company B",
                    Industry = "Finance",
                    CVR = "87654321",
                    Email = "b@example.com"
                }
            };
            _companyServiceMock.Setup(s => s.GetAllCompaniesAsync()).ReturnsAsync(It.IsAny<List<CompanyModel>>());

            // Act
            var result = await _controller.GetAllCompanies() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var returnedCompanyDTOs = result.Value as List<CompanyDTO>;
            Assert.That(returnedCompanyDTOs, Is.Not.Null);
            Assert.That(returnedCompanyDTOs.Count, Is.EqualTo(2));
            Assert.That(returnedCompanyDTOs[0].CompanyName, Is.EqualTo("Company A"));
            Assert.That(returnedCompanyDTOs[1].CompanyName, Is.EqualTo("Company B"));
        }

        [Test]
        public async Task GetAllCompanies_ReturnsOkResultWithEmptyList_WhenNoCompaniesExist()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetAllCompaniesAsync()).ReturnsAsync(new List<CompanyModel>());

            // Act
            var result = await _controller.GetAllCompanies() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var companyDTOs = result.Value as List<CompanyDTO>;
            Assert.That(companyDTOs, Is.Not.Null);
            Assert.That(companyDTOs.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllCompanies_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetAllCompaniesAsync()).ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.GetAllCompanies() as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("An error occurred while retrieving companies"));
        }

        [Test]
        public async Task GetCompanyById_ReturnsOkResultWithCompany()
        {
            // Arrange
            var companyDTO = new CompanyDTO
            {
                CompanyID = 1,
                CompanyName = "Company A",
                Industry = "Tech",
                CVR = "12345678",
                Email = "a@example.com"
            };
            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(1)).ReturnsAsync(It.IsAny<CompanyModel>());

            // Act
            var result = await _controller.GetCompanyById(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var returnedCompanyDTO = result.Value as CompanyDTO;
            Assert.That(returnedCompanyDTO, Is.Not.Null);
            Assert.That(returnedCompanyDTO.CompanyID, Is.EqualTo(1));
            Assert.That(returnedCompanyDTO.CompanyName, Is.EqualTo("Company A"));
        }

        [Test]
        public async Task GetCompanyById_ReturnsNotFound_WhenCompanyDoesNotExist()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(99)).ThrowsAsync(new KeyNotFoundException("Company not found"));

            // Act
            var result = await _controller.GetCompanyById(99) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("Company not found"));
        }

        [Test]
        public async Task GetCompanyById_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(1)).ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.GetCompanyById(1) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("An error occurred while retrieving the company"));
        }

        [Test]
        public async Task InsertCompanies_ReturnsOkResult_WhenCompaniesAreInserted()
        {
            // Arrange
            var companyDTOs = new List<CompanyDTO>
            {
                new CompanyDTO { CompanyName = "Company A", Industry = "Tech", CVR = "12345678", Email = "a@example.com" },
                new CompanyDTO { CompanyName = "Company B", Industry = "Finance", CVR = "87654321", Email = "b@example.com" }
            };

            _companyServiceMock.Setup(s => s.CreateCompaniesAsync(It.IsAny<List<CompanyModel>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.InsertCompanies(companyDTOs) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            var count = result.Value.GetType().GetProperty("count")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Companies inserted successfully"));
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public async Task InsertCompanies_ReturnsBadRequest_WhenNoCompaniesProvided()
        {
            // Arrange
            List<CompanyDTO>? companyDTOs = null;

            // Act
            var result = await _controller.InsertCompanies(companyDTOs!) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("No companies provided"));
        }

        [Test]
        public async Task InsertCompanies_ReturnsBadRequest_WhenEmptyListProvided()
        {
            // Arrange
            var companyDTOs = new List<CompanyDTO>();

            // Act
            var result = await _controller.InsertCompanies(companyDTOs) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("No companies provided"));
        }

        [Test]
        public async Task InsertCompanies_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var companyDTOs = new List<CompanyDTO>
            {
                new CompanyDTO { CompanyName = "Company A", Industry = "Tech", CVR = "12345678", Email = "a@example.com" }
            };

            _companyServiceMock.Setup(s => s.CreateCompaniesAsync(It.IsAny<List<CompanyModel>>()))
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.InsertCompanies(companyDTOs) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("An error occurred while inserting companies"));
        }

        [Test]
        public async Task UpdateCompany_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var companyDTO = new CompanyDTO
            {
                CompanyID = 1,
                CompanyName = "Company A",
                Industry = "Tech",
                CVR = "12345678",
                Email = "a@example.com"
            };

            _companyServiceMock.Setup(s => s.UpdateCompanyAsync(It.IsAny<CompanyModel>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateCompany(1, companyDTO) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public async Task UpdateCompany_ReturnsBadRequest_WhenCompanyIsNull()
        {
            // Act
            var result = await _controller.UpdateCompany(1, null!) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Invalid company data"));
        }

        [Test]
        public async Task UpdateCompany_ReturnsBadRequest_WhenIdDoesNotMatchCompanyId()
        {
            // Arrange
            var companyDTO = new CompanyDTO
            {
                CompanyID = 2, // Different from id parameter
                CompanyName = "Company A",
                Industry = "Tech",
                CVR = "12345678",
                Email = "a@example.com"
            };

            // Act
            var result = await _controller.UpdateCompany(1, companyDTO) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Invalid company data"));
        }

        [Test]
        public async Task UpdateCompany_ReturnsNotFound_WhenCompanyDoesNotExist()
        {
            // Arrange
            var companyDTO = new CompanyDTO
            {
                CompanyID = 1,
                CompanyName = "Company A",
                Industry = "Tech",
                CVR = "12345678",
                Email = "a@example.com"
            };
            var companyModel = new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Company A",
                Industry = "Tech",
                CVR = "12345678",
                Email = "a@example.com"
            };
            _companyServiceMock.Setup(s => s.UpdateCompanyAsync(It.IsAny<CompanyModel>()))
                .ThrowsAsync(new KeyNotFoundException("Company not found"));

            // Act
            var result = await _controller.UpdateCompany(1, companyDTO) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("Company not found"));
        }

        [Test]
        public async Task UpdateCompany_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var companyDTO = new CompanyDTO
            {
                CompanyID = 1,
                CompanyName = "Company A",
                Industry = "Tech",
                CVR = "12345678",
                Email = "a@example.com"
            };
            var companyModel = new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Company A",
                Industry = "Tech",
                CVR = "12345678",
                Email = "a@example.com"
            };
            _companyServiceMock.Setup(s => s.UpdateCompanyAsync(It.IsAny<CompanyModel>()))
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.UpdateCompany(1, companyDTO) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("An error occurred while updating the company"));
        }

        [Test]
        public async Task DeleteCompany_ReturnsNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            var companyDTO = new CompanyDTO
            {
                CompanyID = 1,
                CompanyName = "Company A",
                Industry = "Tech",
                CVR = "12345678",
                Email = "a@example.com"
            };

            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(1)).ReturnsAsync(It.IsAny<CompanyModel>());
            _companyServiceMock.Setup(s => s.DeleteCompanyAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteCompany(1) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public async Task DeleteCompany_ReturnsNotFound_WhenCompanyDoesNotExist()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(99)).ThrowsAsync(new KeyNotFoundException("Company not found"));

            // Act
            var result = await _controller.DeleteCompany(99) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("Company not found"));
        }

        [Test]
        public async Task DeleteCompany_ReturnsNotFound_WhenExistingCompanyIsNull()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(123)).ReturnsAsync((CompanyModel?)null!);

            // Act
            var result = await _controller.DeleteCompany(123) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("Company not found"));
        }

        [Test]
        public async Task DeleteCompany_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(1)).ReturnsAsync(new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Test",
                Industry = "Test",
                CVR = "00000000",
                Email = "test@example.com"
            });
            _companyServiceMock.Setup(s => s.DeleteCompanyAsync(1)).ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.DeleteCompany(1) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("An error occurred while deleting the company"));
        }

        [Test]
        public async Task GetAllIndustries_ReturnsOkResultWithIndustries()
        {
            // Arrange
            var industries = new List<string> { "Tech", "Finance", "Healthcare" };
            _companyServiceMock.Setup(s => s.GetAllIndustriesAsync()).ReturnsAsync(industries);

            // Act
            var result = await _controller.GetAllIndustries() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var returnedIndustries = result.Value as List<string>;
            Assert.That(returnedIndustries, Is.Not.Null);
            Assert.That(returnedIndustries.Count, Is.EqualTo(3));
            Assert.That(returnedIndustries, Is.EquivalentTo(industries));
        }

        [Test]
        public async Task GetAllIndustries_ReturnsOkResultWithEmptyList_WhenNoIndustriesExist()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetAllIndustriesAsync()).ReturnsAsync(new List<string>());

            // Act
            var result = await _controller.GetAllIndustries() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var returnedIndustries = result.Value as List<string>;
            Assert.That(returnedIndustries, Is.Not.Null);
            Assert.That(returnedIndustries.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllIndustries_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetAllIndustriesAsync()).ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.GetAllIndustries() as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("An error occurred while retrieving industries"));
        }

        [Test]
        public async Task GetAverageSalariesForJobsInIndustry_ReturnsOkResultWithAverages()
        {
            // Arrange
            var averages = new List<JobTitleSalaryDTO>
            {
                new JobTitleSalaryDTO { JobTitle = "Developer", GenderData = new Dictionary<string, GenderSalaryData>() },
                new JobTitleSalaryDTO { JobTitle = "Manager", GenderData = new Dictionary<string, GenderSalaryData>() }
            };
            _companyServiceMock.Setup(s => s.GetAverageSalariesForJobsInIndustryAsync("Tech")).ReturnsAsync(averages);

            // Act
            var result = await _controller.GetAverageSalariesForJobsInIndustry("Tech") as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var returnedAverages = result.Value as List<JobTitleSalaryDTO>;
            Assert.That(returnedAverages, Is.Not.Null);
            Assert.That(returnedAverages.Count, Is.EqualTo(2));
            Assert.That(returnedAverages[0].JobTitle, Is.EqualTo("Developer"));
            Assert.That(returnedAverages[1].JobTitle, Is.EqualTo("Manager"));
        }

        [Test]
        public async Task GetAverageSalariesForJobsInIndustry_ReturnsNotFound_WhenNoAveragesExist()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetAverageSalariesForJobsInIndustryAsync("Unknown")).ReturnsAsync(new List<JobTitleSalaryDTO>());

            // Act
            var result = await _controller.GetAverageSalariesForJobsInIndustry("Unknown") as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value, null);
            Assert.That(message, Is.EqualTo("No companies found in the Unknown industry"));
        }

        [Test]
        public async Task GetAverageSalariesForJobsInIndustry_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetAverageSalariesForJobsInIndustryAsync("Tech"))
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.GetAverageSalariesForJobsInIndustry("Tech") as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);
            var errorObj = result.Value.GetType().GetProperty("error")?.GetValue(result.Value, null);
            Assert.That(errorObj, Is.EqualTo("An error occurred while retrieving salary averages"));
        }
    }
}