using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events", table => table.HasCheckConstraint("CK_events_end_after_start", "\"EndDate\" > \"StartDate\""));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.Status).HasConversion<string>().IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasIndex(x => new { x.Status, x.StartDate });
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasMany(x => x.Categories)
            .WithMany(x => x.Events)
            .UsingEntity("EventCategories");
        builder.HasMany(x => x.TicketTypes).WithOne(x => x.Event).HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Reviews).WithOne(x => x.Event).HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Cascade);
    }
}
