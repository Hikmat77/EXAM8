using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class TicketTypeConfiguration : IEntityTypeConfiguration<TicketType>
{
    public void Configure(EntityTypeBuilder<TicketType> builder)
    {
        builder.ToTable("ticket_types", table =>
        {
            table.HasCheckConstraint("CK_ticket_types_quantity", "\"Quantity\" >= 0");
            table.HasCheckConstraint("CK_ticket_types_sold_count", "\"SoldCount\" <= \"Quantity\"");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Price).HasPrecision(10, 2);
        builder.Property(x => x.Version).IsRowVersion();
        builder.HasOne(x => x.Event).WithMany(x => x.TicketTypes).HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Cascade);
    }
}
