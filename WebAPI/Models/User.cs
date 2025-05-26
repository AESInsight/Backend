using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Backend.Models
{
    [ExcludeFromCodeCoverage]
    public class User
    {
        [Key]
        public int UserId { get; set; } // Changed from Id to UserId

        [Required]
        [MaxLength(50)]
        public string Email { get; set; } = null!;

        [Required]
        public byte[] PasswordHash { get; set; } = null!;

        [Required]
        public byte[] PasswordSalt { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "User"; // Default role

        // New: Foreign key to Company (nullable, only set for company users)
        public int? CompanyID { get; set; }
        [ForeignKey("CompanyID")]
        public CompanyModel? Company { get; set; }

        // New: Password reset fields
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpiry { get; set; }
    }
}