using SharedMessages.Enums;
using SharedMessages.Events;
using SharedMessages.ResponseEvents;
using SharedMessages.Timeout;

namespace Api.Gateway.Sagas;

public class PlaceOrderSaga(IConfiguration configuration) : Saga<PlaceOrderSagaData>, IAmStartedByMessages<StartOrder>,
    IHandleMessages<InventoryUpdated>, IHandleMessages<PaymentSucceed>, IHandleMessages<OrdersCreated>,
    IHandleMessages<EndOrderSuccess>, IHandleMessages<RejectOrder>, IHandleMessages<RollbackSuccess>,IHandleTimeouts<CreateOrderTimeout>
{
    private SendOptions Options { get; set; } = new();

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PlaceOrderSagaData> mapper)
    {
        mapper.MapSaga(sagaData => sagaData.OrderId)
            .ToMessage<StartOrder>(message => message.OrderId)
            .ToMessage<InventoryUpdated>(message => message.OrderId)
            .ToMessage<PaymentSucceed>(message => message.OrderId)
            .ToMessage<OrdersCreated>(message => message.OrderId)
            .ToMessage<EndOrderSuccess>(message => message.OrderId)
            .ToMessage<RejectOrder>(message => message.OrderId)
            .ToMessage<RollbackSuccess>(message => message.OrderId)
            .ToMessage<CreateOrderTimeout>(message => message.OrderId);
    }

    public async Task Handle(StartOrder message, IMessageHandlerContext context)
    {
        Data.OrderId = message.OrderId;
        Data.InventoryId = message.InventoryId;
        Data.Quantity = message.Quantity;

        await RequestTimeout(context, TimeSpan.FromMinutes(1), new CreateOrderTimeout { OrderId = Data.OrderId });

        Options.SetDestination(configuration.GetSection("InventoryEndpointName").Value + "-MAAI");
        await context.Send(
            new UpdateInventory { OrderId = message.OrderId, Quantity = Data.Quantity, InventoryId = Data.InventoryId },
            Options);
    }

    public async Task Handle(InventoryUpdated message, IMessageHandlerContext context)
    {
        Data.MessageData = message.MessageData;

        Options.SetDestination(configuration.GetSection("PaymentsEndpointName").Value + "-MAAI");
        await context.Send(new StartPayment { OrderId = Data.OrderId }, Options);
    }

    public async Task Handle(PaymentSucceed message, IMessageHandlerContext context)
    {
        Data.PaymentId = message.PaymentOrderId;
        Data.MessageData = message.MessageData;

        Options.SetDestination(configuration.GetSection("OrdersEndpointName").Value + "-MAAI");
        await context.Send(new CreateOrders { OrderId = Data.OrderId }, Options);
    }

    public async Task Handle(OrdersCreated message, IMessageHandlerContext context)
    {
        Data.ShippingOrderId = message.ShipmentOrderId;
        Data.PurchaseOrderId = message.PurchaseOrderId;
        Data.MessageData = message.MessageData;

        Options.SetDestination(configuration.GetSection("NotificationsEndpointName").Value + "-MAAI");
        await context.Send(new NotifyCustomer { OrderId = Data.OrderId }, Options);
    }

    public async Task Handle(EndOrderSuccess message, IMessageHandlerContext context)
    {
        Data.HasResolved = true;
        Data.MessageData = message.MessageData;

        await ReplyToOriginator(context,
            new EndOrderSuccess { OrderId = Data.OrderId, MessageData = "Successful Request" });
        MarkAsComplete();
    }

    public async Task Timeout(CreateOrderTimeout message, IMessageHandlerContext context)
    {
        if (Data.HasResolved) return;
        await ReplyToOriginator(context,
            new EndOrderSuccess { OrderId = Data.OrderId, MessageData = "Timeout has been reached" });
        MarkAsComplete();
    }

    public async Task Handle(RejectOrder message, IMessageHandlerContext context)
    {
        Options.SetDestination(configuration.GetSection("InventoryEndpointName").Value + "-MAAI");
        switch (message.Failure)
        {
            case CreateOrderFailures.PaymentFailure:
                await context.Send(new RollbackInventory{OrderId = Data.OrderId, InventoryId = Data.InventoryId,Quantity = Data.Quantity},Options);
                break;
            case CreateOrderFailures.OrdersCreationFailure:
                await context.Send(new RollbackInventory {OrderId = Data.OrderId, InventoryId = Data.InventoryId,Quantity = Data.Quantity},Options);
                Options.SetDestination(configuration.GetSection("PaymentsEndpointName").Value + "-MAAI");
                await context.Send(new RollbackPayment {OrderId = Data.OrderId, PaymentOrderId = Data.PaymentId});
                break;
            default:
                await ReplyToOriginator(context,
                    new EndOrderSuccess
                    {
                        OrderId = Data.OrderId,
                        MessageData = "Order has been rejected with no rollback requeriment."
                    });
                MarkAsComplete();
                break;
        }
    }

    public async Task Handle(RollbackSuccess message, IMessageHandlerContext context)
    {
        var step = message.Step;
        if (step == RollbackTypes.InventoryRollback && Data.PaymentId == null)
        {
            await ReplyToOriginator(context,
                new EndOrderSuccess
                {
                    OrderId = Data.OrderId,
                    MessageData = "Order has been rejected and successfully rollback to inventory."
                });
            MarkAsComplete();
        }

        if (step == RollbackTypes.InventoryRollback)
        {
            Data.InventoryHasBeenRollback = true;
        }

        if (step == RollbackTypes.PaymentRollback)
        {
            Data.PaymentHasBeenRollback = true;
        }

        if (step == RollbackTypes.InventoryRollback && Data.PaymentHasBeenRollback)
        {
            await ReplyToOriginator(context,
                new EndOrderSuccess
                {
                    OrderId = Data.OrderId,
                    MessageData = "Order has been rejected and successfully rollback to inventory and payment."
                });
            MarkAsComplete();
        }

        if (step == RollbackTypes.PaymentRollback && Data.InventoryHasBeenRollback)
        {
            await ReplyToOriginator(context,
                new EndOrderSuccess
                {
                    OrderId = Data.OrderId,
                    MessageData = "Order has been rejected and successfully rollback to inventory and payment."
                });
            MarkAsComplete();
        }
    }
}

public class PlaceOrderSagaData : ContainSagaData
{
    public Guid OrderId { get; set; }
    public string? InventoryId { get; set; }
    public int Quantity { get; set; }
    public string? ShippingOrderId { get; set; }
    public string? PaymentId { get; set; }
    public string MessageData { get; set; } = string.Empty;
    public string? PurchaseOrderId { get; set; }
    public bool HasResolved { get; set; }
    public bool PaymentHasBeenRollback { get; set; }
    public bool InventoryHasBeenRollback { get; set; }
}