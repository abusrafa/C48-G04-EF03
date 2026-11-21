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
    public class StudentConfigurations : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(S => S.Id);

            builder.Property(S => S.FullName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(S => S.Email)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.HasIndex(S => S.Email)
                   .IsUnique();

            builder.Property(S => S.DateOfBirth)
                   .IsRequired();
        }
    }
}
