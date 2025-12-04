using Microsoft.EntityFrameworkCore;
using Quizadilla.Models;

public class QuizDbContext : DbContext
{
    public QuizDbContext(DbContextOptions<QuizDbContext> options)
        : base(options)
    {
    }

    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Option> Options { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}