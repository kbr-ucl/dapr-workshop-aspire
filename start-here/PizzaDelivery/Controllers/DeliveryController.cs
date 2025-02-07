using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using PizzaDelivery.Services;
using PizzaShared.Messages.Delivery;

namespace PizzaDelivery.Controllers;

[ApiController]
[Route("[controller]")]
public class DeliveryController : ControllerBase
{
    private readonly DaprClient _daprClient;
    private readonly IDeliveryService _deliveryService;
    private readonly ILogger<DeliveryController> _logger;

    public DeliveryController(IDeliveryService deliveryService, ILogger<DeliveryController> logger,
        DaprClient daprClient)
    {
        _deliveryService = deliveryService;
        _logger = logger;
        _daprClient = daprClient;
    }

    [Topic("pizzapubsub", "delivery")]
    public async Task<IActionResult> Deliver(DeliverMessage deliverMessage)
    {
        _logger.LogInformation("Starting delivery for order: {OrderId}", deliverMessage.OrderId);
        var result = await _deliveryService.DeliverPizzaAsync(deliverMessage);

        await _daprClient.PublishEventAsync("pizzapubsub", "workflow-delivery", result);

        return Ok();
    }
}