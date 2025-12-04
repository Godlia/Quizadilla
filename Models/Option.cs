using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizadilla.Models
{
    [Table("Option")]
    public class Option
    {
        [Key]
        public int OptionId { get; set; }

        [Required]
        public string OptionText { get; set; } = string.Empty;
    }
}
