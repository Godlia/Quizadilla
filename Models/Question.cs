using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;      
using System.ComponentModel.DataAnnotations.Schema; 
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

        public ICollection<Option> Options { get; set; } = new List<Option>();


        public string toString()
        {
            string returnString = string.Empty;
            returnString += "Question ID: " + Id + "\n";
            returnString += "Question Text: " + QuestionText + "\n";
            returnString += "Options: \n";
            foreach (var option in Options)
            {
                returnString += "\t" + option.toString() + "\n";
            }
            return returnString;
        }
    }
}
