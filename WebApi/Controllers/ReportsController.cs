using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("reports")]
public class ReportsController(
    IReportService reportService,
    ILogger<ReportsController> logger) : ControllerBase
{
    private IReportService service = reportService;
    private ILogger<ReportsController> _logger = logger;

    [HttpGet("events/{id:int}/sales")]
    public async Task<ApiResponse<List<TicketTypeSalesDto>>> GetSales(int id)
    {
        _logger.LogInformation("GET reports/events/{id}/sales {date}", id, DateTime.Now);
        return await service.GetSalesAsync(id);
    }

    [HttpGet("top-events")]
    public async Task<ApiResponse<List<TopEventDto>>> GetTopEvents([FromQuery] int count = 5)
    {
        _logger.LogInformation("GET reports/top-events?count={count} {date}", count, DateTime.Now);
        return await service.GetTopEventsAsync(count);
    }
}
