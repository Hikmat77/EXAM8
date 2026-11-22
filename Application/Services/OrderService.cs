using Application.DTOs;
using Application.Interfaces;
using Application.Responses;
using Domain.Entities;
using Domain.Interfaces;
using System.Net;

namespace Application.Services;

public class OrderService(IOrderRepository orderRepository) : IOrderService
{
    private IOrderRepository repository = orderRepository;

    public async Task<ApiResponse<OrderDto>> CreateAsync(CreateOrderDto dto)
    {
        if (dto.Items.Count == 0 || dto.Items.Any(x => x.Quantity <= 0))
        {
            return new ApiResponse<OrderDto>(HttpStatusCode.BadRequest, "Order must contain items with positive quantities.");
        }
        var items = dto.Items
            .GroupBy(x => x.TicketTypeId)
            .Select(x => (TicketTypeId: x.Key, Quantity: x.Sum(i => i.Quantity)))
            .ToList();
        if (!await repository.AttendeeExistsAsync(dto.AttendeeId))
        {
            return new ApiResponse<OrderDto>(HttpStatusCode.NotFound, "Attendee not found.");
        }
        var ticketTypes = await repository.GetTicketTypesAsync(items.Select(x => x.TicketTypeId).ToList());
        if (ticketTypes.Count != items.Count)
        {
            return new ApiResponse<OrderDto>(HttpStatusCode.NotFound, "One or more ticket types were not found.");
        }
        foreach (var item in items)
        {
            var ticketType = ticketTypes.First(x => x.Id == item.TicketTypeId);
            if (ticketType.Event.IsDeleted ||
                ticketType.Event.Status != EventStatus.Published ||
                ticketType.Event.StartDate <= DateTime.UtcNow)
            {
                return new ApiResponse<OrderDto>(
                    HttpStatusCode.Conflict,
                    "Tickets can be purchased only for published future events.");
            }
            if (item.Quantity > ticketType.Quantity - ticketType.SoldCount)
            {
                return new ApiResponse<OrderDto>(HttpStatusCode.Conflict, "Not enough tickets are available.");
            }
        }
        var attendeeTicketCounts = await repository.GetAttendeeTicketCountsByEventAsync(dto.AttendeeId);
        foreach (var eventItems in items
            .Join(ticketTypes, item => item.TicketTypeId, ticketType => ticketType.Id, (item, ticketType) => new
            {
                ticketType.EventId,
                item.Quantity
            })
            .GroupBy(x => x.EventId))
        {
            var existingTicketCount = attendeeTicketCounts.TryGetValue(eventItems.Key, out var count) ? count : 0;
            if (existingTicketCount + eventItems.Sum(x => x.Quantity) > 5)
            {
                return new ApiResponse<OrderDto>(
                    HttpStatusCode.Conflict,
                    "The limit is 5 tickets per attendee per event.");
            }
        }

        try
        {
            var order = await repository.CreateAsync(dto.AttendeeId, items);
            var result = new OrderDto()
            {
                Id = order.Id,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                Tickets = order.Tickets.Select(x => new TicketDto()
                {
                    Code = x.Code,
                    EventTitle = x.TicketType.Event.Title,
                    TicketTypeName = x.TicketType.Name,
                    Price = x.Price,
                    IsCheckedIn = x.IsCheckedIn
                }).ToList()
            };
            return new ApiResponse<OrderDto>(HttpStatusCode.Created, "Order created.", result);
        }
        catch (KeyNotFoundException ex)
        {
            return new ApiResponse<OrderDto>(HttpStatusCode.NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return new ApiResponse<OrderDto>(HttpStatusCode.Conflict, ex.Message);
        }
    }

    public async Task<ApiResponse<OrderDto>> GetAsync(int id)
    {
        var order = await repository.GetAsync(id);
        if (order == null)
        {
            return new ApiResponse<OrderDto>(HttpStatusCode.NotFound, "Order not found.");
        }
        var result = new OrderDto()
        {
            Id = order.Id,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            Tickets = order.Tickets.Select(x => new TicketDto()
            {
                Code = x.Code,
                EventTitle = x.TicketType.Event.Title,
                TicketTypeName = x.TicketType.Name,
                Price = x.Price,
                IsCheckedIn = x.IsCheckedIn
            }).ToList()
        };
        return new ApiResponse<OrderDto>(HttpStatusCode.OK, "Order details.", result);
    }

    public async Task<ApiResponse<string>> PayAsync(int id)
    {
        var order = await repository.GetForPaymentAsync(id);
        if (order == null)
        {
            return new ApiResponse<string>(HttpStatusCode.NotFound, "Order not found.");
        }
        if (order.Status != OrderStatus.Pending)
        {
            return new ApiResponse<string>(HttpStatusCode.Conflict, "Only Pending orders can be paid.");
        }
        order.Status = OrderStatus.Paid;
        order.PaidAt = DateTime.UtcNow;
        await repository.UpdateAsync();
        return new ApiResponse<string>(HttpStatusCode.OK, "Order paid.", "Order paid.");
    }

    public async Task<ApiResponse<string>> CancelAsync(int id)
    {
        var order = await repository.GetForCancellationAsync(id);
        if (order == null)
        {
            return new ApiResponse<string>(HttpStatusCode.NotFound, "Order not found.");
        }
        if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Expired)
        {
            return new ApiResponse<string>(HttpStatusCode.Conflict, "Order is already cancelled or expired.");
        }
        foreach (var ticket in order.Tickets)
        {
            ticket.TicketType.SoldCount--;
        }
        order.Status = OrderStatus.Cancelled;
        await repository.UpdateAsync();
        return new ApiResponse<string>(HttpStatusCode.OK, "Order cancelled.", "Order cancelled.");
    }
}
