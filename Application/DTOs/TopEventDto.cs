namespace Application.DTOs;

public class TopEventDto
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
}