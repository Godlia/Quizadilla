using Quizadilla.Models;

public interface IQuizRepository
{
    Quiz? GetQuizWithDetails(int id);
    List<Quiz> GetQuizzes();
    List<Quiz> GetUserQuizzes(string userId);
    void AddQuiz(Quiz quiz);
    Quiz? GetQuizForEdit(int id);
    void UpdateQuiz(Quiz updatedQuiz);
    void DeleteQuiz(int id);
    void Save();
}
