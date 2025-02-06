using Dapr.Client;
using PizzaShared.Messages.StoreFront;

namespace PizzaKitchen.Services;

public interface ICookService
{
    Task<OrderResultMessage> CookPizzaAsync(OrderMessage order);
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

    public async Task<OrderResultMessage> CookPizzaAsync(OrderMessage orderMessage)
    {
        var stages = new (string status, int duration)[]
        {
            ("cooking_preparing_ingredients", 2),
            ("cooking_making_dough", 3),
            ("cooking_adding_toppings", 2),
            ("cooking_baking", 5),
            ("cooking_quality_check", 1)
        };

        var order = new OrderResultMessage
        {
            WorkflowId = orderMessage.WorkflowId,
            OrderId = orderMessage.OrderId,
            Status = "unknown"
        };

        try
        {
            foreach (var (status, duration) in stages)
            {
                order.Status = status;
                _logger.LogInformation("Order {OrderId} - {Status}", orderMessage.OrderId, status);

                await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, orderMessage);
                await Task.Delay(TimeSpan.FromSeconds(duration));
            }

            order.Status = "cooked";
            await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, orderMessage);
            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cooking order {OrderId}", orderMessage.OrderId);
            order.Status = "cooking_failed";
            order.Error = ex.Message;
            await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, orderMessage);
            return order;
        }
    }
}