using Application.DTOs;
using Application.Responses;

namespace Application.Interfaces;

public interface IVenueService
{
    Task<ApiResponse<VenueDto>> CreateAsync(VenueCreateDto dto);
    Task<ApiResponse<List<VenueDto>>> GetAllAsync();
    Task<ApiResponse<string>> DeleteAsync(int id);
}