namespace Application.Interfaces;

public interface IOrderExpirationService
{
    Task ExpirePendingOrdersAsync();
}
