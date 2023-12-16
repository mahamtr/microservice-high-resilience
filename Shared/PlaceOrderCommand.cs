
namespace SharedMessages;

public class PlaceOrder : ICommand
{
    public Guid OrderId { get; set; }
}