using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Domain.Entities;

public class Organizer
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string CompanyName { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string ContactEmail { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public List<Event> Events { get; set; } = new();
}