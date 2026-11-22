using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrderRepository
{
    Task<bool> AttendeeExistsAsync(int attendeeId);
    Task<List<TicketType>> GetTicketTypesAsync(List<int> ticketTypeIds);
    Task<Dictionary<int, int>> GetAttendeeTicketCountsByEventAsync(int attendeeId);
    Task<List<Order>> GetExpiredPendingAsync(DateTime cutoff);
    Task<Order> CreateAsync(int attendeeId, List<(int TicketTypeId, int Quantity)> items);
    Task<Order?> GetAsync(int id);
    Task<Order?> GetForPaymentAsync(int id);
    Task<Order?> GetForCancellationAsync(int id);
    Task UpdateAsync();
}