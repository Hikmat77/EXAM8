using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("tickets")]
public class TicketsController(
    ITicketService ticketService,
    ILogger<TicketsController> logger) : ControllerBase
{
    private ITicketService service = ticketService;
    private ILogger<TicketsController> _logger = logger;

    [HttpGet("{code}")]
    public async Task<ApiResponse<TicketDto>> Get(Guid code)
    {
        _logger.LogInformation("GET tickets/{code} {date}", code, DateTime.Now);
        return await service.GetAsync(code);
    }

    [HttpPost("{code}/check-in")]
    public async Task<ApiResponse<string>> CheckIn(Guid code)
    {
        _logger.LogInformation("POST tickets/{code}/check-in {date}", code, DateTime.Now);
        return await service.CheckInAsync(code);
    }
}
