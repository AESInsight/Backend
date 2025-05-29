using Backend.Data;
using Backend.Models;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Tests.Services
{
    [TestFixture]
    public class TestEmployeeService
    {
        private ApplicationDbContext _dbContext;
        private EmployeeService _service;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);
            // Seed the database with test data (from HEAD)
            _dbContext.Companies.AddRange(new List<CompanyModel>
            {
                new CompanyModel
                {
                    CompanyID = 1,
                    CompanyName = "TechCorp",
                    Industry = "Technology",
                    CVR = "12345678",
                    Email = "contact@techcorp.com",
                    PasswordHash = Encoding.UTF8.GetBytes("hashedpassword1")
                },
                new CompanyModel
                {
                    CompanyID = 2,
                    CompanyName = "DataCorp",
                    Industry = "Data Analytics",
                    CVR = "87654321",
                    Email = "info@datacorp.com",
                    PasswordHash = Encoding.UTF8.GetBytes("hashedpassword2")
                }
            });
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
            _service = new EmployeeService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            if (_dbContext != null)
            {
                _dbContext.Database.EnsureDeleted();
                _dbContext.Dispose();
                _dbContext = null!;
            }
        }

        [Test]
        public async Task BulkCreateEmployeesAsync_AddsEmployees_WhenDataIsValid()
        {
            // Arrange
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel { JobTitle = "Developer", Experience = 2, Gender = "Male", CompanyID = 1 },
                new EmployeeModel { JobTitle = "Designer", Experience = 3, Gender = "Female", CompanyID = 1 }
            };

            // Act
            var result = await _service.BulkCreateEmployeesAsync(employees);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            var dbEmployees = await _dbContext.Employee.ToListAsync();
            Assert.That(dbEmployees.Count, Is.EqualTo(2));
            Assert.That(dbEmployees.Any(e => e.JobTitle == "Developer" && e.Gender == "Male"));
            Assert.That(dbEmployees.Any(e => e.JobTitle == "Designer" && e.Gender == "Female"));
        }

        [Test]
        public async Task BulkCreateEmployeesAsync_ReturnsEmptyList_WhenInputIsEmpty()
        {
            // Arrange
            var employees = new List<EmployeeModel>();

            // Act
            var result = await _service.BulkCreateEmployeesAsync(employees);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            var dbEmployees = await _dbContext.Employee.ToListAsync();
            Assert.That(dbEmployees.Count, Is.EqualTo(0));
        }

        [Test]
        public void BulkCreateEmployeesAsync_ThrowsException_WhenDbContextFails()
        {
            // Arrange
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel { JobTitle = "Developer", Experience = 2, Gender = "Male", CompanyID = 1 }
            };

            // Simulate a disposed context to force an exception
            _dbContext.Dispose();
            _dbContext = null!; // <-- Add this line

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await _service.BulkCreateEmployeesAsync(employees);
            });
            Assert.That(ex!.Message, Does.Contain("Error during employee creation"));
        }

        [Test]
        public async Task GetAllEmployeesAsync_ReturnsAllEmployees()
        {
            // Act
            var result = await _service.GetAllEmployeesAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].JobTitle, Is.EqualTo("Software Engineer"));
            Assert.That(result[1].JobTitle, Is.EqualTo("Data Scientist"));
        }

        [Test]
        public async Task GetEmployeeByIdAsync_ReturnsCorrectEmployee()
        {
            // Act
            var result = await _service.GetEmployeeByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.JobTitle, Is.EqualTo("Software Engineer"));
            Assert.That(result.Experience, Is.EqualTo(5));
        }

        [Test]
        public void GetEmployeeByIdAsync_ThrowsExceptionForNonExistentEmployee()
        {
            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetEmployeeByIdAsync(999));
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
            await _service.BulkCreateEmployeesAsync(newEmployees);
            var employees = await _service.GetAllEmployeesAsync();

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
            await _service.BulkCreateEmployeesAsync(emptyList);
            var employees = await _service.GetAllEmployeesAsync();

            // Assert
            Assert.That(employees.Count, Is.EqualTo(2)); // No new employees should be added
        }

        [Test]
        public async Task DeleteEmployeeAsync_RemovesEmployee()
        {
            // Act
            await _service.DeleteEmployeeAsync(1);
            var employees = await _service.GetAllEmployeesAsync();

            // Assert
            Assert.That(employees.Any(e => e.EmployeeID == 1), Is.False);
        }

        [Test]
        public void DeleteEmployeeAsync_ThrowsExceptionForNonExistentEmployee()
        {
            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.DeleteEmployeeAsync(999));
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
            var result = await _service.UpdateEmployeeAsync(1, updatedEmployee);

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
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.UpdateEmployeeAsync(999, updatedEmployee));
        }

        [Test]
        public void UpdateEmployeeAsync_ThrowsExceptionForNullInput()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.UpdateEmployeeAsync(1, null!));
        }

        [Test]
        public async Task GetAllJobTitlesAsync_ReturnsUniqueJobTitles()
        {
            // Act
            var jobTitles = await _service.GetAllJobTitlesAsync();

            // Assert
            Assert.That(jobTitles.Count, Is.EqualTo(2));
            Assert.That(jobTitles, Does.Contain("Software Engineer"));
            Assert.That(jobTitles, Does.Contain("Data Scientist"));
        }

        [Test]
        public async Task GetEmployeesByJobTitleAsync_ReturnsCorrectEmployees()
        {
            // Act
            var employees = await _service.GetEmployeesByJobTitleAsync("Software Engineer");

            // Assert
            Assert.That(employees.Count, Is.EqualTo(1));
            Assert.That(employees[0].JobTitle, Is.EqualTo("Software Engineer"));
        }

        [Test]
        public void GetEmployeesByJobTitleAsync_ThrowsExceptionForNullOrEmptyJobTitle()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _service.GetEmployeesByJobTitleAsync(null!));
            Assert.ThrowsAsync<ArgumentException>(async () => await _service.GetEmployeesByJobTitleAsync(""));
        }

        [Test]
        public async Task DeleteAllEmployeesAsync_RemovesAllEmployees()
        {
            // Act
            await _service.DeleteAllEmployeesAsync();
            var employees = await _service.GetAllEmployeesAsync();

            // Assert
            Assert.That(employees.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllEmployeesAsync_ReturnsEmptyList_WhenDatabaseIsEmpty()
        {
            // Arrange
            await _service.DeleteAllEmployeesAsync(); // Ensure the database is empty

            // Act
            var employees = await _service.GetAllEmployeesAsync();

            // Assert
            Assert.That(employees, Is.Empty);
        }

        [Test]
        public async Task GetAllJobTitlesAsync_ReturnsEmptyList_WhenDatabaseIsEmpty()
        {
            // Arrange
            await _service.DeleteAllEmployeesAsync(); // Ensure the database is empty

            // Act
            var jobTitles = await _service.GetAllJobTitlesAsync();

            // Assert
            Assert.That(jobTitles, Is.Empty);
        }

        [Test]
        public void BulkCreateEmployeesAsync_ThrowsExceptionForNullInput()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.BulkCreateEmployeesAsync(null!));
        }

        [Test]
        public async Task DeleteAllEmployeesAsync_ThrowsException_WhenNoEmployeesExist()
        {
            // Arrange
            await _service.DeleteAllEmployeesAsync(); // Ensure the database is empty

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.DeleteAllEmployeesAsync());
        }

        [Test]
        public async Task GetSalaryDifferencesByGenderAsync_ReturnsCorrectSalaryDifferences()
        {
            // Arrange
            var jobTitle = "Software Engineer";

            // Act
            var result = await _service.GetSalaryDifferencesByGenderAsync(jobTitle);

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
            var result = await _service.GetSalaryDifferencesByGenderAsync(jobTitle);

            // Assert
            Assert.That(result["Male"].Count, Is.EqualTo(0));
            Assert.That(result["Female"].Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetMaxEmployeeIdAsync_ReturnsCorrectMaxId()
        {
            // Act
            var maxId = await _service.GetMaxEmployeeIdAsync();

            // Assert
            Assert.That(maxId, Is.EqualTo(2)); // The highest EmployeeID in the seeded data
        }

        [Test]
        public async Task GetMaxEmployeeIdAsync_ReturnsZero_WhenNoEmployeesExist()
        {
            // Act
            var maxId = await _service.GetMaxEmployeeIdAsync();

            // Assert
            Assert.That(maxId, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllSalaryDifferencesByGenderAsync_ReturnsCorrectSalaryDifferences()
        {
            // Act
            var result = await _service.GetAllSalaryDifferencesByGenderAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContainsKey("Male"), Is.True);
            Assert.That(result.ContainsKey("Female"), Is.True);

            var maleList = result["Male"];
            var femaleList = result["Female"];

            Assert.That(maleList.Count, Is.EqualTo(2)); // Jan and Feb for male developer
            Assert.That(femaleList.Count, Is.EqualTo(1)); // Feb for female developer

            Assert.That(maleList[0].Month, Is.EqualTo(new DateTime(2024, 1, 1)));
            Assert.That(maleList[0].AverageSalary, Is.EqualTo(60000));
            Assert.That(maleList[1].Month, Is.EqualTo(new DateTime(2024, 2, 1)));
            Assert.That(maleList[1].AverageSalary, Is.EqualTo(65000));
            Assert.That(femaleList[0].Month, Is.EqualTo(new DateTime(2024, 2, 1)));
            Assert.That(femaleList[0].AverageSalary, Is.EqualTo(70000));
        }

        [Test]
        public async Task GetSalaryDifferencesByGenderAsync_ReturnsEmptyLists_WhenNoEmployeesWithJobTitle()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "test@company.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetSalaryDifferencesByGenderAsync("NonExistentJob");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result["Male"], Is.Empty);
            Assert.That(result["Female"], Is.Empty);
        }

        [Test]
        public async Task GetSalaryDifferencesByGenderAsync_ReturnsEmptyLists_WhenEmployeesHaveNoSalaries()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "test@company.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            var employee = new EmployeeModel
            {
                CompanyID = company.CompanyID,
                JobTitle = "Developer",
                Gender = "Male"
            };
            _dbContext.Employee.Add(employee);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetSalaryDifferencesByGenderAsync("Developer");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result["Male"], Is.Empty);
            Assert.That(result["Female"], Is.Empty);
        }

        [Test]
        public async Task GetAllSalaryDifferencesByGenderAsync_ReturnsCorrectSalaryDifferences_ForAllJobs()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "test@company.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            var employee1 = new EmployeeModel
            {
                CompanyID = company.CompanyID,
                JobTitle = "Developer",
                Gender = "Male"
            };
            var employee2 = new EmployeeModel
            {
                CompanyID = company.CompanyID,
                JobTitle = "Developer",
                Gender = "Female"
            };
            var employee3 = new EmployeeModel
            {
                CompanyID = company.CompanyID,
                JobTitle = "Designer",
                Gender = "Male"
            };
            _dbContext.Employee.AddRange(employee1, employee2, employee3);
            await _dbContext.SaveChangesAsync();

            var salary1 = new SalaryModel
            {
                EmployeeID = employee1.EmployeeID,
                Salary = 60000,
                Timestamp = new DateTime(2024, 1, 1)
            };
            var salary2 = new SalaryModel
            {
                EmployeeID = employee1.EmployeeID,
                Salary = 65000,
                Timestamp = new DateTime(2024, 2, 1)
            };
            var salary3 = new SalaryModel
            {
                EmployeeID = employee2.EmployeeID,
                Salary = 70000,
                Timestamp = new DateTime(2024, 2, 1)
            };
            var salary4 = new SalaryModel
            {
                EmployeeID = employee3.EmployeeID,
                Salary = 55000,
                Timestamp = new DateTime(2024, 2, 1)
            };
            _dbContext.Salaries.AddRange(salary1, salary2, salary3, salary4);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetAllSalaryDifferencesByGenderAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContainsKey("Male"), Is.True);
            Assert.That(result.ContainsKey("Female"), Is.True);

            var maleList = result["Male"];
            var femaleList = result["Female"];

            Assert.That(maleList.Count, Is.EqualTo(2));
            Assert.That(femaleList.Count, Is.EqualTo(1));

            Assert.That(maleList.Any(x => x.Month == new DateTime(2024, 1, 1) && x.AverageSalary == 60000));
            Assert.That(maleList.Any(x => x.Month == new DateTime(2024, 2, 1) && x.AverageSalary == 60000)); // (65000+55000)/2
            Assert.That(femaleList.Any(x => x.Month == new DateTime(2024, 2, 1) && x.AverageSalary == 70000));
        }

        [Test]
        public async Task GetAllSalaryDifferencesByGenderAsync_ReturnsEmptyLists_WhenNoSalariesExist()
        {
            // Act
            var result = await _service.GetAllSalaryDifferencesByGenderAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result["Male"], Is.Empty);
            Assert.That(result["Female"], Is.Empty);
        }

        [Test]
        public async Task GetAllSalaryDifferencesByGenderAsync_ReturnsEmptyLists_WhenEmployeesHaveNoSalaries()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "test@company.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            var employee = new EmployeeModel
            {
                CompanyID = company.CompanyID,
                JobTitle = "Developer",
                Gender = "Male"
            };
            _dbContext.Employee.Add(employee);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetAllSalaryDifferencesByGenderAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result["Male"], Is.Empty);
            Assert.That(result["Female"], Is.Empty);
        }

        [Test]
        public async Task GetEmployeeIndustryByIdAsync_ReturnsIndustry_WhenEmployeeAndCompanyExist()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "TestCompany",
                Industry = "Tech",
                CVR = "12345678",
                Email = "test@company.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            var employee = new EmployeeModel
            {
                JobTitle = "Developer",
                Experience = 2,
                Gender = "Male",
                CompanyID = company.CompanyID
            };
            _dbContext.Employee.Add(employee);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetEmployeeIndustryByIdAsync(employee.EmployeeID);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.EmployeeID, Is.EqualTo(employee.EmployeeID));
            Assert.That(result.CompanyID, Is.EqualTo(company.CompanyID));
            Assert.That(result.Industry, Is.EqualTo("Tech"));
        }

        [Test]
        public async Task GetEmployeeIndustryByIdAsync_ReturnsNull_WhenEmployeeDoesNotExist()
        {
            // Act
            var result = await _service.GetEmployeeIndustryByIdAsync(9999);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEmployeeIndustryByIdAsync_ReturnsNull_WhenEmployeeHasNoCompany()
        {
            // Arrange
            var employee = new EmployeeModel
            {
                JobTitle = "Developer",
                Experience = 2,
                Gender = "Male",
                CompanyID = 9999 // Non-existent company
            };
            _dbContext.Employee.Add(employee);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetEmployeeIndustryByIdAsync(employee.EmployeeID);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEmployeesByCompanyIdAsync_ReturnsEmployees_WhenCompanyHasEmployees()
        {
            // Arrange
            var company1 = new CompanyModel
            {
                CompanyName = "Company1",
                Industry = "Tech",
                CVR = "12345678",
                Email = "company1@test.com"
            };
            var company2 = new CompanyModel
            {
                CompanyName = "Company2",
                Industry = "Finance",
                CVR = "87654321",
                Email = "company2@test.com"
            };
            _dbContext.Companies.AddRange(company1, company2);
            await _dbContext.SaveChangesAsync();

            var employee1 = new EmployeeModel
            {
                JobTitle = "Developer",
                Experience = 2,
                Gender = "Male",
                CompanyID = company1.CompanyID
            };
            var employee2 = new EmployeeModel
            {
                JobTitle = "Designer",
                Experience = 3,
                Gender = "Female",
                CompanyID = company1.CompanyID
            };
            var employee3 = new EmployeeModel
            {
                JobTitle = "Analyst",
                Experience = 1,
                Gender = "Male",
                CompanyID = company2.CompanyID
            };
            _dbContext.Employee.AddRange(employee1, employee2, employee3);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetEmployeesByCompanyIdAsync(company1.CompanyID);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(e => e.CompanyID == company1.CompanyID));
            Assert.That(result.Any(e => e.JobTitle == "Developer"));
            Assert.That(result.Any(e => e.JobTitle == "Designer"));
            Assert.That(result.All(e => e.Company != null && e.Company.CompanyName == "Company1"));
        }

        [Test]
        public async Task GetEmployeesByCompanyIdAsync_ReturnsEmptyList_WhenCompanyHasNoEmployees()
        {
            // Arrange
            var company = new CompanyModel
            {
                CompanyName = "Company1",
                Industry = "Tech",
                CVR = "12345678",
                Email = "company1@test.com"
            };
            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetEmployeesByCompanyIdAsync(company.CompanyID);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetEmployeesByCompanyIdAsync_ReturnsEmptyList_WhenCompanyDoesNotExist()
        {
            // Act
            var result = await _service.GetEmployeesByCompanyIdAsync(9999);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetSalaryDifferencesByGenderAsync_ThrowsException_WhenDbContextFails()
        {
            // Arrange
            var jobTitle = "Developer";
            _dbContext.Dispose(); // Dispose context to force an exception
            _dbContext = null!;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            {
                await _service.GetSalaryDifferencesByGenderAsync(jobTitle);
            });
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Does.Contain("Cannot access a disposed context instance"));
        }
    }
}