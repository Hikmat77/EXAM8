using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Venue
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Address { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public List<Event> Events { get; set; } = new();
}