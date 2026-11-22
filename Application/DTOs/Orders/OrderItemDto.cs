using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class OrderItemDto
{
    [Range(1, int.MaxValue)]
    public int TicketTypeId { get; set; }
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
