using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class AttendeeProfile
{
    public int AttendeeId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? City { get; set; }
    [MaxLength(1000)]
    public string? Bio { get; set; }
    public Attendee Attendee { get; set; } = null!;
}
