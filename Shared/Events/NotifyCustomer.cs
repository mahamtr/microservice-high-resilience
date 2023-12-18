
namespace SharedMessages.Events;

public class NotifyCustomer : ICommand
{
    public Guid OrderId { get; set; }
    
}