using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearning.Data;
using OnlineLearning.Entities;

namespace OnlineLearning.Configurations;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.ToTable("Instructors");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.FullName).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Email).IsRequired().HasMaxLength(150);
        builder.Property(i => i.Bio).HasMaxLength(500);
        builder.Property(i => i.JoinedDate).IsRequired().HasColumnType("date");

        builder.HasIndex(i => i.Email).IsUnique();

        if (SeedData.Enabled) builder.HasData(SeedData.Instructors);
    }
}
