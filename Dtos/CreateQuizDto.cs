using System.Collections.Generic;

namespace Quizadilla.Dtos
{
    public class CreateQuizDto
    {
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string? Theme { get; set; }

        public List<CreateQuestionDto> Questions { get; set; } = new();
    }

    public class CreateQuestionDto
    {
        public string QuestionText { get; set; } = "";

        public List<CreateOptionDto> Options { get; set; } = new();
    }

    public class CreateOptionDto
    {
        public string OptionText { get; set; } = "";
        public bool IsCorrect { get; set; } = false;
    }
}
