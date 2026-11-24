using OnlineLearning.Entities;

namespace OnlineLearning.Data;

public static class SeedData
{
    public static bool Enabled = true;

    public static readonly Instructor[] Instructors =
    {
        new() { Id = 1, FullName = "Ahmed Samir", Email = "ahmed.samir@example.com", Bio = "Senior .NET developer and trainer.", JoinedDate = new DateTime(2022, 1, 15) },
        new() { Id = 2, FullName = "Mona Khaled",  Email = "mona.khaled@example.com",  Bio = null,                               JoinedDate = new DateTime(2023, 6, 1) },
    };

    public static readonly Category[] Categories =
    {
        new() { Id = 1, Name = "Programming", Description = "All programming courses", ParentCategoryId = null },
        new() { Id = 2, Name = "Backend",     Description = "Server-side development", ParentCategoryId = 1 },
        new() { Id = 3, Name = "ASP.NET",     Description = null,                      ParentCategoryId = 2 },
        new() { Id = 4, Name = "Design",      Description = "UI / UX design",          ParentCategoryId = null },
        new() { Id = 5, Name = "Archive",     Description = "Unused category",         ParentCategoryId = null },
    };

    public static readonly Course[] Courses =
    {
        new() { Id = 1, Title = "ASP.NET Core Fundamentals", Description = "Build web apps with ASP.NET Core.", Price = 499.99m, Level = CourseLevel.Beginner,     IsPublished = true,  PublishedDate = new DateTime(2024, 1, 10), InstructorId = 1, CategoryId = 3 },
        new() { Id = 2, Title = "Advanced EF Core",          Description = "Deep dive into EF Core.",         Price = 799.00m, Level = CourseLevel.Advanced,     IsPublished = true,  PublishedDate = new DateTime(2024, 3, 5),  InstructorId = 1, CategoryId = 2 },
        new() { Id = 3, Title = "UI Basics",                 Description = null,                              Price = 250.00m, Level = CourseLevel.Beginner,     IsPublished = false, PublishedDate = null,                      InstructorId = 2, CategoryId = 4 },
        new() { Id = 4, Title = "C# Essentials",             Description = "Learn C# from scratch.",          Price = 299.50m, Level = CourseLevel.Intermediate, IsPublished = true,  PublishedDate = new DateTime(2024, 2, 20), InstructorId = 2, CategoryId = 1 },
    };

    public static readonly Student[] Students =
    {
        new() { Id = 1, FullName = "Sara Ali",     Email = "sara.ali@example.com",     DateOfBirth = new DateTime(2001, 5, 12) },
        new() { Id = 2, FullName = "Youssef Adel", Email = "youssef.adel@example.com", DateOfBirth = new DateTime(2000, 11, 3) },
        new() { Id = 3, FullName = "Nour Hassan",  Email = "nour.hassan@example.com",  DateOfBirth = new DateTime(2002, 2, 28) },
    };

    public static readonly Enrollment[] Enrollments =
    {
        new() { StudentId = 1, CourseId = 1, EnrolledDate = new DateTime(2024, 4, 1), Progress = 40,  Status = EnrollmentStatus.Active,    FinalGrade = null },
        new() { StudentId = 2, CourseId = 1, EnrolledDate = new DateTime(2024, 4, 2), Progress = 100, Status = EnrollmentStatus.Completed, FinalGrade = 92.50m },
        new() { StudentId = 1, CourseId = 4, EnrolledDate = new DateTime(2024, 4, 5), Progress = 10,  Status = EnrollmentStatus.Active,    FinalGrade = null },
    };
}
