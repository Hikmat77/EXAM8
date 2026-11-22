using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TicketRepository(ApplicationDbContext appDbContext) : ITicketRepository
{
    private readonly ApplicationDbContext _context = appDbContext;

    public async Task<Ticket?> GetAsync(Guid code)
    {
        return await _context.Tickets
            .Include(x => x.Order)
            .Include(x => x.TicketType)
            .ThenInclude(x => x.Event)
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task UpdateAsync()
    {
        await _context.SaveChangesAsync();
    }
}
