
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {        
    }

    public DbSet<Question> Questions {get; set;}
    public DbSet<Answer> Answers {get; set;}
}