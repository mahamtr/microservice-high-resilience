
namespace SharedMessages.ResponseEvents;

public class EndOrderSuccess : IMessage
{
    public Guid OrderId { get; set; }
    public string MessageData { get; set; } = string.Empty;
}