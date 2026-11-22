using Application.DTOs;
using Application.Responses;

namespace Application.Interfaces;

public interface IEventService
{
    Task<ApiResponse<PagedResult<EventDto>>> GetAsync(string? city, int? categoryId, DateTime? dateFrom, DateTime? dateTo, decimal? maxPrice, string? search, int page, int pageSize, string sortBy, string sortDir);
    Task<ApiResponse<EventDetailsDto>> GetDetailsAsync(int id);
    Task<ApiResponse<EventDto>> CreateAsync(EventCreateDto dto);
    Task<ApiResponse<EventDto>> UpdateAsync(int id, EventUpdateDto dto);
    Task<ApiResponse<string>> PublishAsync(int id);
    Task<ApiResponse<string>> CancelAsync(int id);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<TicketTypeDto>> AddTicketTypeAsync(int eventId, TicketTypeCreateDto dto);
    Task<ApiResponse<PagedResult<ReviewDto>>> GetReviewsAsync(int eventId, int page, int pageSize);
    Task<ApiResponse<ReviewDto>> CreateReviewAsync(int eventId, ReviewCreateDto dto);
}