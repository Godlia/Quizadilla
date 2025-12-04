using System.Collections.Generic;

namespace Quizadilla.Dtos
{
    public class CreateQuizDto
    {
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public List<QuestionDto> Questions { get; set; } = new();
    }

    public class QuestionDto
    {
        public string QuestionText { get; set; } = "";
        public string? CorrectString { get; set; }
        public List<CreateOptionDto> Options { get; set; } = new();
    }

    public class CreateOptionDto
    {
        public string OptionText { get; set; } = "";
    }
}
