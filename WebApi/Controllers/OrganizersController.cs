using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("organizers")]
public class OrganizersController(
    IOrganizerService organizerService,
    ILogger<OrganizersController> logger) : ControllerBase
{
    private IOrganizerService service = organizerService;
    private ILogger<OrganizersController> _logger = logger;

    [HttpPost]
    public async Task<ApiResponse<OrganizerDto>> Create(OrganizerCreateDto dto)
    {
        _logger.LogInformation("POST organizers {date}", DateTime.Now);
        return await service.CreateAsync(dto);
    }

    [HttpGet]
    public async Task<ApiResponse<List<OrganizerDto>>> GetAll()
    {
        _logger.LogInformation("GET organizers {date}", DateTime.Now);
        return await service.GetAllAsync();
    }

    [HttpGet("{id:int}/events")]
    public async Task<ApiResponse<List<EventDto>>> GetEvents(int id)
    {
        _logger.LogInformation("GET organizers/{id}/events {date}", id, DateTime.Now);
        return await service.GetEventsAsync(id);
    }
}
