using SharedMessages;

namespace Api.Gateway.Sagas;

public class PlaceOrderSaga(IMessageSession messageSession) : Saga<PlaceOrderSagaData>,
    IAmStartedByMessages<PlaceOrder>
{

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PlaceOrderSagaData> mapper)
    {
        mapper.MapSaga(sagaData => sagaData.OrderId)
            .ToMessage<PlaceOrder>(message => message.OrderId);
    }
    
    
    public async Task Handle(PlaceOrder message, IMessageHandlerContext context)
    {
       await messageSession.Send(new ReserveInventory() { OrderId = message.OrderId }, context.CancellationToken);
    }


}


public class PlaceOrderSagaData : ContainSagaData
{
    public Guid OrderId { get; set; }
}
