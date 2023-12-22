
namespace SharedMessages.Events;

public class CreateOrders : ICommand
{
    public Guid OrderId { get; set; }
    
}