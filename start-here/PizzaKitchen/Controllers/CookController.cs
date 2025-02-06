using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using PizzaKitchen.Services;
using PizzaShared.Messages.StoreFront;

namespace PizzaKitchen.Controllers;

[ApiController]
[Route("[controller]")]
public class CookController : ControllerBase
{
    private readonly ICookService _cookService;
    private readonly DaprClient _daprClient;
    private readonly ILogger<CookController> _logger;

    public CookController(ICookService cookService, ILogger<CookController> logger, DaprClient daprClient)
    {
        _cookService = cookService;
        _logger = logger;
        _daprClient = daprClient;
    }

    [Topic("pizzapubsub", "storefront")]
    public async Task<IActionResult> Cook(OrderMessage order)
    {
        _logger.LogInformation("Starting cooking for order: {OrderId}", order.OrderId);
        var result = await _cookService.CookPizzaAsync(order);

        await _daprClient.PublishEventAsync("pizzapubsub", "workflow", result);

        return Ok();
    }
}