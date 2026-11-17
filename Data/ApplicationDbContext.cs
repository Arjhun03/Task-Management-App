using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Users> Users { get; set; }

    public DbSet<TaskItem> Tasks { get; set; }

    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<Users>()
            .HasKey(u => u.UserId);

        modelBuilder.Entity<Users>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // User -> Tasks
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Users)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> Categories
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Users)
            .WithMany(u => u.Categories)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Category -> Tasks
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Tasks)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Category name unique per user
        modelBuilder.Entity<Category>()
            .HasIndex(c => new { c.UserId, c.Name })
            .IsUnique();
    }
}