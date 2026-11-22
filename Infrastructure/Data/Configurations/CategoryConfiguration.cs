using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasData(
            new Category { Id = 1, Name = "Music" },
            new Category { Id = 2, Name = "Conference" },
            new Category { Id = 3, Name = "Sports" },
            new Category { Id = 4, Name = "Art" },
            new Category { Id = 5, Name = "Education" });
    }
}
