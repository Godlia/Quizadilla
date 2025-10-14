using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    public class Question
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int QuizId { get; set; } //points to which quiz this question belongs to (foreign key)
        [Required]
        public int UserID { get; set; } //points to which user created this question (foreign key)
        public string QuestionText { get; set; } = string.Empty;
        public string[] Options { get; set; } = Array.Empty<string>();
        public int CorrectOptionIndex { get; set; }

    }
}
