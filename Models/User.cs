using System.ComponentModel.DataAnnotations;

namespace Quizadilla.Models
{
    public class User
    {
        [Required]
        [Key]
        public int UserID { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string PasswordHash{ get; set; }
        
    }
}
