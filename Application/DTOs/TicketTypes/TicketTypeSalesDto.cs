namespace Application.DTOs;

public class TicketTypeSalesDto
{
    public int TicketTypeId { get; set; }
    public string TicketTypeName { get; set; } = string.Empty;
    public int SoldCount { get; set; }
    public decimal Revenue { get; set; }
    public decimal CapacityPercent { get; set; }
    public decimal CheckInPercent { get; set; }
}