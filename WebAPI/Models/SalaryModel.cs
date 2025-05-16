using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
namespace Backend.Models
{
    [ExcludeFromCodeCoverage]
    public class SalaryModel
    {
        [Key]
        public int SalaryID { get; set; } // Primary key for salary record

        [ForeignKey("EmployeeID")]
        public EmployeeModel? Employee { get; set; }
        public int EmployeeID { get; set; } // FK to Employee

        public double Salary { get; set; } // Salary value

        public DateTime Timestamp { get; set; } // When salary was recorded
    }
}
