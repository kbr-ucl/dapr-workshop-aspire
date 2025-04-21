using PizzaKitchen.Models;

namespace PizzaKitchen.Services;

public interface ICookService
{
    Task<Order> CookPizzaAsync(Order order);
}

public class CookService : ICookService
{
    private readonly ILogger<CookService> _logger;

    public CookService(ILogger<CookService> logger)
    {
        _logger = logger;
    }

    public async Task<Order> CookPizzaAsync(Order order)
    {
        //TODO: Implement cooking process
    }
}