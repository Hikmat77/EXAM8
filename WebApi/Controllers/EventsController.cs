using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("events")]
public class EventsController(
    IEventService eventService,
    ILogger<EventsController> logger) : ControllerBase
{
    private IEventService service = eventService;
    private ILogger<EventsController> _logger = logger;

    [HttpGet]
    public async Task<ApiResponse<PagedResult<EventDto>>> Get(
        [FromQuery] string? city,
        [FromQuery] int? categoryId,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "date",
        [FromQuery] string sortDir = "asc")
    {
        _logger.LogInformation("GET events {date}", DateTime.Now);
        return await service.GetAsync(city, categoryId, dateFrom, dateTo, maxPrice, search, page, pageSize, sortBy, sortDir);
    }

    [HttpGet("{id:int}")]
    public async Task<ApiResponse<EventDetailsDto>> GetDetails(int id)
    {
        _logger.LogInformation("GET events/{id} {date}", id, DateTime.Now);
        return await service.GetDetailsAsync(id);
    }

    [HttpPost]
    public async Task<ApiResponse<EventDto>> Create(EventCreateDto dto)
    {
        _logger.LogInformation("POST events {date}", DateTime.Now);
        return await service.CreateAsync(dto);
    }

    [HttpPut("{id:int}")]
    public async Task<ApiResponse<EventDto>> Update(int id, EventUpdateDto dto)
    {
        _logger.LogInformation("PUT events/{id} {date}", id, DateTime.Now);
        return await service.UpdateAsync(id, dto);
    }

    [HttpPost("{id:int}/publish")]
    public async Task<ApiResponse<string>> Publish(int id)
    {
        _logger.LogInformation("POST events/{id}/publish {date}", id, DateTime.Now);
        return await service.PublishAsync(id);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<ApiResponse<string>> Cancel(int id)
    {
        _logger.LogInformation("POST events/{id}/cancel {date}", id, DateTime.Now);
        return await service.CancelAsync(id);
    }

    [HttpDelete("{id:int}")]
    public async Task<ApiResponse<string>> Delete(int id)
    {
        _logger.LogInformation("DELETE events/{id} {date}", id, DateTime.Now);
        return await service.DeleteAsync(id);
    }

    [HttpPost("{id:int}/ticket-types")]
    public async Task<ApiResponse<TicketTypeDto>> AddTicketType(int id, TicketTypeCreateDto dto)
    {
        _logger.LogInformation("POST events/{id}/ticket-types {date}", id, DateTime.Now);
        return await service.AddTicketTypeAsync(id, dto);
    }

    [HttpGet("{id:int}/reviews")]
    public async Task<ApiResponse<PagedResult<ReviewDto>>> GetReviews(
        int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("GET events/{id}/reviews?page={page}&pageSize={pageSize} {date}", id, page, pageSize, DateTime.Now);
        return await service.GetReviewsAsync(id, page, pageSize);
    }

    [HttpPost("{id:int}/reviews")]
    public async Task<ApiResponse<ReviewDto>> CreateReview(int id, ReviewCreateDto dto)
    {
        _logger.LogInformation("POST events/{id}/reviews {date}", id, DateTime.Now);
        return await service.CreateReviewAsync(id, dto);
    }
}
