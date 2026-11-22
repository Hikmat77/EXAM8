using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AttendeeProfileConfiguration : IEntityTypeConfiguration<AttendeeProfile>
{
    public void Configure(EntityTypeBuilder<AttendeeProfile> builder)
    {
        builder.ToTable("attendee_profiles");
        builder.HasKey(x => x.AttendeeId);
        builder.Property(x => x.Bio).HasMaxLength(1000);
        builder.HasOne(x => x.Attendee).WithOne(x => x.Profile).HasForeignKey<AttendeeProfile>(x => x.AttendeeId).OnDelete(DeleteBehavior.Cascade);
    }
}
