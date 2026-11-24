using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OnlineLearning.Data;

#nullable disable

namespace OnlineLearning.Migrations
{
    [DbContext(typeof(LearningContext))]
    [Migration("20261124120100_SeedData")]
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Instructors",
                columns: new[] { "Id", "Bio", "Email", "FullName", "JoinedDate" },
                values: new object[,]
                {
                    { 1, "Senior .NET developer and trainer.", "ahmed.samir@example.com", "Ahmed Samir", new DateTime(2022, 1, 15) },
                    { 2, null, "mona.khaled@example.com", "Mona Khaled", new DateTime(2023, 6, 1) }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "DateOfBirth", "Email", "FullName" },
                values: new object[,]
                {
                    { 1, new DateTime(2001, 5, 12), "sara.ali@example.com", "Sara Ali" },
                    { 2, new DateTime(2000, 11, 3), "youssef.adel@example.com", "Youssef Adel" },
                    { 3, new DateTime(2002, 2, 28), "nour.hassan@example.com", "Nour Hassan" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name", "ParentCategoryId" },
                values: new object[,]
                {
                    { 1, "All programming courses", "Programming", null },
                    { 4, "UI / UX design", "Design", null },
                    { 5, "Unused category", "Archive", null }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name", "ParentCategoryId" },
                values: new object[] { 2, "Server-side development", "Backend", 1 });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name", "ParentCategoryId" },
                values: new object[] { 3, null, "ASP.NET", 2 });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "CategoryId", "Description", "InstructorId", "IsPublished", "Level", "Price", "PublishedDate", "Title" },
                values: new object[,]
                {
                    { 1, 3, "Build web apps with ASP.NET Core.", 1, true, "Beginner", 499.99m, new DateTime(2024, 1, 10), "ASP.NET Core Fundamentals" },
                    { 2, 2, "Deep dive into EF Core.", 1, true, "Advanced", 799.00m, new DateTime(2024, 3, 5), "Advanced EF Core" },
                    { 3, 4, null, 2, false, "Beginner", 250.00m, null, "UI Basics" },
                    { 4, 1, "Learn C# from scratch.", 2, true, "Intermediate", 299.50m, new DateTime(2024, 2, 20), "C# Essentials" }
                });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "CourseId", "StudentId", "EnrolledDate", "FinalGrade", "Progress", "Status" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 4, 1), null, 40, "Active" },
                    { 1, 2, new DateTime(2024, 4, 2), 92.50m, 100, "Completed" },
                    { 4, 1, new DateTime(2024, 4, 5), null, 10, "Active" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumns: new[] { "CourseId", "StudentId" },
                keyValues: new object[,] { { 1, 1 }, { 1, 2 }, { 4, 1 } });

            migrationBuilder.DeleteData(table: "Courses", keyColumn: "Id", keyValues: new object[] { 1, 2, 3, 4 });
            migrationBuilder.DeleteData(table: "Students", keyColumn: "Id", keyValues: new object[] { 1, 2, 3 });
            migrationBuilder.DeleteData(table: "Categories", keyColumn: "Id", keyValue: 3);
            migrationBuilder.DeleteData(table: "Categories", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "Categories", keyColumn: "Id", keyValues: new object[] { 1, 4, 5 });
            migrationBuilder.DeleteData(table: "Instructors", keyColumn: "Id", keyValues: new object[] { 1, 2 });
        }
    }
}
