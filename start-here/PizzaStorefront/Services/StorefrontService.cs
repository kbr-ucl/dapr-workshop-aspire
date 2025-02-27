using Dapr.Client;
using PizzaShared.Messages.StoreFront;

namespace PizzaStorefront.Services;

public interface IStorefrontService
{
    Task<OrderResultMessage> ProcessOrderAsync(OrderMessage order);
}

public class StorefrontService : IStorefrontService
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<StorefrontService> _logger;

    private const string PUBSUB_NAME = "pizzapubsub";
    private const string TOPIC_NAME = "orders";

    public StorefrontService(DaprClient daprClient, ILogger<StorefrontService> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public async Task<OrderResultMessage> ProcessOrderAsync(OrderMessage orderMessage)
    {
        var stages = new (string status, int duration)[]
        {
            ("validating", 1),
            ("processing", 2),
            ("confirmed", 1)
        };

        var order = new OrderResultMessage
        {
            WorkflowId = orderMessage.WorkflowId,
            OrderId = orderMessage.OrderId,
            PizzaType = orderMessage.PizzaType,
            Size = orderMessage.Size,
            Customer = orderMessage.Customer,
            Status = "unknown"
        };

        try
        {
            // Set pizza order status
            foreach (var (status, duration) in stages)
            {
                order.Status = status;
                _logger.LogInformation("Order {OrderId} - {Status}", order.OrderId, status);

                await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, order);
                await Task.Delay(TimeSpan.FromSeconds(duration));
            }

            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order {OrderId}", order.OrderId);
            order.Status = "failed";
            order.Error = ex.Message;

            await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, order);
            return order;
        }
    }
}