using Orders.Microservice.Data;
using SharedMessages.Enums;
using SharedMessages.Events;
using SharedMessages.ResponseEvents;
using SharedMessages.SharedData;

namespace Orders.Microservice.Handlers;

public class CreateOrdersHandler(ILogger<CreateOrdersHandler> logger,
    IMongoRepository<Order> orderRepo,
    IConfiguration configuration) : IHandleMessages<CreateOrders>
{
    private readonly ILogger<CreateOrdersHandler> _logger = logger;
    private SendOptions Options { get; set; } = new SendOptions();

    public async Task Handle(CreateOrders message, IMessageHandlerContext context)
    {
        Options.SetDestination(configuration.GetSection("OrchestratorEndpointName").Value);
        var messageData = string.Empty;
        var purchaseOrderId = string.Empty;
        var shippingOrderId = string.Empty;
        
        try
        {
            var purchaseOrder = new Order { Status = "Succeed",DateTime = DateTime.UtcNow,Type = "Purchase"};
            var shippingOrder = new Order { Status = "Succeed",DateTime = DateTime.UtcNow,Type = "Shipping"};
            await orderRepo.CreateAsync(purchaseOrder);
            await orderRepo.CreateAsync(shippingOrder);
            await context.Send(new OrdersCreated
            { OrderId = message.OrderId,
                PurchaseOrderId = purchaseOrderId,
                ShipmentOrderId  = shippingOrderId,
                MessageData = messageData
            }, Options);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            messageData = e.Message;
            await context.Send(new RejectOrder()
            { OrderId = message.OrderId,
                MessageData = messageData,
                Failure = CreateOrderFailures.OrdersCreationFailure
            }, Options);
        }

     
    }
}