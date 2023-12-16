using Microsoft.AspNetCore.Mvc;
using SharedMessages;

namespace Api.Gateway.Controllers;


[Route("[controller]")]
public class OrdersController(IMessageSession messageSession) : Controller
{

    [HttpGet("[action]")]
    public async Task<int> PostPurchaseOrder()
    {
        var command = new PlaceOrderCommand { OrderId = Guid.NewGuid() };

        await messageSession.Send(command)
            .ConfigureAwait(false);
        return 1;
    }
}