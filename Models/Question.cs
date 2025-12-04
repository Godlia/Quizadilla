using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;       // ← Required + Key
using System.ComponentModel.DataAnnotations.Schema; // ← Table
using Quizadilla.Models;

namespace Quizadilla.Models
{
    [Table("Question")]
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        public ICollection<Option> options { get; set; } = new List<Option>();

        public string correctString { get; set; } = string.Empty;
    }
}
