using Domain.Entities;

namespace Application.DTOs;

public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public string City { get; set; } = string.Empty;
    public EventStatus Status { get; set; }
    public decimal? MinPrice { get; set; }
}
