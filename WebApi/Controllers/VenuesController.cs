using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("venues")]
public class VenuesController(
    IVenueService venueService,
    ILogger<VenuesController> logger) : ControllerBase
{
    private IVenueService service = venueService;
    private ILogger<VenuesController> _logger = logger;

    [HttpPost]
    public async Task<ApiResponse<VenueDto>> Create(VenueCreateDto dto)
    {
        _logger.LogInformation("POST venues {date}", DateTime.Now);
        return await service.CreateAsync(dto);
    }

    [HttpGet]
    public async Task<ApiResponse<List<VenueDto>>> GetAll()
    {
        _logger.LogInformation("GET venues {date}", DateTime.Now);
        return await service.GetAllAsync();
    }

    [HttpDelete("{id:int}")]
    public async Task<ApiResponse<string>> Delete(int id)
    {
        _logger.LogInformation("DELETE venues/{id} {date}", id, DateTime.Now);
        return await service.DeleteAsync(id);
    }
}
