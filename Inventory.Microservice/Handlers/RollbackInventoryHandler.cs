using MongoDB.Driver;
using SharedMessages;
using SharedMessages.Enums;
using SharedMessages.Events;
using SharedMessages.ResponseEvents;
using SharedMessages.SharedData;

namespace Inventory.Microservice.Handlers;

public class RollbackInventoryHandler(
    ILogger<RollbackInventoryHandler> logger,
    IMongoRepository<Data.Inventory> inventoryRepo,
    IConfiguration configuration) : IHandleMessages<RollbackInventory>
{
    private readonly ILogger<RollbackInventoryHandler> _logger = logger;
    private SendOptions Options { get; set; } = new();

    public async Task Handle(RollbackInventory message, IMessageHandlerContext context)
    {
        Options.SetDestination(configuration.GetSection("ApiGatewayEndpointName").Value + "-MAAI");
        try
        {
            var inventoryDetail = await inventoryRepo.GetAsync(message.InventoryId ?? "");
            if (inventoryDetail != null && message.InventoryId != null)
            {
                var newQuantity = inventoryDetail.Quantitiy + message.Quantity;
                inventoryDetail.Quantitiy = newQuantity;
                await inventoryRepo.UpdateAsync(message.InventoryId, inventoryDetail);
                await context.Send(new RollbackSuccess() { OrderId = message.OrderId, Step = RollbackTypes.InventoryRollback}, Options);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            await context.Send(new RollbackSuccess() { OrderId = message.OrderId, Step = RollbackTypes.InventoryRollback}, Options);
        }

    }
}