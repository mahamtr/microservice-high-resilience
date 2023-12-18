
namespace SharedMessages.Events;

public class StartOrder : ICommand
{
    public Guid OrderId { get; set; }
    
    public string? InventoryId { get; set; }
    public int Quantity { get; set; }
}