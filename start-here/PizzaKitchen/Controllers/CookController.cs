using Microsoft.AspNetCore.Mvc;
using PizzaKitchen.Models;
using PizzaKitchen.Services;

namespace PizzaKitchen.Controllers;

[ApiController]
[Route("[controller]")]
public class CookController : ControllerBase
{
    private readonly ICookService _cookService;
    private readonly ILogger<CookController> _logger;

    public CookController(ICookService cookService, ILogger<CookController> logger)
    {
        _cookService = cookService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Cook(Order order)
    {
        _logger.LogInformation("Starting cooking for order: {OrderId}", order.OrderId);
        var result = await _cookService.CookPizzaAsync(order);
        return Ok(result);
    }
}