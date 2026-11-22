using Application.DTOs;
using Application.Responses;

namespace Application.Interfaces;

public interface IOrganizerService
{
    Task<ApiResponse<OrganizerDto>> CreateAsync(OrganizerCreateDto dto);
    Task<ApiResponse<List<OrganizerDto>>> GetAllAsync();
    Task<ApiResponse<List<EventDto>>> GetEventsAsync(int organizerId);
}