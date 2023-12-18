using MongoDB.Bson;
using Payments.Microservice.Data;
using SharedMessages.Enums;
using SharedMessages.Events;
using SharedMessages.ResponseEvents;
using SharedMessages.SharedData;

namespace Payments.Microservice.Handlers;

public class RollbackPaymentHandler(ILogger<RollbackPaymentHandler> logger,
    IMongoRepository<Payment> paymentRepo,
    IConfiguration configuration) : IHandleMessages<RollbackPayment>
{
    private readonly ILogger<RollbackPaymentHandler> _logger = logger;
    private  SendOptions Options { get; set; } = new SendOptions();

    public async Task Handle(RollbackPayment message, IMessageHandlerContext context)
    {
        Options.SetDestination(configuration.GetSection("OrchestratorEndpointName").Value);
   
        try
        {
            var payment = await paymentRepo.GetAsync(message.PaymentOrderId);
            payment.Status = "Refund";
            await paymentRepo.UpdateAsync(message.PaymentOrderId,payment);
            await context.Send(new RollbackSuccess() { OrderId = message.OrderId, Step = RollbackTypes.PaymentRollback}, Options);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            await context.Send(new RollbackSuccess() { OrderId = message.OrderId, Step = RollbackTypes.PaymentRollback}, Options);
        }
        
        
    }

}