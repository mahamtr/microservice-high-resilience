
namespace SharedMessages;

public class ReserveInventory : ICommand
{
    public Guid OrderId { get; set; }
}