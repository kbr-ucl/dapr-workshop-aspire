using PizzaDelivery.Models;

namespace PizzaDelivery.Services;

public interface IDeliveryService
{
    Task<Order> DeliverPizzaAsync(Order order);
}

public class DeliveryService : IDeliveryService
{
    private readonly ILogger<DeliveryService> _logger;

    public DeliveryService(ILogger<DeliveryService> logger)
    {
        _logger = logger;
    }
        
    public async Task<Order> DeliverPizzaAsync(Order order)
    {
        //TODO: Implement delivery process
    }
}

