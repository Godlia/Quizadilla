using System.Collections.Generic;

namespace Quizadilla.Dtos
{
   
    public class UpdateQuizDto
    {
        public string Title { get; set; } = "";
        public string? Description { get; set; }

       
       
        public string? Theme { get; set; }

                public List<UpdateQuestionDto> Questions { get; set; } = new();
    }

    
    public class UpdateQuestionDto
    {
      
        public int Id { get; set; }

        public string QuestionText { get; set; } = "";


       
        public List<UpdateOptionDto> Options { get; set; } = new();
    }

       public class UpdateOptionDto
    {
       
        public int OptionId { get; set; }

        public string OptionText { get; set; } = "";
        public bool IsCorrect { get; set; } = false;
    }
}
