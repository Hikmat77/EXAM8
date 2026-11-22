using Domain.Entities;

namespace Application.DTOs;

public class EventDetailsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public EventStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public VenueDto Venue { get; set; } = new();
    public string OrganizerName { get; set; } = string.Empty;
    public List<CategoryDto> Categories { get; set; } = new();
    public List<TicketTypeDto> TicketTypes { get; set; } = new();
    public decimal AverageRating { get; set; }
    public int ReviewsCount { get; set; }
}