using UniverseLabs.Oms.Models.Enums;

namespace UniverseLabs.Messages
{
    public class OmsOrderStatusChangedMessage : BaseMessage
    {
        public override string RoutingKey => "order.status.changed";

        public long OrderId { get; set; }

        public OrderStatus NewStatus { get; set; }
    }
}
