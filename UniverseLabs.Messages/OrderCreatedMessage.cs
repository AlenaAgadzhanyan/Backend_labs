using UniverseLabs.Oms.Models.Dto.Common;

namespace UniverseLabs.Messages;

public class OrderCreatedMessage : BaseMessage
{
    public override string RoutingKey => "order.created";

    public long Id { get; set; }
    
    public long CustomerId { get; set; }

    public string DeliveryAddress { get; set; }

    public long TotalPriceCents { get; set; }

    public string TotalPriceCurrency { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }

    public OrderItemUnit[] OrderItems { get; set; }
}