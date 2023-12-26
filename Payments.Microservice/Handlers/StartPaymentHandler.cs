using MongoDB.Bson;
using Payments.Microservice.Data;
using SharedMessages.Enums;
using SharedMessages.Events;
using SharedMessages.ResponseEvents;
using SharedMessages.SharedData;

namespace Payments.Microservice.Handlers;

public class StartPaymentHandler(ILogger<StartPaymentHandler> logger,
    IMongoRepository<Payment> paymentRepo,
    IConfiguration configuration) : IHandleMessages<StartPayment>
{
    private readonly ILogger<StartPaymentHandler> _logger = logger;
    private  SendOptions Options { get; set; } = new SendOptions();

    public async Task Handle(StartPayment message, IMessageHandlerContext context)
    {
        Options.SetDestination(configuration.GetSection("ApiGatewayEndpointName").Value +"-MAAI");
        var messageData = string.Empty;
        var paymentTransactionId = string.Empty;

        try
        {
            var paymentId = ProcessPayment();
            var payment = new Payment{Id = paymentId.ToString(), DateTime = DateTime.UtcNow,Status = "Succeed"};
            paymentTransactionId = payment.Id;
            await paymentRepo.CreateAsync(payment);
            messageData = "Payment Succeed";
            await context.Send(new PaymentSucceed { OrderId = message.OrderId, MessageData = messageData ,PaymentOrderId = paymentTransactionId}, Options);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            messageData = e.Message;
            await context.Send(new RejectOrder { OrderId = message.OrderId, MessageData = messageData ,Failure = CreateOrderFailures.PaymentFailure}, Options);
        }
        
        
    }

    private ObjectId ProcessPayment()
    {
        #region simulate third-party failure

        // throw  new Exception( "Third-party payment provider failure");
        #endregion

        return ObjectId.GenerateNewId();
    }
}