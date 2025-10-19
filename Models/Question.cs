using System.ComponentModel.DataAnnotations;

namespace Quizadilla.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        public ICollection<string> options { get; set; } = new List<string>();

        public int correctAnswerIndex { get; set; }


    }
}
