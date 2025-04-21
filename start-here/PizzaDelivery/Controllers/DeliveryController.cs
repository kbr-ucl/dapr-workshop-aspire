using Microsoft.AspNetCore.Mvc;
using PizzaDelivery.Models;
using PizzaDelivery.Services;

namespace PizzaDelivery.Controllers;

[ApiController]
[Route("[controller]")]
public class DeliveryController : ControllerBase
{
    private readonly IDeliveryService _deliveryService;
    private readonly ILogger<DeliveryController> _logger;

    public DeliveryController(IDeliveryService deliveryService, ILogger<DeliveryController> logger)
    {
        _deliveryService = deliveryService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Deliver(Order order)
    {
        _logger.LogInformation("Starting delivery for order: {OrderId}", order.OrderId);
        var result = await _deliveryService.DeliverPizzaAsync(order);
        return Ok(result);
    }
}