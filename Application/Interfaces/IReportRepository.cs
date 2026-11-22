using Application.DTOs;

namespace Application.Interfaces;

public interface IReportRepository
{
    Task<bool> EventExistsAsync(int eventId);
    Task<List<TicketTypeSalesDto>> GetTicketTypeSalesAsync(int eventId);
    Task<List<TopEventDto>> GetTopEventsAsync(int count);
}
