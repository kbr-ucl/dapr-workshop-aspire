using Dapr.Client;
using PizzaShared.Messages.Delivery;

namespace PizzaDelivery.Services;

public interface IDeliveryService
{
    Task<DeliverResultMessage> DeliverPizzaAsync(DeliverMessage deliverMessage);
}

public class DeliveryService : IDeliveryService
{
    private const string PUBSUB_NAME = "pizzapubsub";
    private const string TOPIC_NAME = "orders";
    private readonly DaprClient _daprClient;
    private readonly ILogger<DeliveryService> _logger;

    public DeliveryService(DaprClient daprClient, ILogger<DeliveryService> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public async Task<DeliverResultMessage> DeliverPizzaAsync(DeliverMessage deliverMessage)
    {
        var stages = new (string status, int duration)[]
        {
            ("delivery_finding_driver", 2),
            ("delivery_driver_assigned", 1),
            ("delivery_picked_up", 2),
            ("delivery_on_the_way", 5),
            ("delivery_arriving", 2),
            ("delivery_at_location", 1)
        };

        var order = new DeliverResultMessage
        {
            WorkflowId = deliverMessage.WorkflowId,
            OrderId = deliverMessage.OrderId,
            PizzaType = deliverMessage.PizzaType,
            Size = deliverMessage.Size,
            Customer = deliverMessage.Customer,
            Status = "unknown"
        };

        try
        {
            foreach (var (status, duration) in stages)
            {
                order.Status = status;
                _logger.LogInformation("Order {OrderId} - {Status}", order.OrderId, status);

                await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, order);
                await Task.Delay(TimeSpan.FromSeconds(duration));
            }

            order.Status = "delivered";
            await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, order);
            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error delivering order {OrderId}", order.OrderId);
            order.Status = "delivery_failed";
            order.Error = ex.Message;
            await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, order);
            return order;
        }
    }
}