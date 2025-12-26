namespace UniverseLabs.Messages;

public class OrderStatusChangedMessage : BaseMessage
{
    public override string RoutingKey => "order.status.changed";
    public long[] OrderIds { get; set; }
    public string NewStatus { get; set; }
}
