using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CreateOrderDto
{
    [Range(1, int.MaxValue)]
    public int AttendeeId { get; set; }
    [Required, MinLength(1)]
    public List<OrderItemDto> Items { get; set; } = new();
}