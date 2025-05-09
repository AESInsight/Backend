using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class EmployeeIndustry
{
    [Key]
    public int EmployeeID { get; set; }

    [Required]
    public string JobTitle { get; set; }

    [Required]
    public int CompanyID { get; set; }

    [Required]
    public string Industry { get; set; }

    [ForeignKey("EmployeeID")]
    public virtual EmployeeModel Employee { get; set; }

    [ForeignKey("CompanyID")]
    public virtual CompanyModel Company { get; set; }
}