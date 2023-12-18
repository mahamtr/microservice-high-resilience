
namespace SharedMessages.Events;

public class RollbackPayment : ICommand
{
    public Guid OrderId { get; set; }
    public string? PaymentOrderId { get; set; }
}