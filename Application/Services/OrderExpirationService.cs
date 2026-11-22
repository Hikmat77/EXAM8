using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class OrderExpirationService(IOrderRepository orderRepository) : IOrderExpirationService
{
    private IOrderRepository repository = orderRepository;

    public async Task ExpirePendingOrdersAsync()
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-15);
        var orders = await repository.GetExpiredPendingAsync(cutoff);
        foreach (var order in orders)
        {
            foreach (var ticket in order.Tickets)
            {
                ticket.TicketType.SoldCount--;
            }
            order.Status = OrderStatus.Expired;
        }
        if (orders.Count > 0)
        {
            await repository.UpdateAsync();
        }
    }
}
