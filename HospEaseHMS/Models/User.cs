using System.ComponentModel.DataAnnotations;

namespace HospEaseHMS.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Stored as hash

        [Required]
        public string Role { get; set; } // "Admin", "Doctor", or "Patient"
    }
}
