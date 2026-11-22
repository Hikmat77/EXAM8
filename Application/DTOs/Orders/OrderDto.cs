using Domain.Entities;

namespace Application.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TicketDto> Tickets { get; set; } = new();
}