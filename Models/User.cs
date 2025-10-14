using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    public class User
    {
        [Required]
        public int UserID { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string PasswordHash{ get; set; }
        
    }
}
