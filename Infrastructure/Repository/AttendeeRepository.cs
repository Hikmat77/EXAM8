using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AttendeeRepository(ApplicationDbContext appDbContext) : IAttendeeRepository
{
    private readonly ApplicationDbContext _context = appDbContext;

    public async Task<Attendee> AddAsync(Attendee attendee)
    {
        await _context.Attendees.AddAsync(attendee);
        await _context.SaveChangesAsync();
        return attendee;
    }

    public async Task<Attendee?> GetByEmailAsync(string email)
    {
        return await _context.Attendees.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<Attendee?> GetAsync(int id)
    {
        return await _context.Attendees
            .Include(x => x.Profile)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Attendees.AnyAsync(x => x.Id == id);
    }

    public async Task<List<Attendee>> GetPagedAsync(int page, int pageSize)
    {
        return await _context.Attendees
            .AsNoTracking()
            .Include(x => x.Profile)
            .OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Attendees.CountAsync();
    }

    public async Task UpdateAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Order>> GetOrdersAsync(int attendeeId, int page, int pageSize)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(x => x.AttendeeId == attendeeId)
            .Include(x => x.Tickets)
            .ThenInclude(x => x.TicketType)
            .ThenInclude(x => x.Event)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountOrdersAsync(int attendeeId)
    {
        return await _context.Orders.CountAsync(x => x.AttendeeId == attendeeId);
    }

    public async Task<List<Ticket>> GetTicketsAsync(int attendeeId)
    {
        return await _context.Tickets
            .AsNoTracking()
            .Where(x => x.AttendeeId == attendeeId)
            .Include(x => x.TicketType)
            .ThenInclude(x => x.Event)
            .ToListAsync();
    }
}
