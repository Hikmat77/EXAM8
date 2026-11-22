using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VenueRepository(ApplicationDbContext appDbContext) : IVenueRepository
{
    private readonly ApplicationDbContext _context = appDbContext;

    public async Task<Venue> AddAsync(Venue venue)
    {
        await _context.Venues.AddAsync(venue);
        await _context.SaveChangesAsync();
        return venue;
    }

    public async Task<List<Venue>> GetAllAsync()
    {
        return await _context.Venues
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

    public async Task<Venue?> GetWithEventsAsync(int id)
    {
        return await _context.Venues
            .IgnoreQueryFilters()
            .Include(x => x.Events)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task DeleteAsync(Venue venue)
    {
        _context.Venues.Remove(venue);
        await _context.SaveChangesAsync();
    }
}