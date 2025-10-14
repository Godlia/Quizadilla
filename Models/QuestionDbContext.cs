using Microsoft.EntityFrameworkCore;

namespace project.Models
{
    public class QuestionDbContext : DbContext
    {
        public QuestionDbContext(DbContextOptions<QuestionDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Question> Questions { get; set; } = null!;
    }
}
