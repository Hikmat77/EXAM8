using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository(ApplicationDbContext appDbContext) : IOrderRepository
{
    private readonly ApplicationDbContext _context = appDbContext;

    public async Task<bool> AttendeeExistsAsync(int attendeeId)
    {
        return await _context.Attendees.AnyAsync(x => x.Id == attendeeId);
    }

    public async Task<List<TicketType>> GetTicketTypesAsync(List<int> ticketTypeIds)
    {
        return await _context.TicketTypes
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Include(x => x.Event)
            .Where(x => ticketTypeIds.Contains(x.Id))
            .ToListAsync();
    }

    public async Task<Dictionary<int, int>> GetAttendeeTicketCountsByEventAsync(int attendeeId)
    {
        return await _context.Tickets
            .Where(x => x.AttendeeId == attendeeId &&
                x.Order.Status != OrderStatus.Cancelled &&
                x.Order.Status != OrderStatus.Expired)
            .GroupBy(x => x.TicketType.EventId)
            .Select(x => new { EventId = x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.EventId, x => x.Count);
    }

    public async Task<List<Order>> GetExpiredPendingAsync(DateTime cutoff)
    {
        return await _context.Orders
            .Include(x => x.Tickets)
            .ThenInclude(x => x.TicketType)
            .Where(x => x.Status == OrderStatus.Pending && x.CreatedAt <= cutoff)
            .ToListAsync();
    }

    public async Task<Order> CreateAsync(int attendeeId, List<(int TicketTypeId, int Quantity)> items)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var attendee = await _context.Attendees.FirstOrDefaultAsync(x => x.Id == attendeeId)
                ?? throw new KeyNotFoundException("Attendee not found.");
            attendee.PurchaseVersion++;

            var ticketTypeIds = items.Select(x => x.TicketTypeId).Distinct().ToList();
            var ticketTypes = await _context.TicketTypes
                .IgnoreQueryFilters()
                .Include(x => x.Event)
                .Where(x => ticketTypeIds.Contains(x.Id))
                .ToListAsync();
            if (ticketTypes.Count != ticketTypeIds.Count)
                throw new KeyNotFoundException("One or more ticket types were not found.");

            var order = new Order { AttendeeId = attendeeId, Status = OrderStatus.Pending };
            decimal totalAmount = 0;
            foreach (var item in items)
            {
                var ticketType = ticketTypes.First(x => x.Id == item.TicketTypeId);
                if (ticketType.Event.IsDeleted ||
                    ticketType.Event.Status != EventStatus.Published ||
                    ticketType.Event.StartDate <= DateTime.UtcNow)
                {
                    throw new InvalidOperationException("Tickets can be purchased only for published future events.");
                }
                if (item.Quantity > ticketType.Quantity - ticketType.SoldCount)
                {
                    throw new InvalidOperationException("Not enough tickets are available.");
                }
                for (var index = 0; index < item.Quantity; index++)
                {
                    order.Tickets.Add(new Ticket
                    {
                        TicketTypeId = ticketType.Id,
                        AttendeeId = attendeeId,
                        Code = Guid.NewGuid(),
                        Price = ticketType.Price,
                        PurchasedAt = DateTime.UtcNow
                    });
                }
                ticketType.SoldCount += item.Quantity;
                totalAmount += ticketType.Price * item.Quantity;
            }

            foreach (var eventItems in items
                .Join(ticketTypes, item => item.TicketTypeId, ticketType => ticketType.Id, (item, ticketType) => new
                {
                    ticketType.EventId,
                    item.Quantity
                })
                .GroupBy(x => x.EventId))
            {
                var existingTicketCount = await _context.Tickets.CountAsync(x =>
                    x.AttendeeId == attendeeId &&
                    x.TicketType.EventId == eventItems.Key &&
                    x.Order.Status != OrderStatus.Cancelled &&
                    x.Order.Status != OrderStatus.Expired);
                if (existingTicketCount + eventItems.Sum(x => x.Quantity) > 5)
                {
                    throw new InvalidOperationException("The limit is 5 tickets per attendee per event.");
                }
            }

            order.TotalAmount = totalAmount;
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return order;
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("Ticket availability changed. Please try again.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Order?> GetAsync(int id)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(x => x.Tickets)
            .ThenInclude(x => x.TicketType)
            .ThenInclude(x => x.Event)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Order?> GetForPaymentAsync(int id)
    {
        return await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Order?> GetForCancellationAsync(int id)
    {
        return await _context.Orders
            .Include(x => x.Tickets)
            .ThenInclude(x => x.TicketType)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task UpdateAsync()
    {
        await _context.SaveChangesAsync();
    }
}
