using Dapr.Client;
using PizzaShared.Messages.Kitchen;

namespace PizzaKitchen.Services;

public interface ICookService
{
    Task<CookResultMessage> CookPizzaAsync(CookMessage order);
}

public class CookService : ICookService
{
    private const string PUBSUB_NAME = "pizzapubsub";
    private const string TOPIC_NAME = "orders";
    private readonly DaprClient _daprClient;
    private readonly ILogger<CookService> _logger;


    public CookService(DaprClient daprClient, ILogger<CookService> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public async Task<CookResultMessage> CookPizzaAsync(CookMessage cookMessage)
    {
        var stages = new (string status, int duration)[]
        {
            ("cooking_preparing_ingredients", 2),
            ("cooking_making_dough", 3),
            ("cooking_adding_toppings", 2),
            ("cooking_baking", 5),
            ("cooking_quality_check", 1)
        };

        var order = new CookResultMessage
        {
            WorkflowId = cookMessage.WorkflowId,
            OrderId = cookMessage.OrderId,
            Status = "unknown"
        };

        try
        {
            foreach (var (status, duration) in stages)
            {
                order.Status = status;
                _logger.LogInformation("Order {OrderId} - {Status}", cookMessage.OrderId, status);

                await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, cookMessage);
                await Task.Delay(TimeSpan.FromSeconds(duration));
            }

            order.Status = "cooked";
            await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, cookMessage);
            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cooking order {OrderId}", cookMessage.OrderId);
            order.Status = "cooking_failed";
            order.Error = ex.Message;
            await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, cookMessage);
            return order;
        }
    }
}