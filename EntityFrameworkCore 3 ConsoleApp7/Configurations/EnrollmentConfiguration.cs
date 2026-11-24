using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearning.Data;
using OnlineLearning.Entities;

namespace OnlineLearning.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments", t =>
            t.HasCheckConstraint("CK_Enrollments_Progress", "[Progress] >= 0 AND [Progress] <= 100"));

        builder.HasKey(e => new { e.StudentId, e.CourseId });

        builder.Property(e => e.EnrolledDate).IsRequired();
        builder.Property(e => e.Progress).IsRequired();
        builder.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.FinalGrade).HasColumnType("decimal(5,2)");

        builder.HasOne(e => e.Student)
               .WithMany(s => s.Enrollments)
               .HasForeignKey(e => e.StudentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Course)
               .WithMany(c => c.Enrollments)
               .HasForeignKey(e => e.CourseId)
               .OnDelete(DeleteBehavior.Cascade);

        if (SeedData.Enabled) builder.HasData(SeedData.Enrollments);
    }
}
