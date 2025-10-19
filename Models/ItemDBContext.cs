using Microsoft.EntityFrameworkCore;

namespace Quizadilla.Models;

public class ItemDBContext : DbContext
{
    public ItemDBContext(DbContextOptions<ItemDBContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Item> Items { get; set; }
}