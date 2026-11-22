using Domain.Entities;

namespace Domain.Interfaces;

public interface IAttendeeRepository
{
    Task<Attendee> AddAsync(Attendee attendee);
    Task<Attendee?> GetByEmailAsync(string email);
    Task<Attendee?> GetAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<List<Attendee>> GetPagedAsync(int page, int pageSize);
    Task<int> CountAsync();
    Task UpdateAsync();
    Task<List<Domain.Entities.Order>> GetOrdersAsync(int attendeeId, int page, int pageSize);
    Task<int> CountOrdersAsync(int attendeeId);
    Task<List<Domain.Entities.Ticket>> GetTicketsAsync(int attendeeId);
}