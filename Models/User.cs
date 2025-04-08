using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; } // Changed from Id to UserId

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
    }
}