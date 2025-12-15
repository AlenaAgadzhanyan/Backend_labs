using UniverseLabs.Oms.Consumer.Base;
using UniverseLabs.Oms.Consumer.Clients;
using UniverseLabs.Oms.Consumer.Config;
using UniverseLabs.Messages;
using Microsoft.Extensions.Options;
using UniverseLabs.Oms.Models.Enums;
using UniverseLabs.Oms.Models.Dto.V1.Requests;

namespace UniverseLabs.Oms.Consumer.Consumers;

public class BatchOmsOrderCreatedConsumer(
    IOptions<RabbitMqSettings> rabbitMqSettings,
    IServiceProvider serviceProvider)
    : BaseBatchMessageConsumer<OrderCreatedMessage>(rabbitMqSettings.Value)
{
    protected override async Task ProcessMessages(OrderCreatedMessage[] messages)
    {
        using var scope = serviceProvider.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<OmsClient>();
        
        await client.LogOrder(new V1AuditLogOrderRequest
        {
            Orders = messages.SelectMany(order => order.OrderItems.Select(ol => 
                new V1AuditLogOrderRequest.LogOrder
                {
                    OrderId = order.Id,
                    OrderItemId = ol.Id,
                    CustomerId = order.CustomerId,
                    OrderStatus = nameof(OrderStatus.Created)
                })).ToArray()
        }, CancellationToken.None);
    }
}