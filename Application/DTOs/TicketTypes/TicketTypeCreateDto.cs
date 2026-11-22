using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class TicketTypeCreateDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Range(typeof(decimal), "0", "100000000")]
    public decimal Price { get; set; }
    [Range(0, int.MaxValue)]
     public int Quantity { get; set; }
}