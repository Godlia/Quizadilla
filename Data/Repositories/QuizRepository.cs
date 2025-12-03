using Microsoft.EntityFrameworkCore;
using Quizadilla.Models;
using System.Linq;
using System.Collections.Generic;

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
            .ThenInclude(q => q.Options)
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
        // Ensure each question has Options initialized and at least one correct option
        foreach (var q in quiz.Questions ?? Enumerable.Empty<Question>())
        {
            q.Options ??= new List<Option>();
            if (!q.Options.Any(o => o.IsCorrect))
            {
                if (q.Options.Any())
                    q.Options.First().IsCorrect = true;
            }
        }

        _db.Quizzes.Add(quiz);
    }

    public void DeleteQuiz(int id)
    {
        var quiz = GetQuizWithDetails(id);
        if (quiz == null) return;

        foreach (var question in quiz.Questions.ToList())
        {
            foreach (var option in question.Options.ToList())
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

        // Add/update questions & Options
        foreach (var q in updatedQuiz.Questions ?? Enumerable.Empty<Question>())
        {
            var eq = existingQuestions.FirstOrDefault(x => x.Id == q.Id);

            if (eq == null)
            {
                // New question: ensure Options initialized and a correct option exists
                q.Options ??= new List<Option>();
                if (!q.Options.Any(o => o.IsCorrect) && q.Options.Any())
                    q.Options.First().IsCorrect = true;

                existingQuiz.Questions.Add(q);
                continue;
            }

            eq.QuestionText = q.QuestionText;

            var existingOptions = eq.Options.ToList();

            // Remove deleted Options
            foreach (var eo in existingOptions)
            {
                if (!(q.Options?.Any(o => o.OptionId == eo.OptionId) ?? false))
                    _db.Remove(eo);
            }

            // Add or update Options
            foreach (var opt in q.Options ?? Enumerable.Empty<Option>())
            {
                var eo = existingOptions.FirstOrDefault(o => o.OptionId == opt.OptionId);

                if (eo == null)
                {
                    eq.Options.Add(opt);
                }
                else
                {
                    eo.OptionText = opt.OptionText;
                    eo.IsCorrect = opt.IsCorrect; // persist correctness flag
                }
            }

            // Ensure at least one option is marked correct after updates
            if (!eq.Options.Any(o => o.IsCorrect) && eq.Options.Any())
                eq.Options.First().IsCorrect = true;
        }
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}
