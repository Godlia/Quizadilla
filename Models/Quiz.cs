using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Quizadilla.Models
{
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Use a collection type instead of an array for EF navigation
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
