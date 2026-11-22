using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class EventCreateDto
{
    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(2000)]
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int VenueId { get; set; }
    public int OrganizerId { get; set; }
    public List<int> CategoryIds { get; set; } = new();
}