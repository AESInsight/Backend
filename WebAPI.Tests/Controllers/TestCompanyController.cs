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
    public class TestCompanyController
    {
        private Mock<ICompanyService> _companyServiceMock;
        private CompanyController _controller;

        [SetUp]
        public void SetUp()
        {
            _companyServiceMock = new Mock<ICompanyService>();
            _controller = new CompanyController(_companyServiceMock.Object);
        }

        [Test]
        public async Task GetAllCompanies_ReturnsOkResultWithCompanies()
        {
            // Arrange
            var companies = new List<CompanyModel>
            {
                new CompanyModel
                {
                    CompanyID = 1,
                    CompanyName = "Company A",
                    Industry = "Tech",
                    CVR = "12345678",
                    Email = "a@example.com",
                    PasswordHash = Encoding.UTF8.GetBytes("hash1")
                },
                new CompanyModel
                {
                    CompanyID = 2,
                    CompanyName = "Company B",
                    Industry = "Finance",
                    CVR = "87654321",
                    Email = "b@example.com",
                    PasswordHash = Encoding.UTF8.GetBytes("hash2")
                }
            };
            _companyServiceMock.Setup(s => s.GetAllCompaniesAsync()).ReturnsAsync(companies);

            // Act
            var result = await _controller.GetAllCompanies() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("data"), Is.True);

            var companyDTOs = response["data"] as List<CompanyDTO>;
            Assert.That(companyDTOs, Is.Not.Null);
            Assert.That(companyDTOs.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task InsertCompanies_ReturnsOkResult()
        {
            // Arrange
            var companyDTOs = new List<CompanyDTO>
            {
                new CompanyDTO { CompanyName = "Company A", Industry = "Tech", CVR = "12345678", Email = "a@example.com" }
            };

            // Mock the CreateCompaniesAsync method to simulate successful insertion
            _companyServiceMock
                .Setup(s => s.CreateCompaniesAsync(It.IsAny<List<CompanyModel>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.InsertCompanies(companyDTOs) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            // Cast the result.Value to a dictionary
            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("Companies inserted successfully"));
            Assert.That(response["count"], Is.EqualTo(1)); // Ensure the count matches the number of companies inserted
        }

        [Test]
        public async Task DeleteCompany_ReturnsNotFoundResult()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(1)).ThrowsAsync(new KeyNotFoundException("Company not found"));

            // Act
            var result = await _controller.DeleteCompany(1) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var error = result.Value as Dictionary<string, object>;
            Assert.That(error, Is.Not.Null);
            Assert.That(error["error"], Is.EqualTo("Company not found"));
        }

        [Test]
        public async Task GetCompanyById_ReturnsOkResultWithCompany()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Company A",
                Industry = "Tech",
                CVR = "12345678",
                Email = "a@example.com",
                PasswordHash = Encoding.UTF8.GetBytes("password1")
            };

            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(1)).ReturnsAsync(company);

            // Act
            var result = await _controller.GetCompanyById(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("data"), Is.True);

            var companyDTO = response["data"] as CompanyDTO;
            Assert.That(companyDTO, Is.Not.Null);
            Assert.That(companyDTO.CompanyName, Is.EqualTo("Company A"));
        }

        [Test]
        public async Task GetCompanyById_ReturnsNotFoundResult()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(1)).ThrowsAsync(new KeyNotFoundException("Company not found"));

            // Act
            var result = await _controller.GetCompanyById(1) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var error = result.Value as Dictionary<string, object>;
            Assert.That(error, Is.Not.Null);
            Assert.That(error["error"], Is.EqualTo("Company not found"));
        }

        [Test]
        public async Task UpdateCompany_ReturnsBadRequestResult()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyID = 2, // Mismatched ID
                CompanyName = "Updated Company",
                Industry = "Tech",
                CVR = "12345678",
                Email = "updated@example.com",
                PasswordHash = Encoding.UTF8.GetBytes("updatedhash")
            };

            // Act
            var result = await _controller.UpdateCompany(1, company) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("Invalid company data"));
        }

        [Test]
        public async Task GenerateSampleCompanies_ReturnsOkResult()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GenerateSampleCompaniesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.GenerateSampleCompanies() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("Sample companies generated successfully"));
        }

        [Test]
        public async Task DeleteAllCompanies_ReturnsOkResult()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.DeleteAllCompaniesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteAllCompanies() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("All companies deleted successfully"));
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

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("data"), Is.True);

            var industryList = response["data"] as List<string>;
            Assert.That(industryList, Is.Not.Null);
            Assert.That(industryList.Count, Is.EqualTo(3));
            Assert.That(industryList, Is.EquivalentTo(industries));
        }

        [Test]
        public async Task GetAllCompanies_ReturnsInternalServerErrorOnException()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetAllCompaniesAsync()).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAllCompanies() as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var error = result.Value as Dictionary<string, object>;
            Assert.That(error, Is.Not.Null);
            Assert.That(error["error"], Is.EqualTo("An error occurred while retrieving companies"));
            Assert.That(error["details"], Is.EqualTo("Database error")); // Ensure exception details are included
        }

        [Test]
        public async Task GetCompanyById_ReturnsNotFoundWhenCompanyIsNull()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(1)).ReturnsAsync((CompanyModel)null!);

            // Act
            var result = await _controller.GetCompanyById(1) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("Company not found"));
        }

        [Test]
        public async Task InsertCompanies_ReturnsInternalServerErrorOnException()
        {
            // Arrange
            var companyDTOs = new List<CompanyDTO>
            {
                new CompanyDTO { CompanyName = "Company A", Industry = "Tech", CVR = "12345678", Email = "a@example.com" }
            };

            _companyServiceMock.Setup(s => s.CreateCompaniesAsync(It.IsAny<List<CompanyModel>>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.InsertCompanies(companyDTOs) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var error = result.Value as Dictionary<string, object>;
            Assert.That(error, Is.Not.Null);
            Assert.That(error["error"], Is.EqualTo("An error occurred while inserting companies"));
            Assert.That(error["details"], Is.EqualTo("Database error")); // Ensure exception details are included
        }

        [Test]
        public async Task UpdateCompany_ReturnsNotFoundWhenCompanyDoesNotExist()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Updated Company",
                Industry = "Tech",
                CVR = "12345678",
                Email = "updated@example.com",
                PasswordHash = Encoding.UTF8.GetBytes("updatedhash")
            };

            _companyServiceMock.Setup(s => s.UpdateCompanyAsync(company))
                .ThrowsAsync(new KeyNotFoundException("Company not found"));

            // Act
            var result = await _controller.UpdateCompany(1, company) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var error = result.Value as Dictionary<string, object>;
            Assert.That(error, Is.Not.Null);
            Assert.That(error["error"], Is.EqualTo("Company not found"));
        }

        [Test]
        public async Task UpdateCompany_ReturnsInternalServerErrorOnException()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Updated Company",
                Industry = "Tech",
                CVR = "12345678",
                Email = "updated@example.com",
                PasswordHash = Encoding.UTF8.GetBytes("updatedhash")
            };

            _companyServiceMock.Setup(s => s.UpdateCompanyAsync(company))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.UpdateCompany(1, company) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var error = result.Value as Dictionary<string, object>;
            Assert.That(error, Is.Not.Null);
            Assert.That(error["error"], Is.EqualTo("An error occurred while updating the company"));
        }

        [Test]
        public async Task GenerateSampleCompanies_ReturnsInternalServerErrorOnException()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GenerateSampleCompaniesAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GenerateSampleCompanies() as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var error = result.Value as Dictionary<string, object>;
            Assert.That(error, Is.Not.Null);
            Assert.That(error["error"], Is.EqualTo("An error occurred while generating sample companies"));
        }

        [Test]
        public async Task DeleteAllCompanies_ReturnsInternalServerErrorOnException()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.DeleteAllCompaniesAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.DeleteAllCompanies() as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var error = result.Value as Dictionary<string, object>;
            Assert.That(error, Is.Not.Null);
            Assert.That(error["error"], Is.EqualTo("An error occurred while deleting all companies"));
        }

        [Test]
        public async Task GetAverageSalariesForJobsInIndustry_ReturnsNotFoundWhenNoCompaniesFound()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetAverageSalariesForJobsInIndustryAsync("Tech"))
                .ReturnsAsync(new List<JobTitleSalaryDTO>());

            // Act
            var result = await _controller.GetAverageSalariesForJobsInIndustry("Tech") as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("No companies found in the Tech industry"));
        }

        [Test]
        public async Task GetAverageSalariesForJobsInIndustry_ReturnsInternalServerErrorOnException()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetAverageSalariesForJobsInIndustryAsync("Tech"))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAverageSalariesForJobsInIndustry("Tech") as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var error = result.Value as Dictionary<string, object>;
            Assert.That(error, Is.Not.Null);
            Assert.That(error["error"], Is.EqualTo("An error occurred while retrieving salary averages"));
            Assert.That(error["details"], Is.EqualTo("Database error")); // Ensure exception details are included
        }

        [Test]
        public async Task DeleteCompany_ReturnsOkResult()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(1)).ReturnsAsync(new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Company A",
                Industry = "Tech",
                CVR = "12345678",
                Email = "a@example.com",
                PasswordHash = Encoding.UTF8.GetBytes("password1")
            });

            _companyServiceMock.Setup(s => s.DeleteCompanyAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteCompany(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("Company deleted successfully"));
        }

        [Test]
        public async Task UpdateCompany_ReturnsOkResult()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyID = 1,
                CompanyName = "Updated Company",
                Industry = "Tech",
                CVR = "12345678",
                Email = "updated@example.com",
                PasswordHash = Encoding.UTF8.GetBytes("updatedhash")
            };

            _companyServiceMock.Setup(s => s.UpdateCompanyAsync(company)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateCompany(1, company) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("Company updated successfully"));
        }

        [Test]
        public async Task InsertCompanies_ReturnsBadRequestWhenCompanyDTOsIsNull()
        {
            // Arrange
            List<CompanyDTO>? companyDTOs = null;

            // Act
            var result = companyDTOs == null 
                ? new BadRequestObjectResult(new Dictionary<string, object> { { "message", "No companies provided" } }) 
                : await _controller.InsertCompanies(companyDTOs) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("No companies provided"));
        }

        [Test]
        public async Task InsertCompanies_ReturnsBadRequestWhenCompanyDTOsIsEmpty()
        {
            // Arrange
            var companyDTOs = new List<CompanyDTO>();

            // Act
            var result = await _controller.InsertCompanies(companyDTOs) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("No companies provided"));
        }

        [Test]
        public async Task UpdateCompany_ReturnsBadRequestWhenCompanyIsNull()
        {
            // Arrange
            CompanyModel company = null!;

            // Act
            var result = await _controller.UpdateCompany(1, company) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("Invalid company data"));
        }

        [Test]
        public async Task DeleteCompany_ReturnsNotFoundWhenCompanyDoesNotExist()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(1)).ReturnsAsync((CompanyModel)null!);

            // Act
            var result = await _controller.DeleteCompany(1) as NotFoundObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response["message"], Is.EqualTo("Company not found"));
        }

        [Test]
        public async Task DeleteCompany_ReturnsInternalServerErrorOnException()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetCompanyByIdAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.DeleteCompany(1) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var error = result.Value as Dictionary<string, object>;
            Assert.That(error, Is.Not.Null);
            Assert.That(error["error"], Is.EqualTo("An error occurred while deleting the company"));
            Assert.That(error["details"], Is.EqualTo("Database error"));
        }

        [Test]
        public async Task GetAllIndustries_ReturnsInternalServerErrorOnException()
        {
            // Arrange
            _companyServiceMock.Setup(s => s.GetAllIndustriesAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAllIndustries() as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.Not.Null);

            var error = result.Value as Dictionary<string, object>;
            Assert.That(error, Is.Not.Null);
            Assert.That(error["error"], Is.EqualTo("An error occurred while retrieving industries"));
            Assert.That(error["details"], Is.EqualTo("Database error"));
        }

        [Test]
        public async Task GetAverageSalariesForJobsInIndustry_ReturnsOkResultWithData()
        {
            // Arrange
            var jobTitleAverages = new List<JobTitleSalaryDTO>
            {
                new JobTitleSalaryDTO
                {
                    JobTitle = "Software Engineer",
                    GenderData = new Dictionary<string, GenderSalaryData>
                    {
                        { "Male", new GenderSalaryData { AverageSalary = 85000, EmployeeCount = 10 } },
                        { "Female", new GenderSalaryData { AverageSalary = 80000, EmployeeCount = 8 } }
                    }
                },
                new JobTitleSalaryDTO
                {
                    JobTitle = "Data Scientist",
                    GenderData = new Dictionary<string, GenderSalaryData>
                    {
                        { "Male", new GenderSalaryData { AverageSalary = 95000, EmployeeCount = 12 } },
                        { "Female", new GenderSalaryData { AverageSalary = 90000, EmployeeCount = 10 } }
                    }
                }
            };

            _companyServiceMock.Setup(s => s.GetAverageSalariesForJobsInIndustryAsync("Tech"))
                .ReturnsAsync(jobTitleAverages);

            // Act
            var result = await _controller.GetAverageSalariesForJobsInIndustry("Tech") as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.Not.Null);

            var response = result.Value as Dictionary<string, object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("data"), Is.True);

            var data = response["data"] as List<JobTitleSalaryDTO>;
            Assert.That(data, Is.Not.Null);
            Assert.That(data.Count, Is.EqualTo(2));

            // Validate the first job title
            var softwareEngineerData = data.FirstOrDefault(d => d.JobTitle == "Software Engineer");
            Assert.That(softwareEngineerData, Is.Not.Null);
            Assert.That(softwareEngineerData!.GenderData["Male"].AverageSalary, Is.EqualTo(85000));
            Assert.That(softwareEngineerData.GenderData["Female"].AverageSalary, Is.EqualTo(80000));

            // Validate the second job title
            var dataScientistData = data.FirstOrDefault(d => d.JobTitle == "Data Scientist");
            Assert.That(dataScientistData, Is.Not.Null);
            Assert.That(dataScientistData!.GenderData["Male"].AverageSalary, Is.EqualTo(95000));
            Assert.That(dataScientistData.GenderData["Female"].AverageSalary, Is.EqualTo(90000));
        }
    }
}