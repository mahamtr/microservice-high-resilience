using Api.Gateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Gateway.Controllers;


[Route("[controller]")]
public class OrdersController(ICommandService commandService) : Controller
{
    
    [HttpGet("[action]")]
    public async Task<string> PostPurchaseOrder()
    {
        var message = await commandService.PlaceOrder();
        return message;
    }
    
    
    [HttpGet("[action]")]
    public async Task<string> dummy()
    {
        return "message";
    }
}