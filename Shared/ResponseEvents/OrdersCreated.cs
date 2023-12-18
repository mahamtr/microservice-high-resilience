
namespace SharedMessages.ResponseEvents;

public class OrdersCreated : ICommand
{
    public Guid OrderId { get; set; }
    public string? ShipmentOrderId { get; set; }
    public string? PurchaseOrderId { get; set; }
    public string MessageData { get; set; } = string.Empty;
}