using Api.Gateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Gateway.Controllers;


[Route("[controller]")]
public class OrdersController(ICommandService commandService) : Controller
{
    
    [HttpGet("[action]")]
    public async Task<Guid?> PostPurchaseOrder()
    {
        var orderId = await commandService.PlaceOrder();
        return orderId;
    }
}