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
    public class EnrollmentConfigurations : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasKey(E => new { E.StudentId, E.CourseId });

            builder.Property(E => E.EnrollmentDate)
                   .IsRequired();

            builder.Property(E => E.ProgressPercentage)
                   .IsRequired();

            builder.Property(E => E.Status)
                   .IsRequired();

            builder.Property(E => E.FinalGrade)
                   .IsRequired(false)
                   .HasPrecision(5, 2);

            builder.HasOne(E => E.Student)
                   .WithMany(S => S.Enrollments)
                   .HasForeignKey(E => E.StudentId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(E => E.Course)
                   .WithMany(C => C.Enrollments)
                   .HasForeignKey(E => E.CourseId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
