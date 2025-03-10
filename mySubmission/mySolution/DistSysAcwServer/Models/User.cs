using System.ComponentModel.DataAnnotations;

namespace DistSysAcwServer.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string PasswordHash { get; set; }
    }
}