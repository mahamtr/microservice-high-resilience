using SharedMessages;

namespace Inventory.Microservice.Handlers;

public class PlaceOrderCommandHandler(ILogger<PlaceOrderCommandHandler> logger) : IHandleMessages<PlaceOrderCommand>
{
    private readonly ILogger<PlaceOrderCommandHandler> _logger = logger;

    public  Task Handle(PlaceOrderCommand message, IMessageHandlerContext context)
    {
        _logger.LogInformation(message.OrderId + " _ OrderId");
        return Task.CompletedTask;
    }
}