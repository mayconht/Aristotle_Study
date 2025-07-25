using Aristotle.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aristotle.Infrastructure;

/// <summary>
/// 
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    //Any domain or entity that you want to persist in the database should be added here
    //This is the DbSet for the User entity, it literally means "table" in the database
    //So as a poor implementation this should not be here, should be in a interface or a repository
    //But for the sake of simplicity, we will keep it here
    /// <summary>
    /// 
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Here I am going agains the DRY principle, just to show how we can configure the User entity here as well
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100); 
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200); 
            entity.Property(u => u.DateOfBirth).IsRequired(false); 
            entity.HasIndex(u => u.Email).IsUnique(); 
        });
    }
}