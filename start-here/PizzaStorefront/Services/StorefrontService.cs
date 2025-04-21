using PizzaStorefront.Models;

namespace PizzaStorefront.Services;

public interface IStorefrontService
{
    Task<Order> ProcessOrderAsync(Order order);
}

public class StorefrontService : IStorefrontService
{
    private readonly ILogger<StorefrontService> _logger;

    public StorefrontService(ILogger<StorefrontService> logger)
    {
        _logger = logger;
    }

    public async Task<Order> ProcessOrderAsync(Order order)
    {
        //TODO: Implement order processing
    }
}