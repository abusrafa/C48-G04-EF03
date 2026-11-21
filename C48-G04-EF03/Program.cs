using C48_G04_EF03.Models;
using Microsoft.EntityFrameworkCore;

namespace C48_G04_EF03
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            APPDbContext dbContext = new APPDbContext();

            #region AddInstructor
            var newInstructorId = await AddInstructorAsync(dbContext, "Khaled Mansour", "khaled@platform.com", "Full-Stack Developer");
            #endregion

            #region Add a course

            await AddCourseAsync(dbContext, "Invalid Course", null, 50m, CourseLevel.Beginner, 999, 1);

            int newCourseId = await AddCourseAsync(dbContext, "Clean Architecture in .NET", "Build maintainable apps", 79.99m, CourseLevel.Advanced, newInstructorId, 3);

            #endregion

            #region Add several students

            List<int> newStudentIds = await AddThreeStudentsAsync(dbContext);

            #endregion

            #region Enroll a student

            await EnrollStudentAsync(dbContext, 999, 1);
            await EnrollStudentAsync(dbContext, 3, 3);
            await EnrollStudentAsync(dbContext, 1, 1);
            await EnrollStudentAsync(dbContext, 3, 1);

            #endregion

            #region Find a course

            await FindCourseAsync(dbContext, 1, "C# Fundamentals");
            await FindCourseAsync(dbContext, 999, "Unknown Course");

            #endregion

            #region Filter courses

            await FilterCoursesByPriceRangeAsync(dbContext, 40m, 90m);

            #endregion

            #region Sub-categories

            await GetSubAndTopCategoriesAsync(dbContext, 1);

            #endregion

            #region Course roster

            await GetCourseRosterAsync(dbContext, 1);
            #endregion

            #region Change a price

            await ChangeCoursePriceAsync(dbContext, 999, 55m);
            await ChangeCoursePriceAsync(dbContext, 1, 59.99m);

            #endregion

            #region Update progress

            await UpdateEnrollmentProgressAsync(dbContext, 1, 1, 150);
            await UpdateEnrollmentProgressAsync(dbContext, 1, 1, 60);
            await UpdateEnrollmentProgressAsync(dbContext, 1, 1, 100);

            #endregion

            #region  Publish a course

            await PublishCourseAsync(dbContext, 999);
            await PublishCourseAsync(dbContext, 1);
            await PublishCourseAsync(dbContext, 3);

            #endregion

            #region Unenroll 

            await UnenrollStudentAsync(dbContext, 999, 1);
            await UnenrollStudentAsync(dbContext, 2, 1);

            #endregion

            #region Delete a category

            await TryDeleteCategoryUnsafeAsync(dbContext, 1);
            await DeleteCategorySafelyAsync(dbContext, 1);
            await DeleteCategorySafelyAsync(dbContext, 4);

            #endregion

            #region Delete a course

            await DeleteCourseWithEnrollmentsAsync(dbContext, 1);

            #endregion


        }

        static async Task<int> AddInstructorAsync(APPDbContext dbContext, string fullName, string email, string? bio)
        {
            Instructor instructor = new Instructor()
            {
                FullName = fullName,
                Email = email,
                Bio = bio,
                JoinedDate = DateTime.Now
            };

            dbContext.Instructors.Add(instructor);
            await dbContext.SaveChangesAsync();

            Console.WriteLine($"After Save Changes: Instructor '{instructor.FullName}' Added with Id = {instructor.Id}");
            return instructor.Id;
        }

        static async Task<int> AddCourseAsync(APPDbContext dbContext, string title, string? description, decimal price, CourseLevel level, int instructorId, int categoryId)
        {
     
            var instructor = await dbContext.Instructors.AsNoTracking().FirstOrDefaultAsync(I => I.Id == instructorId);
            if (instructor is null)
            {
                Console.WriteLine($"[AddCourse] Refused: Instructor with Id = {instructorId} does not exist.");
                return -1;
            }

            var category = await dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(C => C.Id == categoryId);
            if (category is null)
            {
                Console.WriteLine($"[AddCourse] Refused: Category with Id = {categoryId} does not exist.");
                return -1;
            }

      
            Course course = new Course()
            {
                Title = title,
                Description = description,
                Price = price,
                Level = level,
                IsPublished = false,
                PublishedDate = null,
                InstructorId = instructorId,
                CategoryId = categoryId
            };


            dbContext.Courses.Add(course);
            await dbContext.SaveChangesAsync();

            Console.WriteLine($"After Save Changes: Course '{course.Title}' Added with Id = {course.Id}");
            return course.Id;
        }

        static async Task<List<int>> AddThreeStudentsAsync(APPDbContext dbContext)
        {
            List<Student> students = new List<Student>()
    {
        new Student() { FullName = "Youssef Tarek", Email = "youssef@student.com", DateOfBirth = new DateOnly(2002, 2, 14) },
        new Student() { FullName = "Nourhan Sami", Email = "nourhan@student.com", DateOfBirth = new DateOnly(2001, 11, 3) },
        new Student() { FullName = "Ziad Mahmoud", Email = "ziad@student.com", DateOfBirth = new DateOnly(2003, 7, 19) }
    };

            dbContext.Students.AddRange(students);
            await dbContext.SaveChangesAsync();

            List<int> ids = students.Select(S => S.Id).ToList();
            Console.WriteLine($"After Save Changes: 3 Students Added with Ids = {string.Join(", ", ids)}");
            return ids;
        }

        static async Task EnrollStudentAsync(APPDbContext dbContext, int studentId, int courseId)
        {
           
            var student = await dbContext.Students.AsNoTracking().FirstOrDefaultAsync(S => S.Id == studentId);
            if (student is null)
            {
                Console.WriteLine($"[EnrollStudent] Refused: Student with Id = {studentId} does not exist.");
                return;
            }

            var course = await dbContext.Courses.AsNoTracking().FirstOrDefaultAsync(C => C.Id == courseId);
            if (course is null)
            {
                Console.WriteLine($"[EnrollStudent] Refused: Course with Id = {courseId} does not exist.");
                return;
            }

            if (!course.IsPublished)
            {
                Console.WriteLine($"[EnrollStudent] Refused: Course '{course.Title}' is not published yet.");
                return;
            }

            var existingEnrollment = await dbContext.Enrollments
                .AsNoTracking()
                .FirstOrDefaultAsync(E => E.StudentId == studentId && E.CourseId == courseId);

            if (existingEnrollment is not null)
            {
                Console.WriteLine($"[EnrollStudent] Refused: Student '{student.FullName}' is already enrolled in '{course.Title}'.");
                return;
            }

            Enrollment enrollment = new Enrollment()
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                ProgressPercentage = 0,
                Status = EnrollmentStatus.Active,
                FinalGrade = null
            };

            dbContext.Enrollments.Add(enrollment);
            await dbContext.SaveChangesAsync();

            Console.WriteLine($"After Save Changes: Student '{student.FullName}' enrolled in '{course.Title}'.");
        }

        static async Task FindCourseAsync(APPDbContext dbContext, int courseId, string courseTitle)
        {
            
            var courseById = await dbContext.Courses.FindAsync(courseId);
            if (courseById is not null)
            {
                Console.WriteLine($"[Find by Id={courseId}] {courseById.Id} :: {courseById.Title}");
            }
            else
            {
                Console.WriteLine($"[Find by Id={courseId}] not found");
            }

          
            var courseByTitle = await dbContext.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(C => C.Title == courseTitle);

            if (courseByTitle is not null)
            {
                Console.WriteLine($"[FirstOrDefault by Title='{courseTitle}'] {courseByTitle.Id} :: {courseByTitle.Title}");
            }
            else
            {
                Console.WriteLine($"[FirstOrDefault by Title='{courseTitle}'] not found");
            }
        }

        static async Task FilterCoursesByPriceRangeAsync(APPDbContext dbContext, decimal minPrice, decimal maxPrice)
        {
            var courses = await dbContext.Courses
                .AsNoTracking()
                .Where(C => C.Price >= minPrice && C.Price <= maxPrice)
                .OrderByDescending(C => C.Price)
                .Select(C => new
                {
                    C.Title,
                    C.Level,
                    C.Price
                })
                .ToListAsync();

            Console.WriteLine($"--- Courses Priced Between {minPrice} and {maxPrice} (Descending) ---");
            if (courses.Count == 0)
            {
                Console.WriteLine("No courses found in this price range.");
                return;
            }

            foreach (var course in courses)
            {
                Console.WriteLine($"Title: {course.Title} :: Level: {course.Level} :: Price: {course.Price}");
            }
        }

        static async Task GetSubAndTopCategoriesAsync(APPDbContext dbContext, int parentCategoryId)
        {
            // 1. التأكد من وجود التصنيف الأب أولاً
            var parentCategory = await dbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(C => C.Id == parentCategoryId);

            if (parentCategory is null)
            {
                Console.WriteLine($"[SubCategories] Category with Id = {parentCategoryId} not found.");
                return;
            }

            var subCategories = await dbContext.Categories
                .AsNoTracking()
                .Where(C => C.ParentCategoryId == parentCategoryId)
                .ToListAsync();

            Console.WriteLine($"--- Direct Sub-Categories of '{parentCategory.Name}' (Id={parentCategoryId}) ---");
            if (subCategories.Count == 0)
            {
                Console.WriteLine("No direct sub-categories found.");
            }
            else
            {
                foreach (var sub in subCategories)
                {
                    Console.WriteLine($"{sub.Id} :: {sub.Name}");
                }
            }

            var topCategories = await dbContext.Categories
                .AsNoTracking()
                .Where(C => C.ParentCategoryId == null)
                .ToListAsync();

            Console.WriteLine("--- Top-Level Categories ---");
            foreach (var top in topCategories)
            {
                Console.WriteLine($"{top.Id} :: {top.Name}");
            }
        }

        static async Task GetCourseRosterAsync(APPDbContext dbContext, int courseId)
        {
          
            var course = await dbContext.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(C => C.Id == courseId);

            if (course is null)
            {
                Console.WriteLine($"[CourseRoster] Course with Id = {courseId} not found.");
                return;
            }

       
            List<int> studentIds = await dbContext.Enrollments
                .AsNoTracking()
                .Where(E => E.CourseId == courseId)
                .Select(E => E.StudentId)
                .ToListAsync();

            Console.WriteLine($"--- Course Roster for '{course.Title}' (Id={courseId}) ---");
            Console.WriteLine($"Total Enrolled Students: {studentIds.Count}");

            if (studentIds.Count == 0)
            {
                Console.WriteLine("No students enrolled in this course.");
                return;
            }

     
            var studentNames = await dbContext.Students
                .AsNoTracking()
                .Where(S => studentIds.Contains(S.Id))
                .Select(S => S.FullName)
                .ToListAsync();

            foreach (var name in studentNames)
            {
                Console.WriteLine($"- {name}");
            }
        }

        static async Task ChangeCoursePriceAsync(APPDbContext dbContext, int courseId, decimal newPrice)
        {
   
            if (newPrice < 0)
            {
                Console.WriteLine($"[ChangePrice] Refused: Price ({newPrice}) cannot be negative.");
                return;
            }

            var course = await dbContext.Courses.FindAsync(courseId);
            if (course is null)
            {
                Console.WriteLine($"[ChangePrice] Refused: Course with Id = {courseId} not found.");
                return;
            }

            decimal oldPrice = course.Price;

            course.Price = newPrice;
            await dbContext.SaveChangesAsync();

            Console.WriteLine($"After Save Changes: Course '{course.Title}' price changed from {oldPrice} to {course.Price}.");
        }

        static async Task UpdateEnrollmentProgressAsync(APPDbContext dbContext, int studentId, int courseId, int newProgress)
        {
    
            if (newProgress < 0 || newProgress > 100)
            {
                Console.WriteLine($"[UpdateProgress] Refused: Progress ({newProgress}) must be between 0 and 100.");
                return;
            }

            var enrollment = await dbContext.Enrollments.FindAsync(studentId, courseId);
            if (enrollment is null)
            {
                Console.WriteLine($"[UpdateProgress] Refused: Enrollment for StudentId = {studentId} in CourseId = {courseId} not found.");
                return;
            }

            enrollment.ProgressPercentage = newProgress;

            if (newProgress == 100)
            {
                enrollment.Status = EnrollmentStatus.Completed;
            }

            await dbContext.SaveChangesAsync();

            Console.WriteLine($"After Save Changes: StudentId={studentId}, CourseId={courseId} :: Progress={enrollment.ProgressPercentage}% :: Status={enrollment.Status}");
        }

        static async Task PublishCourseAsync(APPDbContext dbContext, int courseId)
        {
       
            var course = await dbContext.Courses.FindAsync(courseId);

   
            if (course is null)
            {
                Console.WriteLine($"[PublishCourse] Refused: Course with Id = {courseId} not found.");
                return;
            }

            if (course.IsPublished)
            {
                Console.WriteLine($"[PublishCourse] Refused: Course '{course.Title}' is already published on {course.PublishedDate:yyyy-MM-dd}.");
                return;
            }


            course.IsPublished = true;
            course.PublishedDate = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            Console.WriteLine($"After Save Changes: Course '{course.Title}' is now published on {course.PublishedDate:yyyy-MM-dd}.");
        }


        static async Task UnenrollStudentAsync(APPDbContext dbContext, int studentId, int courseId)
        {
            
            var enrollment = await dbContext.Enrollments.FindAsync(studentId, courseId);

            
            if (enrollment is null)
            {
                Console.WriteLine($"[Unenroll] Refused: Enrollment for StudentId = {studentId} in CourseId = {courseId} not found.");
                return;
            }

           
            dbContext.Enrollments.Remove(enrollment);
            await dbContext.SaveChangesAsync();

            Console.WriteLine($"After Save Changes: StudentId = {studentId} unenrolled from CourseId = {courseId}.");
        }


      
        static async Task TryDeleteCategoryUnsafeAsync(APPDbContext dbContext, int categoryId)
        {
            var category = await dbContext.Categories.FindAsync(categoryId);
            if (category is null)
            {
                Console.WriteLine($"[TryDeleteCategory] Category with Id = {categoryId} not found.");
                return;
            }

            try
            {
                dbContext.Categories.Remove(category);
                await dbContext.SaveChangesAsync();
                Console.WriteLine($"After Save Changes: Category '{category.Name}' deleted.");
            }
            catch (DbUpdateException ex)
            {
                // إرجاع حالة الكائن في الـ ChangeTracker إلى Unchanged حتى لا يُعاد إرسال أمر الحذف في SaveChanges اللاحقة
                dbContext.Entry(category).State = EntityState.Unchanged;

                Console.WriteLine($"[TryDeleteCategory] Caught DbUpdateException for '{category.Name}':");
                Console.WriteLine($"Message: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

    
        static async Task DeleteCategorySafelyAsync(APPDbContext dbContext, int categoryId)
        {
            var category = await dbContext.Categories.FindAsync(categoryId);
            if (category is null)
            {
                Console.WriteLine($"[DeleteCategorySafely] Refused: Category with Id = {categoryId} not found.");
                return;
            }

          
            bool hasSubCategories = await dbContext.Categories.AnyAsync(C => C.ParentCategoryId == categoryId);
            if (hasSubCategories)
            {
                Console.WriteLine($"[DeleteCategorySafely] Refused: Category '{category.Name}' still has sub-categories and cannot be deleted.");
                return;
            }

        
            bool hasCourses = await dbContext.Courses.AnyAsync(C => C.CategoryId == categoryId);
            if (hasCourses)
            {
                Console.WriteLine($"[DeleteCategorySafely] Refused: Category '{category.Name}' still has courses and cannot be deleted.");
                return;
            }

            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();

            Console.WriteLine($"After Save Changes: Category '{category.Name}' (Id = {categoryId}) deleted safely.");
        }

        static async Task DeleteCourseWithEnrollmentsAsync(APPDbContext dbContext, int courseId)
        {
       
            var course = await dbContext.Courses.FindAsync(courseId);
            if (course is null)
            {
                Console.WriteLine($"[DeleteCourse] Refused: Course with Id = {courseId} not found.");
                return;
            }

            int enrollmentsBefore = await dbContext.Enrollments.CountAsync(E => E.CourseId == courseId);
            Console.WriteLine($"[DeleteCourse] Course '{course.Title}' (Id = {courseId}) -> Enrollments count BEFORE delete: {enrollmentsBefore}");

        
            dbContext.Courses.Remove(course);
            await dbContext.SaveChangesAsync();

            int enrollmentsAfter = await dbContext.Enrollments.CountAsync(E => E.CourseId == courseId);
            Console.WriteLine($"After Save Changes: Course '{course.Title}' deleted -> Enrollments count AFTER delete: {enrollmentsAfter}");

            /*
  * Why were the enrollment rows removed automatically?
  * Because the relationship between Course (principal) and Enrollment (dependent) uses
  * DeleteBehavior.Cascade (the default delete behavior for required foreign keys in EF Core,
  * which generates ON DELETE CASCADE in SQL Server). When the parent Course row is deleted,
  * SQL Server automatically cascades the delete to all dependent Enrollment rows referencing that CourseId.
  */

        }

    }

}
