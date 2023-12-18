
namespace SharedMessages.ResponseEvents;

public class PaymentSucceed : ICommand
{
    public Guid OrderId { get; set; }
    public string? PaymentOrderId { get; set; }
    public string? MessageData { get; set; }
}