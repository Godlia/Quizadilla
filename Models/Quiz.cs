using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;  
using Quizadilla.Models;

namespace Quizadilla.Models
{
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }

        [Required]
        public string UserID { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ICollection<Question> Questions { get; set; } = new List<Question>();

        public string? Theme { get; set; }
    }
}
