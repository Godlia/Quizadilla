using System.Collections.Generic;

namespace Quizadilla.Dtos
{
    // --------------------------
    // QUIZ DTO (for update)
    // --------------------------
    public class UpdateQuizDto
    {
        public string Title { get; set; } = "";
        public string? Description { get; set; }

        // NEW: theme support
        public string? Theme { get; set; }

        // Liste av spørsmål
        public List<UpdateQuestionDto> Questions { get; set; } = new();
    }

    // --------------------------
    // QUESTION DTO
    // --------------------------
    public class UpdateQuestionDto
    {
        // 0 = nytt spørsmål
        public int Id { get; set; }

        public string QuestionText { get; set; } = "";

        // Optional (samme som i EF-modellen)
        public string? CorrectString { get; set; }

        // Liste av alternativer
        public List<UpdateOptionDto> Options { get; set; } = new();
    }

    // --------------------------
    // OPTION DTO
    // --------------------------
    public class UpdateOptionDto
    {
        // 0 = nytt alternativ
        public int OptionId { get; set; }

        public string OptionText { get; set; } = "";
    }
}
