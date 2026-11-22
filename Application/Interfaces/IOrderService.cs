using Application.DTOs;
using Application.Responses;

namespace Application.Interfaces;

public interface IOrderService
{
    Task<ApiResponse<OrderDto>> CreateAsync(CreateOrderDto dto);
    Task<ApiResponse<OrderDto>> GetAsync(int id);
    Task<ApiResponse<string>> PayAsync(int id);
    Task<ApiResponse<string>> CancelAsync(int id);
}