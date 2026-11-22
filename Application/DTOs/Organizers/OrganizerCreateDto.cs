using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class OrganizerCreateDto
{

    [Required, MaxLength(100)]
    public string CompanyName { get; set; } = string.Empty;
    [Required, EmailAddress, MaxLength(150)]
    public string ContactEmail { get; set; } = string.Empty;
    public string? Phone { get; set; }
}