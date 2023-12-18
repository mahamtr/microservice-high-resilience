
namespace SharedMessages.ResponseEvents;

public class InventoryUpdated : ICommand
{
    public Guid OrderId { get; set; }
    public string MessageData { get; set; } = string.Empty;
}