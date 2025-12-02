using Microsoft.EntityFrameworkCore;
using Quizadilla.Models;

namespace Quizadilla.Services
{
    public class QuizService : IQuizService
    {
        private readonly QuizDbContext _db;

        // same themes as controller used
        private static readonly string[] Themes = { "tomato", "guac", "cheese", "onion", "chicken", "salsa" };
        private static readonly Random Rng = new();

        public QuizService(QuizDbContext db)
        {
            _db = db;
        }

        // ----- READ -----

        public Quiz? GetQuizForEdit(int id)
        {
            return _db.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.options)
                .FirstOrDefault(q => q.QuizId == id);
        }

        public Quiz? GetQuizForPlay(int id)
        {
            var quiz = _db.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.options)
                .FirstOrDefault(q => q.QuizId == id);

            if (quiz == null) return null;

            // Randomize options for each question (your old Quiz action)
            foreach (var question in quiz.Questions)
            {
                question.options = question.options
                    .OrderBy(_ => Rng.Next())
                    .ToList();
            }

            return quiz;
        }

        public List<Quiz> GetAllQuizzes()
        {
            return _db.Quizzes
                .Include(q => q.Questions)
                .ToList();
        }

        public List<Quiz> GetQuizzesForUser(string userId)
        {
            return _db.Quizzes
                .Where(q => q.UserID == userId)
                .Include(q => q.Questions)
                .ToList();
        }

        // ----- CREATE -----

        public Quiz CreateQuiz(Quiz quiz, string userId)
        {
            // ensure relations are not null
            quiz.Questions ??= new List<Question>();

            // your "add correct option if missing" logic
            foreach (var q in quiz.Questions)
            {
                q.options ??= new List<Option>();

                var correct = (q.correctString ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(correct))
                {
                    var hasCorrect = q.options.Any(o =>
                        string.Equals((o.OptionText ?? string.Empty).Trim(),
                                      correct,
                                      StringComparison.OrdinalIgnoreCase));

                    if (!hasCorrect)
                    {
                        q.options.Add(new Option { OptionText = q.correctString });
                    }
                }
            }

            // random theme if missing
            if (string.IsNullOrWhiteSpace(quiz.Theme))
            {
                quiz.Theme = Themes[Rng.Next(Themes.Length)];
            }

            // set user id here (inside service, as you chose)
            quiz.UserID = userId;

            _db.Quizzes.Add(quiz);
            _db.SaveChanges();

            return quiz;
        }

        // ----- UPDATE -----

        public Quiz? UpdateQuiz(Quiz updatedQuiz)
        {
            var existingQuiz = _db.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.options)
                .FirstOrDefault(q => q.QuizId == updatedQuiz.QuizId);

            if (existingQuiz == null)
                return null;

            // update simple fields
            existingQuiz.Title = updatedQuiz.Title;
            existingQuiz.Description = updatedQuiz.Description;
            existingQuiz.Theme = updatedQuiz.Theme;

            // handle questions
            var existingQuestions = existingQuiz.Questions.ToList();

            // remove deleted questions
            foreach (var existingQuestion in existingQuestions)
            {
                if (!updatedQuiz.Questions.Any(q => q.Id == existingQuestion.Id))
                {
                    _db.Remove(existingQuestion);
                }
            }

            // add or update questions
            foreach (var q in updatedQuiz.Questions)
            {
                var existingQuestion = existingQuestions.FirstOrDefault(eq => eq.Id == q.Id);

                if (existingQuestion == null)
                {
                    // new question
                    existingQuiz.Questions.Add(q);
                }
                else
                {
                    // update question fields
                    existingQuestion.QuestionText = q.QuestionText;
                    existingQuestion.correctString = q.correctString;

                    existingQuestion.options ??= new List<Option>();
                    q.options ??= new List<Option>();

                    var existingOptions = existingQuestion.options.ToList();

                    // remove deleted options
                    foreach (var existingOpt in existingOptions)
                    {
                        if (!q.options.Any(o => o.OptionId == existingOpt.OptionId))
                        {
                            _db.Remove(existingOpt);
                        }
                    }

                    // add or update options
                    foreach (var opt in q.options)
                    {
                        var existingOpt = existingOptions.FirstOrDefault(o => o.OptionId == opt.OptionId);
                        if (existingOpt == null)
                        {
                            existingQuestion.options.Add(opt);
                        }
                        else
                        {
                            existingOpt.OptionText = opt.OptionText;
                        }
                    }
                }
            }

            _db.SaveChanges();
            return existingQuiz;
        }

        // ----- DELETE -----

        public bool DeleteQuiz(int id)
        {
            var quiz = _db.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.options)
                .FirstOrDefault(q => q.QuizId == id);

            if (quiz == null)
                return false;

            // Remove related entities (Options -> Questions -> Quiz)
            foreach (var question in quiz.Questions.ToList())
            {
                if (question.options != null && question.options.Any())
                {
                    foreach (var option in question.options.ToList())
                    {
                        _db.Remove(option);
                    }
                }

                _db.Remove(question);
            }

            _db.Remove(quiz);
            _db.SaveChanges();

            return true;
        }
    }
}
