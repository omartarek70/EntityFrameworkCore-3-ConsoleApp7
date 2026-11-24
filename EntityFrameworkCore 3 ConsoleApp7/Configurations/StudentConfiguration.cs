using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearning.Data;
using OnlineLearning.Entities;

namespace OnlineLearning.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.FullName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Email).IsRequired().HasMaxLength(150);
        builder.Property(s => s.DateOfBirth).IsRequired().HasColumnType("date");

        builder.HasIndex(s => s.Email).IsUnique();

        if (SeedData.Enabled) builder.HasData(SeedData.Students);
    }
}
