using System.ComponentModel.DataAnnotations;

namespace Quizadilla.Models
{
    public class Option
    {
        [Key]
        public int OptionId { get; set; }
        [Required]
        public string OptionText { get; set; } = string.Empty;

        public bool IsCorrect { get; set; } = false;
        public string toString()
        {
            string returnString = string.Empty;
            returnString += "Option ID: " + OptionId + "\n";
            returnString += "Option Text: " + OptionText + "\n";
            return returnString;
        }

    }
}
