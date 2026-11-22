using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EventRepository(ApplicationDbContext appDbContext) : IEventRepository, IEventQueryRepository
{
    private readonly ApplicationDbContext _context = appDbContext;

    public async Task<Event?> GetAsync(int id)
    {
        return await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Event?> GetWithCategoriesAsync(int id)
    {
        return await _context.Events
            .Include(x => x.Categories)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Event?> GetDetailsAsync(int id)
    {
        return await _context.Events
            .AsNoTracking()
            .Include(x => x.Venue)
            .Include(x => x.Organizer)
            .Include(x => x.Categories)
            .Include(x => x.TicketTypes)
            .Include(x => x.Reviews)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Event?> GetWithTicketTypesAsync(int id)
    {
        return await _context.Events
            .Include(x => x.TicketTypes)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Event> AddAsync(Event entity)
    {
        await _context.Events.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<List<EventDto>> GetPublishedAsync(
        string? city, int? categoryId, DateTime? dateFrom, DateTime? dateTo, decimal? maxPrice,
        string? search, int page, int pageSize, string sortBy, string sortDir)
    {
        var query = FilterPublished(city, categoryId, dateFrom, dateTo, maxPrice, search);
        IOrderedQueryable<Event> ordered = (sortBy.ToLowerInvariant(), sortDir.ToLowerInvariant()) switch
        {
            ("price", "asc") => query.OrderBy(x => x.TicketTypes.Any() ? x.TicketTypes.Min(t => t.Price) : 0),
            ("price", "desc") => query.OrderByDescending(x => x.TicketTypes.Any() ? x.TicketTypes.Min(t => t.Price) : 0),
            ("title", "desc") => query.OrderByDescending(x => x.Title),
            ("title", _) => query.OrderBy(x => x.Title),
            (_, "desc") => query.OrderByDescending(x => x.StartDate),
            _ => query.OrderBy(x => x.StartDate)
        };

        return await ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new EventDto
            {
                Id = x.Id,
                Title = x.Title,
                StartDate = x.StartDate,
                Status = x.Status,
                City = x.Venue.City,
                MinPrice = x.TicketTypes.Min(ticketType => (decimal?)ticketType.Price)
            })
            .ToListAsync();
    }

    public async Task<int> CountPublishedAsync(
        string? city, int? categoryId, DateTime? dateFrom, DateTime? dateTo, decimal? maxPrice, string? search)
    {
        return await FilterPublished(city, categoryId, dateFrom, dateTo, maxPrice, search).CountAsync();
    }

    public async Task<bool> VenueExistsAsync(int venueId)
    {
        return await _context.Venues.AnyAsync(x => x.Id == venueId);
    }

    public async Task<bool> OrganizerExistsAsync(int organizerId)
    {
        return await _context.Organizers.AnyAsync(x => x.Id == organizerId);
    }

    public async Task<List<Category>> GetCategoriesAsync(List<int> categoryIds)
    {
        return await _context.Categories
            .Where(x => categoryIds.Contains(x.Id))
            .ToListAsync();
    }

    public async Task<string?> GetVenueCityAsync(int venueId)
    {
        return await _context.Venues
            .Where(x => x.Id == venueId)
            .Select(x => x.City)
            .FirstOrDefaultAsync();
    }

    public async Task<TicketType> AddTicketTypeAsync(TicketType ticketType)
    {
        await _context.TicketTypes.AddAsync(ticketType);
        await _context.SaveChangesAsync();
        return ticketType;
    }

    public async Task<List<Order>> GetPaidOrdersForEventAsync(int eventId)
    {
        return await _context.Orders
            .Include(x => x.Tickets)
            .ThenInclude(x => x.TicketType)
            .Where(x => x.Status == OrderStatus.Paid &&
                x.Tickets.Any(t => t.TicketType.EventId == eventId))
            .ToListAsync();
    }

    public async Task<List<Review>> GetReviewsAsync(int eventId, int page, int pageSize)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountReviewsAsync(int eventId)
    {
        return await _context.Reviews.CountAsync(x => x.EventId == eventId);
    }

    public async Task<bool> AttendeeHasReviewAsync(int eventId, int attendeeId)
    {
        return await _context.Reviews.AnyAsync(x => x.EventId == eventId && x.AttendeeId == attendeeId);
    }

    public async Task<bool> AttendeeHasCheckedInPaidTicketAsync(int eventId, int attendeeId)
    {
        return await _context.Tickets.AnyAsync(x =>
            x.AttendeeId == attendeeId &&
            x.TicketType.EventId == eventId &&
            x.IsCheckedIn &&
            x.Order.Status == OrderStatus.Paid);
    }

    public async Task<Review> AddReviewAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
        return review;
    }

    public async Task UpdateAsync()
    {
        await _context.SaveChangesAsync();
    }

    private IQueryable<Event> FilterPublished(
        string? city, int? categoryId, DateTime? dateFrom, DateTime? dateTo, decimal? maxPrice, string? search)
    {
        var query = _context.Events
            .AsNoTracking()
            .Where(x => x.Status == EventStatus.Published);
        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(x => x.Venue.City == city);
        if (categoryId.HasValue)
            query = query.Where(x => x.Categories.Any(c => c.Id == categoryId.Value));
        if (dateFrom.HasValue)
            query = query.Where(x => x.StartDate >= dateFrom.Value);
        if (dateTo.HasValue)
            query = query.Where(x => x.StartDate <= dateTo.Value);
        if (maxPrice.HasValue)
            query = query.Where(x => x.TicketTypes.Any(t => t.Price <= maxPrice.Value));
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => EF.Functions.ILike(x.Title, "%" + search + "%"));
        return query;
    }
}
