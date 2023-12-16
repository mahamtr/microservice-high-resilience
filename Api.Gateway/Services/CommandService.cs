using SharedMessages;

namespace Api.Gateway.Services;

public class CommandService(IMessageSession messageSession) : ICommandService
{

    public async Task<Guid?> PlaceOrder()
    {
        var command = new PlaceOrder { OrderId = Guid.NewGuid() };

        await messageSession.Send(command)
            .ConfigureAwait(false);


        return Guid.NewGuid();


    }
}