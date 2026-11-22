using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReportRepository(ApplicationDbContext appDbContext) : IReportRepository
{
    private readonly ApplicationDbContext _context = appDbContext;

    public async Task<bool> EventExistsAsync(int eventId)
    {
        return await _context.Events.AnyAsync(x => x.Id == eventId);
    }

    public async Task<List<TicketTypeSalesDto>> GetTicketTypeSalesAsync(int eventId)
    {
        var ticketTypes = await _context.TicketTypes
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Quantity
            })
            .ToListAsync();
        var sales = await _context.Tickets
            .AsNoTracking()
            .Where(x => x.TicketType.EventId == eventId && x.Order.Status == OrderStatus.Paid)
            .GroupBy(x => x.TicketTypeId)
            .Select(x => new
            {
                TicketTypeId = x.Key,
                SoldCount = x.Count(),
                Revenue = x.Sum(ticket => ticket.Price),
                CheckedInCount = x.Count(ticket => ticket.IsCheckedIn)
            })
            .ToListAsync();

        return ticketTypes.Select(x =>
        {
            var sale = sales.FirstOrDefault(s => s.TicketTypeId == x.Id);
            var soldCount = sale?.SoldCount ?? 0;
            return new TicketTypeSalesDto
            {
                TicketTypeId = x.Id,
                TicketTypeName = x.Name,
                SoldCount = soldCount,
                Revenue = sale?.Revenue ?? 0,
                CapacityPercent = x.Quantity == 0 ? 0 : (decimal)soldCount * 100 / x.Quantity,
                CheckInPercent = soldCount == 0 ? 0 : (decimal)sale!.CheckedInCount * 100 / soldCount
            };
        }).ToList();
    }

    public async Task<List<TopEventDto>> GetTopEventsAsync(int count)
    {
        return await _context.Events
            .AsNoTracking()
            .Select(x => new TopEventDto
            {
                EventId = x.Id,
                Title = x.Title,
                Revenue = x.TicketTypes
                    .SelectMany(t => t.Tickets)
                    .Where(t => t.Order.Status == OrderStatus.Paid)
                    .Sum(t => (decimal?)t.Price) ?? 0
            })
            .OrderByDescending(x => x.Revenue)
            .Take(count)
            .ToListAsync();
    }
}