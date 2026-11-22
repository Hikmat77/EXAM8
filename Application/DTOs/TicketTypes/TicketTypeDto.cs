namespace Application.DTOs;

public class TicketTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int SoldCount { get; set; }
    public int Available { get; set; }
}
