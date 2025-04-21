using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class SalaryModel
    {
        [Key]
        public int SalaryID { get; set; } // Primary key for salary record

        [ForeignKey("EmployeeID")]
        public int EmployeeID { get; set; } // FK to Employee

        public double Salary { get; set; } // Salary value

        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // When salary was recorded
    }
}
