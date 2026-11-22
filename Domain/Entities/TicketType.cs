using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class TicketType
{
    public int Id { get; set; }
    public int EventId { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int SoldCount { get; set; }
    public uint Version { get; set; }
    public Event Event { get; set; } = null!;
    public List<Ticket> Tickets { get; set; } = new();
}
