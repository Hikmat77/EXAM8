namespace Application.DTOs;

public class ReviewDto
{
    public int Id { get; set; }
    public int AttendeeId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}