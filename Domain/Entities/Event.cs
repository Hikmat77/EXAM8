using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Event
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(2000)]
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Draft;
    public int VenueId { get; set; }
    public int OrganizerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public Venue Venue { get; set; } = null!;
    public Organizer Organizer { get; set; } = null!;
    public List<Category> Categories { get; set; } = new();
    public List<TicketType> TicketTypes { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}