using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Attendee
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
    public int PurchaseVersion { get; set; }
    public AttendeeProfile? Profile { get; set; }
    public List<Order> Orders { get; set; } = new();
    public List<Ticket> Tickets { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}
