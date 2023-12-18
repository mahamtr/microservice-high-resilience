
namespace SharedMessages.Events;

public class RollbackInventory : ICommand
{
    public Guid OrderId { get; set; }
    public string? InventoryId { get; set; }
    public int Quantity { get; set; }
}