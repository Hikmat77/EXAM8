using Application.DTOs;

namespace Application.Interfaces;

public interface IEventQueryRepository
{
    Task<List<EventDto>> GetPublishedAsync(
        string? city,
        int? categoryId,
        DateTime? dateFrom,
        DateTime? dateTo,
        decimal? maxPrice,
        string? search,
        int page,
        int pageSize,
        string sortBy,
        string sortDir);
}
