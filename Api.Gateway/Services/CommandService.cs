using SharedMessages;
using SharedMessages.Events;
using SharedMessages.ResponseEvents;

namespace Api.Gateway.Services;

public class CommandService(IMessageSession messageSession, IConfiguration configuration) : ICommandService
{

    public async Task<string> PlaceOrder()
    {
        var options = new SendOptions();
        options.SetDestination(configuration.GetSection("OrchestratorEndpointName").Value);
        var command = new StartOrder { OrderId = Guid.NewGuid(), Quantity = 1, InventoryId = "657de96832293e22c6012809"};

        var response = await messageSession.Request<EndOrderSuccess>(command,options);


        return response.MessageData;


    }
}