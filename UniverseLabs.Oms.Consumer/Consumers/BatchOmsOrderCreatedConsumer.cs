using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UniverseLabs.Messages;
using UniverseLabs.Oms.Consumer.Base;
using UniverseLabs.Oms.Consumer.Clients;
using UniverseLabs.Oms.Consumer.Config;
using UniverseLabs.Oms.Models.Dto.V1.Requests;
using UniverseLabs.Oms.Models.Enums;

namespace UniverseLabs.Oms.Consumer.Consumers;

public class BatchOmsOrderCreatedConsumer(
    IOptions<RabbitMqSettings> rabbitMqSettings,
    IServiceProvider serviceProvider)
    : BaseBatchMessageConsumer<OrderCreatedMessage>(rabbitMqSettings.Value, s => s.OrderCreated)
{
    private static int _counter = 0;

    protected override async Task ProcessMessages(OrderCreatedMessage[] messages)
    {
        _counter++;

        if (_counter % 5 == 0)
        {
            throw new Exception("Simulated processing failure for testing dead-letter queue.");
        }

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
                    OrderStatus = OrderStatus.Created
                })).ToArray()
        }, CancellationToken.None);
    }
}