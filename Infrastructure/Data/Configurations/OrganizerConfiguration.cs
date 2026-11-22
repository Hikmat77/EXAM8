using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OrganizerConfiguration : IEntityTypeConfiguration<Organizer>
{
    public void Configure(EntityTypeBuilder<Organizer> builder)
    {
        builder.ToTable("organizers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CompanyName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ContactEmail).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Phone);
        builder.HasMany(x => x.Events).WithOne(x => x.Organizer).HasForeignKey(x => x.OrganizerId).OnDelete(DeleteBehavior.Restrict);
    }
}
