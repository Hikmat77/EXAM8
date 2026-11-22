using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("tickets");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Price).HasPrecision(10, 2);
        builder.HasOne(x => x.TicketType).WithMany(x => x.Tickets).HasForeignKey(x => x.TicketTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Attendee).WithMany(x => x.Tickets).HasForeignKey(x => x.AttendeeId).OnDelete(DeleteBehavior.Restrict);
    }
}
