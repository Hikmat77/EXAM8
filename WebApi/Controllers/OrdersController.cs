using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController(
    IOrderService orderService,
    ILogger<OrdersController> logger) : ControllerBase
{
    private IOrderService service = orderService;
    private ILogger<OrdersController> _logger = logger;

    [HttpPost]
    public async Task<ApiResponse<OrderDto>> Create(CreateOrderDto dto)
    {
        _logger.LogInformation("POST orders {date}", DateTime.Now);
        return await service.CreateAsync(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ApiResponse<OrderDto>> Get(int id)
    {
        _logger.LogInformation("GET orders/{id} {date}", id, DateTime.Now);
        return await service.GetAsync(id);
    }

    [HttpPost("{id:int}/pay")]
    public async Task<ApiResponse<string>> Pay(int id)
    {
        _logger.LogInformation("POST orders/{id}/pay {date}", id, DateTime.Now);
        return await service.PayAsync(id);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<ApiResponse<string>> Cancel(int id)
    {
        _logger.LogInformation("POST orders/{id}/cancel {date}", id, DateTime.Now);
        return await service.CancelAsync(id);
    }
}
