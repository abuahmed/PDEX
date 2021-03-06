﻿using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class CategoryMap : EntityTypeConfiguration<CategoryDTO>
    {
        public CategoryMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.DisplayName)
               .IsRequired();

            Property(t => t.NameType)
                .IsRequired();

            // Table & Column Mappings
            ToTable("Categories");
            Property(t => t.Id).HasColumnName("Id");
        }
    }
}
