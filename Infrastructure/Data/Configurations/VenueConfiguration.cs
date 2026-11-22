using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("venues");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
        builder.Property(x => x.Address).HasColumnName("address").IsRequired();
        builder.Property(x => x.City).HasColumnName("city").IsRequired();
        builder.Property(x => x.Capacity).HasColumnName("capacity");
        builder.HasMany(x => x.Events).WithOne(x => x.Venue).HasForeignKey(x => x.VenueId).OnDelete(DeleteBehavior.Restrict);
        builder.HasData(
            new Venue { Id = 1, Name = "Palace of Culture", Address = "Somoni 1", City = "Dushanbe", Capacity = 1500 },
            new Venue { Id = 2, Name = "Vahdat Centre", Address = "Rudaki 20", City = "Khujand", Capacity = 800 });
    }
}
