using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    public class Quiz
    {
        [Required]
        public int userID { get; set; } //points to which user created this quiz (foreign key)
        
        [Required]
        public int QuizId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Question[] Questions { get; set; } = Array.Empty<Question>();
    }
}
