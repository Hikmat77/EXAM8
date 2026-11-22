using Domain.Entities;

namespace Domain.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetAsync(int id);
    Task<Event?> GetWithCategoriesAsync(int id);
    Task<Event?> GetDetailsAsync(int id);
    Task<Event?> GetWithTicketTypesAsync(int id);
    Task<Event> AddAsync(Event entity);
    Task<int> CountPublishedAsync(string? city, int? categoryId, DateTime? dateFrom, DateTime? dateTo, decimal? maxPrice, string? search);
    Task<bool> VenueExistsAsync(int venueId);
    Task<bool> OrganizerExistsAsync(int organizerId);
    Task<List<Category>> GetCategoriesAsync(List<int> categoryIds);
    Task<string?> GetVenueCityAsync(int venueId);
    Task<TicketType> AddTicketTypeAsync(TicketType ticketType);
    Task<List<Order>> GetPaidOrdersForEventAsync(int eventId);
    Task<List<Review>> GetReviewsAsync(int eventId, int page, int pageSize);
    Task<int> CountReviewsAsync(int eventId);
    Task<bool> AttendeeHasReviewAsync(int eventId, int attendeeId);
    Task<bool> AttendeeHasCheckedInPaidTicketAsync(int eventId, int attendeeId);
    Task<Review> AddReviewAsync(Review review);
    Task UpdateAsync();
}
