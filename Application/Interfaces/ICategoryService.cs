using Application.DTOs;
using Application.Responses;

namespace Application.Interfaces;

public interface ICategoryService
{
    Task<ApiResponse<List<CategoryDto>>> GetAllAsync();
}