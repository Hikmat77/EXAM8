using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Domain.Interfaces;
using System.Net;

namespace Application.Services;

public class ReportService(IReportRepository reportRepository) : IReportService
{
    private IReportRepository repository = reportRepository;

    public async Task<ApiResponse<List<TicketTypeSalesDto>>> GetSalesAsync(int eventId)
    {
        if (!await repository.EventExistsAsync(eventId))
        {
            return new ApiResponse<List<TicketTypeSalesDto>>(HttpStatusCode.NotFound, "Event not found.");
        }
        var result = await repository.GetTicketTypeSalesAsync(eventId);
        return new ApiResponse<List<TicketTypeSalesDto>>(HttpStatusCode.OK, "Event sales report.", result);
    }

    public async Task<ApiResponse<List<TopEventDto>>> GetTopEventsAsync(int count)
    {
        count = Math.Clamp(count, 1, 100);
        var result = await repository.GetTopEventsAsync(count);
        return new ApiResponse<List<TopEventDto>>(HttpStatusCode.OK, "Top events by sales.", result);
    }
}
