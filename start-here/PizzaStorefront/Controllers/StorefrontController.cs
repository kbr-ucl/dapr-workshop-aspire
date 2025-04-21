using Microsoft.AspNetCore.Mvc;
using PizzaStorefront.Models;
using PizzaStorefront.Services;

namespace PizzaStorefront.Controllers;

[ApiController]
[Route("[controller]")]
public class StorefrontController : ControllerBase
{
    private readonly ILogger<StorefrontController> _logger;
    private readonly IStorefrontService _storefrontService;

    public StorefrontController(IStorefrontService storefrontService, ILogger<StorefrontController> logger)
    {
        _storefrontService = storefrontService;
        _logger = logger;
    }

    [HttpPost("order")]
    public async Task<ActionResult<Order>> CreateOrder(Order order)
    {
        _logger.LogInformation("Received new order: {OrderId}", order.OrderId);
        var result = await _storefrontService.ProcessOrderAsync(order);
        return Ok(result);
    }
}