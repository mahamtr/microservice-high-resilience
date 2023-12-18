
namespace SharedMessages.Events;

public class StartPayment : ICommand
{
    public Guid OrderId { get; set; }
}