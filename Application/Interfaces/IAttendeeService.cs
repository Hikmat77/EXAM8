using Application.DTOs;
using Application.Responses;

namespace Application.Interfaces;

public interface IAttendeeService
{
    Task<ApiResponse<AttendeeDto>> CreateAsync(AttendeeCreateDto dto);
    Task<ApiResponse<PagedResult<AttendeeDto>>> GetPagedAsync(int page, int pageSize);
    Task<ApiResponse<AttendeeDto>> UpdateProfileAsync(int id, AttendeeProfileDto dto);
    Task<ApiResponse<PagedResult<OrderDto>>> GetOrdersAsync(int attendeeId, int page, int pageSize);
    Task<ApiResponse<List<TicketDto>>> GetTicketsAsync(int attendeeId);
}