using project.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizadilla.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        public ICollection<Option> options { get; set; } = new List<Option>();

        public string correctString { get; set; } = string.Empty;

        public string toString()
        {
            string returnString = string.Empty;
            returnString += "Question ID: " + Id + "\n";
            returnString += "Question Text: " + QuestionText + "\n";
            returnString += "Options: \n";
            foreach (var option in options)
            {
                returnString += "\t" + option.toString() + "\n";
            }
            returnString += "Correct Answer: " + correctString + "\n";
            return returnString;
        }
    }
}
