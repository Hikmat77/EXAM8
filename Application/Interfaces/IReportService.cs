using Application.DTOs;
using Application.Responses;

namespace Application.Interfaces;

public interface IReportService
{
    Task<ApiResponse<List<TicketTypeSalesDto>>> GetSalesAsync(int eventId);
    Task<ApiResponse<List<TopEventDto>>> GetTopEventsAsync(int count);
}
