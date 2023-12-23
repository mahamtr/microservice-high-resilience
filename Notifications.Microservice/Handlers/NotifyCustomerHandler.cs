using SharedMessages.Events;
using SharedMessages.ResponseEvents;

namespace Notifications.Microservice.Handlers;

public class NotifyCustomerHandler(ILogger<NotifyCustomerHandler> logger, IConfiguration configuration) : IHandleMessages<NotifyCustomer>
{
    private readonly ILogger<NotifyCustomerHandler> _logger = logger;
    private SendOptions Options { get; set; } = new SendOptions();

    public async Task Handle(NotifyCustomer message, IMessageHandlerContext context)
    {
        Options.SetDestination(configuration.GetSection("ApiGatewayEndpointName").Value);
        var messageData = string.Empty;
        try
        {
            if (NotifyCustomer())
            {
                messageData = "Notification was sucessful";
                await context.Send(new EndOrderSuccess { OrderId = message.OrderId, MessageData = messageData }, Options);
            }
            else
            {
            }
        }
        catch (Exception e)
        {
            logger.LogInformation(e.Message);
            messageData = e.Message;
        }

    }

    public bool NotifyCustomer()
    {
        #region simulate third-party notification service failure

        // throw new Exception("Third-party Notification service failure");

        #endregion

        return true;
    }
}