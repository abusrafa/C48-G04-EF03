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
    public class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {

            builder.HasKey(C => C.Id);


            builder.Property(C => C.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(C => C.Description)
                   .IsRequired(false)
                   .HasMaxLength(300);

            builder.HasOne(C => C.ParentCategory)
                   .WithMany(C => C.SubCategories)
                   .HasForeignKey(C => C.ParentCategoryId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
