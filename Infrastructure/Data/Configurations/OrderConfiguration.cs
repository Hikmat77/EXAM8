using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<string>().IsRequired();
        builder.Property(x => x.TotalAmount).HasPrecision(12, 2);
        builder.HasOne(x => x.Attendee).WithMany(x => x.Orders).HasForeignKey(x => x.AttendeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Tickets).WithOne(x => x.Order).HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
    }
}
