
namespace SharedMessages;

public class PlaceOrderCommand : ICommand
{
    public Guid OrderId { get; set; }
}