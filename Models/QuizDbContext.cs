using Microsoft.EntityFrameworkCore;

namespace project.Models
{
    public class QuizDbContext : DbContext
    {
        public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Quiz> Quizzes { get; set; } = null!;
    }
}
