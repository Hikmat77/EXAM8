using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("attendees")]
public class AttendeesController(
    IAttendeeService attendeeService,
    ILogger<AttendeesController> logger) : ControllerBase
{
    private IAttendeeService service = attendeeService;
    private ILogger<AttendeesController> _logger = logger;

    [HttpPost]
    public async Task<ApiResponse<AttendeeDto>> Create(AttendeeCreateDto dto)
    {
        _logger.LogInformation("POST attendees {date}", DateTime.Now);
        return await service.CreateAsync(dto);
    }

    [HttpGet]
    public async Task<ApiResponse<PagedResult<AttendeeDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("GET attendees?page={page}&pageSize={pageSize} {date}", page, pageSize, DateTime.Now);
        return await service.GetPagedAsync(page, pageSize);
    }

    [HttpPut("{id:int}/profile")]
    public async Task<ApiResponse<AttendeeDto>> UpdateProfile(int id, AttendeeProfileDto dto)
    {
        _logger.LogInformation("PUT attendees/{id}/profile {date}", id, DateTime.Now);
        return await service.UpdateProfileAsync(id, dto);
    }

    [HttpGet("{id:int}/orders")]
    public async Task<ApiResponse<PagedResult<OrderDto>>> GetOrders(
        int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("GET attendees/{id}/orders?page={page}&pageSize={pageSize} {date}", id, page, pageSize, DateTime.Now);
        return await service.GetOrdersAsync(id, page, pageSize);
    }

    [HttpGet("{id:int}/tickets")]
    public async Task<ApiResponse<List<TicketDto>>> GetTickets(int id)
    {
        _logger.LogInformation("GET attendees/{id}/tickets {date}", id, DateTime.Now);
        return await service.GetTicketsAsync(id);
    }
}
