using Microsoft.EntityFrameworkCore;
using Quizadilla.Models;

public class QuizRepository : IQuizRepository
{
    private readonly QuizDbContext _db;

    public QuizRepository(QuizDbContext db)
    {
        _db = db;
    }

    public Quiz? GetQuizWithDetails(int id)
    {
        return _db.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.options)
            .FirstOrDefault(q => q.QuizId == id);
    }

    public Quiz? GetQuizForEdit(int id)
    {
        return GetQuizWithDetails(id);
    }

    public List<Quiz> GetQuizzes()
    {
        return _db.Quizzes.Include(q => q.Questions).ToList();
    }

    public List<Quiz> GetUserQuizzes(string userId)
    {
        return _db.Quizzes
            .Where(q => q.UserID == userId)
            .ToList();
    }

    public void AddQuiz(Quiz quiz)
    {
        _db.Quizzes.Add(quiz);
    }

    public void DeleteQuiz(int id)
    {
        var quiz = GetQuizWithDetails(id);
        if (quiz == null) return;

        foreach (var question in quiz.Questions.ToList())
        {
            foreach (var option in question.options.ToList())
                _db.Remove(option);

            _db.Remove(question);
        }

        _db.Remove(quiz);
    }

    public void UpdateQuiz(Quiz updatedQuiz)
    {
        var existingQuiz = GetQuizWithDetails(updatedQuiz.QuizId);
        if (existingQuiz == null) return;

        existingQuiz.Title = updatedQuiz.Title;
        existingQuiz.Description = updatedQuiz.Description;

        var existingQuestions = existingQuiz.Questions.ToList();

        // Remove deleted questions
        foreach (var eq in existingQuestions)
        {
            if (!updatedQuiz.Questions.Any(q => q.Id == eq.Id))
                _db.Remove(eq);
        }

        // Add/update questions & options
        foreach (var q in updatedQuiz.Questions)
        {
            var eq = existingQuestions.FirstOrDefault(x => x.Id == q.Id);

            if (eq == null)
            {
                existingQuiz.Questions.Add(q);
                continue;
            }

            eq.QuestionText = q.QuestionText;
            eq.correctString = q.correctString;

            var existingOptions = eq.options.ToList();

            // Remove deleted options
            foreach (var eo in existingOptions)
            {
                if (!q.options.Any(o => o.OptionId == eo.OptionId))
                    _db.Remove(eo);
            }

            // Add or update options
            foreach (var opt in q.options)
            {
                var eo = existingOptions.FirstOrDefault(o => o.OptionId == opt.OptionId);

                if (eo == null)
                {
                    eq.options.Add(opt);
                }
                else
                {
                    eo.OptionText = opt.OptionText;
                }
            }
        }
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}
