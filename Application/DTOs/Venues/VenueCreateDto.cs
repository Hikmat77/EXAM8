using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class VenueCreateDto
{
    [Required, MaxLength(100)]
     public string Name { get; set; } = string.Empty;
    [Required]
     public string Address { get; set; } = string.Empty;
    [Required]
     public string City { get; set; } = string.Empty;
    [Range(1, int.MaxValue)]
     public int Capacity { get; set; }
}