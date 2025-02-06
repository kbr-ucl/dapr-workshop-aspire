using Dapr.Client;
using Dapr.Workflow;
using PizzaShared.Messages.StoreFront;
using PizzaWorkflow.Models;
using Customer = PizzaShared.Messages.StoreFront.Customer;

namespace PizzaWorkflow.Activities;

public class StorefrontActivity : WorkflowActivity<Order, object?>
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<StorefrontActivity> _logger;

    public StorefrontActivity(DaprClient daprClient, ILogger<StorefrontActivity> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public override async Task<object?> RunAsync(WorkflowActivityContext context, Order order)
    {
        try
        {
            _logger.LogInformation("Starting ordering process for order {OrderId}", order.OrderId);

            var message = FillOrderMessage(context, order);

            await _daprClient.PublishEventAsync("pizzapubsub", "storefront", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order {OrderId}", order.OrderId);
            throw;
        }
    }

    private OrderMessage FillOrderMessage(WorkflowActivityContext context, Order order)
    {
        var result = new OrderMessage
        {
            WorkflowId = context.InstanceId,
            OrderId = order.OrderId,
            PizzaType = order.PizzaType,
            Size = order.Size,
            Customer = new Customer
            {
                Address = order.Customer.Address,
                Name = order.Customer.Name,
                Phone = order.Customer.Phone
            }
        };

        return result;
    }
}