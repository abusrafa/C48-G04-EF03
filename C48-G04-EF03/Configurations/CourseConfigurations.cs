using C48_G04_EF03.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C48_G04_EF03.Configurations
{
    public class CourseConfigurations : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
  
            builder.HasKey(C => C.Id);

            builder.Property(C => C.Title)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(C => C.Description)
                   .IsRequired(false)
                   .HasMaxLength(1000);

            builder.Property(C => C.Price)
                   .IsRequired()
                   .HasPrecision(10, 2);

            builder.Property(C => C.Level)
                   .IsRequired();

            builder.Property(C => C.IsPublished)
                   .IsRequired();

            builder.Property(C => C.PublishedDate)
                   .IsRequired(false);

            builder.HasOne(C => C.Instructor)
                   .WithMany(I => I.Courses)
                   .HasForeignKey(C => C.InstructorId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(C => C.Category)
                   .WithMany(Cat => Cat.Courses)
                   .HasForeignKey(C => C.CategoryId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
