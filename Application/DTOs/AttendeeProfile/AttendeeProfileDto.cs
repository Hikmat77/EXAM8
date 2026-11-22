using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class AttendeeProfileDto
{
    public DateTime? DateOfBirth { get; set; }
    public string? City { get; set; }
    [MaxLength(1000)]
    public string? Bio { get; set; }
}