using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Domain.Entities;
using Domain.Interfaces;
using System.Net;

namespace Application.Services;

public class TicketService(ITicketRepository ticketRepository) : ITicketService
{
    private ITicketRepository repository = ticketRepository;

    public async Task<ApiResponse<TicketDto>> GetAsync(Guid code)
    {
        var ticket = await repository.GetAsync(code);
        if (ticket == null)
        {
            return new ApiResponse<TicketDto>(HttpStatusCode.NotFound, "Ticket not found.");
        }
        var result = new TicketDto()
        {
            Code = ticket.Code,
            EventTitle = ticket.TicketType.Event.Title,
            TicketTypeName = ticket.TicketType.Name,
            Price = ticket.Price,
            IsCheckedIn = ticket.IsCheckedIn
        };
        return new ApiResponse<TicketDto>(HttpStatusCode.OK, "Ticket details.", result);
    }

    public async Task<ApiResponse<string>> CheckInAsync(Guid code)
    {
        var ticket = await repository.GetAsync(code);
        if (ticket == null)
        {
            return new ApiResponse<string>(HttpStatusCode.NotFound, "Ticket not found.");
        }
        if (ticket.Order.Status != OrderStatus.Paid)
        {
            return new ApiResponse<string>(HttpStatusCode.Conflict, "Ticket order is not paid.");
        }
        if (ticket.IsCheckedIn)
        {
            return new ApiResponse<string>(HttpStatusCode.Conflict, "Ticket is already checked in.");
        }
        if (DateTime.UtcNow < ticket.TicketType.Event.StartDate.AddHours(-2))
        {
            return new ApiResponse<string>(HttpStatusCode.Conflict, "Check-in starts 2 hours before the event.");
        }
        ticket.IsCheckedIn = true;
        ticket.CheckedInAt = DateTime.UtcNow;
        await repository.UpdateAsync();
        return new ApiResponse<string>(HttpStatusCode.OK, "Ticket checked in.", "Ticket checked in.");
    }
}
