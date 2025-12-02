using Quizadilla.Models;

namespace Quizadilla.Services
{
    public interface IQuizService
    {
        Quiz? GetQuizForEdit(int id);
        Quiz? GetQuizForPlay(int id);
        List<Quiz> GetAllQuizzes();
        List<Quiz> GetQuizzesForUser(string userId);

        Quiz CreateQuiz(Quiz quiz, string userId);
        Quiz? UpdateQuiz(Quiz updatedQuiz);
        bool DeleteQuiz(int id);
    }
}
