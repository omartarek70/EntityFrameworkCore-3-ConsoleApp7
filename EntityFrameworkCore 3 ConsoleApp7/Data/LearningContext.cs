using Microsoft.EntityFrameworkCore;
using OnlineLearning.Entities;

namespace OnlineLearning.Data;

public class LearningContext : DbContext
{
    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=(localdb)\MSSQLLocalDB;Database=OnlineLearningDb;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LearningContext).Assembly);
    }
}
