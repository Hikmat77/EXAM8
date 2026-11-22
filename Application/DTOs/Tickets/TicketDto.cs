namespace Application.DTOs;

public class TicketDto
{
    public Guid Code { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public string TicketTypeName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsCheckedIn { get; set; }
}