using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Rating).IsRequired();
        builder.Property(x => x.Comment).HasMaxLength(1000);
        builder.HasIndex(x => new { x.EventId, x.AttendeeId }).IsUnique();
        builder.HasOne(x => x.Attendee).WithMany(x => x.Reviews).HasForeignKey(x => x.AttendeeId).OnDelete(DeleteBehavior.Cascade);
    }
}
