
using SharedMessages.Enums;

namespace SharedMessages.ResponseEvents;

public class RollbackSuccess : IMessage
{
    public Guid OrderId { get; set; }
    public RollbackTypes Step { get; set; }
}