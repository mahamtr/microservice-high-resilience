
using SharedMessages.Enums;

namespace SharedMessages.Events;

public class RejectOrder : ICommand
{
    public Guid OrderId { get; set; }

    public string MessageData { get; set; } = string.Empty;
    public string PaymentOrderId { get; set; }
    public CreateOrderFailures Failure { get; set; }
}