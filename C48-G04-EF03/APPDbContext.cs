using C48_G04_EF03.Configurations;
using C48_G04_EF03.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace C48_G04_EF03
{
    internal class APPDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=.;Database=OnlineLearningPlatformDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new InstructorConfigurations());
            modelBuilder.ApplyConfiguration(new CategoryConfigurations());
            modelBuilder.ApplyConfiguration(new CourseConfigurations());
            modelBuilder.ApplyConfiguration(new StudentConfigurations());
            modelBuilder.ApplyConfiguration(new EnrollmentConfigurations());

            //Seeding data
            modelBuilder.Entity<Instructor>().HasData(
                new Instructor { Id = 1, FullName = "Dr. Ahmed Hassan", Email = "ahmed@platform.com", Bio = "Senior Software Architect", JoinedDate = new DateTime(2025, 1, 10) },
                new Instructor { Id = 2, FullName = "Sara Ali", Email = "sara@platform.com", Bio = "Database & Cloud Expert", JoinedDate = new DateTime(2025, 3, 15) }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Programming", Description = "Software development courses", ParentCategoryId = null },
                new Category { Id = 2, Name = "Backend", Description = "Server-side technologies", ParentCategoryId = 1 },
                new Category { Id = 3, Name = "ASP.NET", Description = "Microsoft web framework", ParentCategoryId = 2 },
                new Category { Id = 4, Name = "Data Science", Description = "AI and Data Analysis", ParentCategoryId = null }
            );

            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Title = "EF Core Mastery", Description = "Deep dive into EF Core", Price = 49.99m, Level = CourseLevel.Intermediate, IsPublished = true, PublishedDate = new DateTime(2025, 2, 1), InstructorId = 1, CategoryId = 3 },
                new Course { Id = 2, Title = "C# Fundamentals", Description = "Start coding in C#", Price = 29.99m, Level = CourseLevel.Beginner, IsPublished = true, PublishedDate = new DateTime(2025, 2, 15), InstructorId = 1, CategoryId = 1 },
                new Course { Id = 3, Title = "Microservices Architecture", Description = "Advanced distributed systems", Price = 99.99m, Level = CourseLevel.Advanced, IsPublished = false, PublishedDate = null, InstructorId = 2, CategoryId = 2 }
            );

            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, FullName = "Omar Khaled", Email = "omar@student.com", DateOfBirth = new DateOnly(2000, 5, 12) },
                new Student { Id = 2, FullName = "Mona Adel", Email = "mona@student.com", DateOfBirth = new DateOnly(2001, 8, 22) }
            );

            modelBuilder.Entity<Enrollment>().HasData(
                new Enrollment { StudentId = 1, CourseId = 1, EnrollmentDate = new DateTime(2025, 3, 1), ProgressPercentage = 40, Status = EnrollmentStatus.Active, FinalGrade = null },
                new Enrollment { StudentId = 2, CourseId = 1, EnrollmentDate = new DateTime(2025, 3, 2), ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 95.50m }
            );
        }

    }

}
