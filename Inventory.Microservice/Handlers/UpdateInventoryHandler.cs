using MongoDB.Driver;
using SharedMessages;
using SharedMessages.Enums;
using SharedMessages.Events;
using SharedMessages.ResponseEvents;
using SharedMessages.SharedData;

namespace Inventory.Microservice.Handlers;

public class UpdateInventoryHandler(
    ILogger<UpdateInventoryHandler> logger,
    IMongoRepository<Data.Inventory> inventoryRepo,
    IConfiguration configuration) : IHandleMessages<UpdateInventory>
{
    private readonly ILogger<UpdateInventoryHandler> _logger = logger;
    private SendOptions Options { get; set; } = new SendOptions();

    public async Task Handle(UpdateInventory message, IMessageHandlerContext context)
    {
        Options.SetDestination(configuration.GetSection("ApiGatewayEndpointName").Value+"-MAAI");
        var messageData = string.Empty;
        try
        {
            var inventoryDetail = await inventoryRepo.GetAsync(message.InventoryId ?? "");
            if (inventoryDetail != null && inventoryDetail.Quantitiy >= message.Quantity && message.InventoryId != null)
            {
                var newQuantity = inventoryDetail.Quantitiy - message.Quantity;
                inventoryDetail.Quantitiy = newQuantity;
                await inventoryRepo.UpdateAsync(message.InventoryId, inventoryDetail);

                messageData = "Inventory Updated SuccessFully";
                await context.Send(new InventoryUpdated { OrderId = message.OrderId, MessageData = messageData }, Options);
            }
            else if (inventoryDetail == null)
            {
                throw new Exception("Inventory Not Found");
            }
            else
            {
                throw new Exception("Inventory Quantity is not sufficient");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            messageData = e.Message;
            await context.Send(new RejectOrder { OrderId = message.OrderId, MessageData = messageData ,Failure = CreateOrderFailures.InventoryUpdateFailure}, Options);
        }

    }
}