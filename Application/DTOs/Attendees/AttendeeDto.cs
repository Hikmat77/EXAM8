namespace Application.DTOs;

public class AttendeeDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
    public AttendeeProfileDto? Profile { get; set; }
}