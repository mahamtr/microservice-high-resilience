using SharedMessages;

namespace Inventory.Microservice.Handlers;

public class PlaceOrderCommandHandler(ILogger<PlaceOrderCommandHandler> logger) : IHandleMessages<ReserveInventory>
{
    private readonly ILogger<PlaceOrderCommandHandler> _logger = logger;

    public  Task Handle(ReserveInventory message, IMessageHandlerContext context)
    {
        
        return Task.CompletedTask;
    }
}