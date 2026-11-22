using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrganizerRepository(ApplicationDbContext appDbContext) : IOrganizerRepository
{
    private readonly ApplicationDbContext _context = appDbContext;

    public async Task<Organizer> AddAsync(Organizer organizer)
    {
        await _context.Organizers.AddAsync(organizer);
        await _context.SaveChangesAsync();
        return organizer;
    }

    public async Task<List<Organizer>> GetAllAsync()
    {
        return await _context.Organizers
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

    public async Task<Organizer?> GetAsync(int id)
    {
        return await _context.Organizers.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Event>> GetEventsAsync(int organizerId)
    {
        return await _context.Events
            .AsNoTracking()
            .Include(x => x.Venue)
            .Include(x => x.TicketTypes)
            .Where(x => x.OrganizerId == organizerId)
            .ToListAsync();
    }
}
