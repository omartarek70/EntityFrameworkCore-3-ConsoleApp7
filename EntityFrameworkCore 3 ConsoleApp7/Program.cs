using Microsoft.EntityFrameworkCore;
using OnlineLearning.Data;
using OnlineLearning.Entities;

namespace OnlineLearning;

    internal static class Program
    {
        private static void Main()
        {
            using var context = new LearningContext();

            context.Database.Migrate();
            Console.WriteLine("Database is ready (migrations applied).");

            Header("CREATE");
            Run(context, "Add an instructor", () => AddInstructor(context, "Hassan Mahmoud", "Backend engineer and mentor."));
            Run(context, "Add a course (valid instructor + category)", () => AddCourse(context, 1, 3, "Minimal APIs in Practice", 350m, CourseLevel.Intermediate));
            Run(context, "Add a course (instructor 99 does not exist)", () => AddCourse(context, 99, 3, "Ghost Course", 100m, CourseLevel.Beginner));
            Run(context, "Add several students", () => AddStudents(context));
            Run(context, "Enroll student 3 in course 1", () => Enroll(context, 3, 1));
            Run(context, "Enroll student 1 in course 1 again", () => Enroll(context, 1, 1));
            Run(context, "Enroll student 1 in unpublished course 3", () => Enroll(context, 1, 3));
            Run(context, "Enroll student 99 in course 1", () => Enroll(context, 99, 1));

            Header("READ");
            Run(context, "Find course 1 / by title", () => FindCourse(context, 1, "C# Essentials"));
            Run(context, "Find course 999 / by title", () => FindCourse(context, 999, "Does Not Exist"));
            Run(context, "Courses priced between 250 and 600", () => FilterCourses(context, 250m, 600m));
            Run(context, "Sub-categories of 'Programming' + top-level categories", () => ShowCategories(context, 1));
            Run(context, "Roster of course 1", () => CourseRoster(context, 1));

            Header("UPDATE");
            Run(context, "Change the price of course 2", () => ChangePrice(context, 2, 749.00m));
            Run(context, "Update progress (student 1, course 1) -> 75", () => UpdateProgress(context, 1, 1, 75));
            Run(context, "Update progress -> 150 (invalid)", () => UpdateProgress(context, 1, 1, 150));
            Run(context, "Update progress -> 100 (completes the course)", () => UpdateProgress(context, 1, 1, 100, 88.5m));
            Run(context, "Publish course 3", () => PublishCourse(context, 3));

            Header("DELETE");
            Run(context, "Unenroll student 3 from course 1", () => Unenroll(context, 3, 1));
            Run(context, "Delete category 2 (has sub-categories) - unsafe", () => DeleteCategoryUnsafe(context, 2));
            Run(context, "Delete category 2 - safe version", () => DeleteCategorySafe(context, 2));
            Run(context, "Delete category 5 (no children) - safe version", () => DeleteCategorySafe(context, 5));
            Run(context, "Delete course 4 (has enrollments)", () => DeleteCourseWithEnrollments(context, 4));

            Console.WriteLine("\nAll operations finished.");
        }

        private static void Header(string text) =>
            Console.WriteLine($"\n==================== {text} ====================");

        private static void Run(LearningContext context, string title, Action operation)
        {
            Console.WriteLine($"\n--- {title} ---");
            try
            {
                operation();
            }
            catch (DbUpdateException ex)
            {
                context.ChangeTracker.Clear();
                Console.WriteLine($"Database refused the change: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                context.ChangeTracker.Clear();
                Console.WriteLine($"Something went wrong: {ex.Message}");
            }
        }

        private static string UniqueEmail(string prefix) =>
            $"{prefix}.{Guid.NewGuid().ToString("N")[..8]}@example.com";

        private static void AddInstructor(LearningContext context, string fullName, string? bio)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                Console.WriteLine("Instructor name is required.");
                return;
            }

            var instructor = new Instructor
            {
                FullName = fullName,
                Email = UniqueEmail("instructor"),
                Bio = bio,
                JoinedDate = DateTime.Today
            };

            context.Instructors.Add(instructor);
            context.SaveChanges();
            Console.WriteLine($"Instructor added. Generated Id = {instructor.Id}");
        }

        private static void AddCourse(LearningContext context, int instructorId, int categoryId,
                                      string title, decimal price, CourseLevel level)
        {
            if (string.IsNullOrWhiteSpace(title) || price < 0)
            {
                Console.WriteLine("Invalid input: title is required and price cannot be negative.");
                return;
            }

            if (context.Instructors.Find(instructorId) == null)
            {
                Console.WriteLine($"Instructor {instructorId} does not exist. Course was not added.");
                return;
            }

            if (context.Categories.Find(categoryId) == null)
            {
                Console.WriteLine($"Category {categoryId} does not exist. Course was not added.");
                return;
            }

            var course = new Course
            {
                Title = title,
                Price = price,
                Level = level,
                IsPublished = false,
                InstructorId = instructorId,
                CategoryId = categoryId
            };

            context.Courses.Add(course);
            context.SaveChanges();
            Console.WriteLine($"Course added. Generated Id = {course.Id}");
        }

        private static void AddStudents(LearningContext context)
        {
            var students = new List<Student>
        {
            new() { FullName = "Mariam Fathy",  Email = UniqueEmail("mariam"), DateOfBirth = new DateTime(2001, 3, 14) },
            new() { FullName = "Karim Osama",   Email = UniqueEmail("karim"),  DateOfBirth = new DateTime(1999, 8, 22) },
            new() { FullName = "Laila Mostafa", Email = UniqueEmail("laila"),  DateOfBirth = new DateTime(2003, 1, 9) }
        };

            context.Students.AddRange(students);
            context.SaveChanges();

            Console.WriteLine("Students added. Generated Ids: " + string.Join(", ", students.Select(s => s.Id)));
        }

        private static void Enroll(LearningContext context, int studentId, int courseId)
        {
            var student = context.Students.Find(studentId);
            if (student == null)
            {
                Console.WriteLine($"Enrollment refused: student {studentId} does not exist.");
                return;
            }

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Enrollment refused: course {courseId} does not exist.");
                return;
            }

            if (!course.IsPublished)
            {
                Console.WriteLine($"Enrollment refused: course '{course.Title}' is not published yet.");
                return;
            }

            if (context.Enrollments.Find(studentId, courseId) != null)
            {
                Console.WriteLine($"Enrollment refused: {student.FullName} is already enrolled in '{course.Title}'.");
                return;
            }

            context.Enrollments.Add(new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrolledDate = DateTime.UtcNow,
                Progress = 0,
                Status = EnrollmentStatus.Active
            });
            context.SaveChanges();
            Console.WriteLine($"{student.FullName} was enrolled in '{course.Title}'.");
        }

        private static void FindCourse(LearningContext context, int id, string title)
        {
            var byId = context.Courses.Find(id);
            Console.WriteLine(byId == null
                ? $"By Id ({id}): not found"
                : $"By Id ({id}): {byId.Title} - {byId.Level} - {byId.Price:F2}");

            var byTitle = context.Courses.FirstOrDefault(c => c.Title == title);
            Console.WriteLine(byTitle == null
                ? $"By title ('{title}'): not found"
                : $"By title ('{title}'): Id {byTitle.Id} - {byTitle.Level} - {byTitle.Price:F2}");
        }

        private static void FilterCourses(LearningContext context, decimal min, decimal max)
        {
            if (min > max)
            {
                Console.WriteLine("Invalid range: the minimum price is greater than the maximum.");
                return;
            }

            var courses = context.Courses
                .Where(c => c.Price >= min && c.Price <= max)
                .OrderByDescending(c => c.Price)
                .Select(c => new { c.Title, c.Level, c.Price })
                .ToList();

            if (courses.Count == 0)
            {
                Console.WriteLine("No courses in this price range.");
                return;
            }

            foreach (var c in courses)
                Console.WriteLine($"{c.Title} | {c.Level} | {c.Price:F2}");
        }

        private static void ShowCategories(LearningContext context, int parentId)
        {
            var parent = context.Categories.Find(parentId);
            if (parent == null)
            {
                Console.WriteLine($"Category {parentId} not found.");
                return;
            }

            Console.WriteLine($"Direct sub-categories of '{parent.Name}':");
            var subs = context.Categories.Where(c => c.ParentCategoryId == parentId).ToList();
            if (subs.Count == 0) Console.WriteLine("  (none)");
            foreach (var s in subs) Console.WriteLine($"  - {s.Name}");

            Console.WriteLine("Top-level categories:");
            foreach (var t in context.Categories.Where(c => c.ParentCategoryId == null).ToList())
                Console.WriteLine($"  - {t.Name}");
        }

        private static void CourseRoster(LearningContext context, int courseId)
        {
            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Course {courseId} not found.");
                return;
            }

            var studentIds = context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => e.StudentId)
                .ToList();

            Console.WriteLine($"'{course.Title}' has {studentIds.Count} enrolled student(s).");

            var students = context.Students
                .Where(s => studentIds.Contains(s.Id))
                .ToList();

            foreach (var s in students)
                Console.WriteLine($"  - {s.FullName}");
        }

        private static void ChangePrice(LearningContext context, int courseId, decimal newPrice)
        {
            if (newPrice < 0)
            {
                Console.WriteLine("Invalid price: it cannot be negative.");
                return;
            }

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Course {courseId} not found.");
                return;
            }

            var oldPrice = course.Price;
            course.Price = newPrice;
            context.SaveChanges();
            Console.WriteLine($"Price of '{course.Title}' changed from {oldPrice:F2} to {newPrice:F2}.");
        }

        private static void UpdateProgress(LearningContext context, int studentId, int courseId,
                                           int progress, decimal? finalGrade = null)
        {
            if (progress < 0 || progress > 100)
            {
                Console.WriteLine($"Rejected: progress must be between 0 and 100 (got {progress}).");
                return;
            }

            var enrollment = context.Enrollments.Find(studentId, courseId);
            if (enrollment == null)
            {
                Console.WriteLine($"No enrollment found for student {studentId} in course {courseId}.");
                return;
            }

            enrollment.Progress = progress;

            if (progress == 100)
            {
                enrollment.Status = EnrollmentStatus.Completed;
                enrollment.FinalGrade = finalGrade;
            }

            context.SaveChanges();
            Console.WriteLine($"Progress is now {progress}% (status: {enrollment.Status}" +
                              (enrollment.FinalGrade.HasValue ? $", final grade: {enrollment.FinalGrade:F2}" : "") + ").");
        }

        private static void PublishCourse(LearningContext context, int courseId)
        {
            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Course {courseId} not found.");
                return;
            }

            if (course.IsPublished)
            {
                Console.WriteLine($"'{course.Title}' is already published.");
                return;
            }

            course.IsPublished = true;
            course.PublishedDate = DateTime.UtcNow;
            context.SaveChanges();
            Console.WriteLine($"'{course.Title}' is now published (date: {course.PublishedDate:yyyy-MM-dd}).");
        }

        private static void Unenroll(LearningContext context, int studentId, int courseId)
        {
            var enrollment = context.Enrollments.Find(studentId, courseId);
            if (enrollment == null)
            {
                Console.WriteLine($"No enrollment found for student {studentId} in course {courseId}.");
                return;
            }

            context.Enrollments.Remove(enrollment);
            context.SaveChanges();
            Console.WriteLine("Enrollment removed.");
        }

        private static void DeleteCategoryUnsafe(LearningContext context, int categoryId)
        {
            var category = context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine($"Category {categoryId} not found.");
                return;
            }

            try
            {
                context.Categories.Remove(category);
                context.SaveChanges();
                Console.WriteLine("Category deleted.");
            }
            catch (DbUpdateException ex)
            {
                context.ChangeTracker.Clear();
                Console.WriteLine("DbUpdateException: " + (ex.InnerException?.Message ?? ex.Message));
            }
        }

        private static void DeleteCategorySafe(LearningContext context, int categoryId)
        {
            var category = context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine($"Category {categoryId} not found.");
                return;
            }

            if (context.Categories.Any(c => c.ParentCategoryId == categoryId))
            {
                Console.WriteLine($"Cannot delete '{category.Name}': it still has sub-categories.");
                return;
            }

            if (context.Courses.Any(c => c.CategoryId == categoryId))
            {
                Console.WriteLine($"Cannot delete '{category.Name}': it still has courses.");
                return;
            }

            context.Categories.Remove(category);
            context.SaveChanges();
            Console.WriteLine($"Category '{category.Name}' deleted.");
        }

        private static void DeleteCourseWithEnrollments(LearningContext context, int courseId)
        {
            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Course {courseId} not found.");
                return;
            }

            var before = context.Enrollments.Count(e => e.CourseId == courseId);
            Console.WriteLine($"Enrollments before delete: {before}");

            context.Courses.Remove(course);
            context.SaveChanges();

            var after = context.Enrollments.Count(e => e.CourseId == courseId);
            Console.WriteLine($"Course '{course.Title}' deleted. Enrollments after delete: {after}");
        }
    }

