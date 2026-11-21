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
    public class InstructorConfigurations : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
          
            builder.HasKey(I => I.Id);

            builder.Property(I => I.FullName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(I => I.Email)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.HasIndex(I => I.Email)
                   .IsUnique();

            builder.Property(I => I.Bio)
                   .IsRequired(false)
                   .HasMaxLength(500);

            builder.Property(I => I.JoinedDate)
                   .IsRequired();
        }
    }
}
