using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class User
{
    [Key]
    public int UserId { get; set; } // Primary key for the user

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = null!;

    [Required]
    public byte[] PasswordHash { get; set; } = null!;

    [Required]
    public byte[] PasswordSalt { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "User"; // Default role

    public int? CompanyID { get; set; } // Nullable foreign key to associate the user with a company

    [ForeignKey("CompanyID")]
    public CompanyModel? Company { get; set; } // Navigation property for the associated company
}