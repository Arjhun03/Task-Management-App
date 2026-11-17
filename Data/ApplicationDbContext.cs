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

    public DbSet<Users> Users => Set<Users>();

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================================
        // USERS
        // =========================================

        modelBuilder.Entity<Users>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(u => u.UserId);

            entity.Property(u => u.UserId)
                .ValueGeneratedOnAdd();

            entity.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(u => u.CreatedAt)
                .IsRequired();

            // Unique Email
            entity.HasIndex(u => u.Email)
                .IsUnique();
        });


        // =========================================
        // TASKS
        // =========================================

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Tasks");

            entity.HasKey(t => t.TaskId);

            entity.Property(t => t.TaskId)
                .ValueGeneratedOnAdd();

            entity.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(t => t.Description)
                .HasMaxLength(2000);

            entity.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(t => t.Priority)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(t => t.DueDate);

            entity.Property(t => t.CreatedAt)
                .IsRequired();

            entity.Property(t => t.UpdatedAt);


            // =====================================
            // TASK -> USERS
            // =====================================

            entity.HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // =====================================
            // TASK -> CATEGORY
            // =====================================

            entity.HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);


            // Indexes

            entity.HasIndex(t => t.UserId);

            entity.HasIndex(t => new
            {
                t.UserId,
                t.Status
            });

            entity.HasIndex(t => new
            {
                t.UserId,
                t.DueDate
            });
        });


        // =========================================
        // CATEGORIES
        // =========================================

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");

            entity.HasKey(c => c.CategoryId);

            entity.Property(c => c.CategoryId)
                .ValueGeneratedOnAdd();

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(60);

            entity.Property(c => c.Description)
                .HasMaxLength(250);

            entity.Property(c => c.CreatedAt)
                .IsRequired();


            // =====================================
            // CATEGORY -> USERS
            // =====================================

            // NoAction prevents SQL Server
            // multiple cascade path error.
            entity.HasOne(c => c.Users)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);


            // Unique category name per user

            entity.HasIndex(c => new
            {
                c.UserId,
                c.Name
            })
            .IsUnique();
        });
    }
}