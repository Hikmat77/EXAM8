using Application.DTOs;
using Application.Responses;

namespace Application.Interfaces;

public interface ITicketService
{
    Task<ApiResponse<TicketDto>> GetAsync(Guid code);
    Task<ApiResponse<string>> CheckInAsync(Guid code);
}