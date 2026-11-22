using Domain.Entities;

namespace Domain.Interfaces;

public interface ITicketRepository
{
    Task<Ticket?> GetAsync(Guid code);
    Task UpdateAsync();
}