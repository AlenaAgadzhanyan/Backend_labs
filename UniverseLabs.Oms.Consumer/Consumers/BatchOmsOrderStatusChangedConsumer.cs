using UniverseLabs.Oms.Consumer.Base;
using UniverseLabs.Oms.Consumer.Clients;
using UniverseLabs.Oms.Consumer.Config;
using UniverseLabs.Messages;
using Microsoft.Extensions.Options;
using UniverseLabs.Oms.Models.Dto.V1.Requests;
using UniverseLabs.Oms.Models.Enums;

namespace UniverseLabs.Oms.Consumer.Consumers;

public class BatchOmsOrderStatusChangedConsumer(IOptions<RabbitMqSettings> settings, IServiceProvider serviceProvider)
    : BaseBatchMessageConsumer<OmsOrderStatusChangedMessage>(settings.Value, s => s.OrderStatusChanged)
{
    protected override async Task ProcessMessages(OmsOrderStatusChangedMessage[] messages)
    {
        using var scope = serviceProvider.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<OmsClient>();
        
        // TODO: The message OmsOrderStatusChangedMessage does not contain the OldStatus.
        // It's necessary to update the message and the publisher to include the old status for a complete audit log.
        // Using null as a placeholder.
        var request = new V1AuditLogOrderRequest
        {
            Orders = messages.Select(x => new V1AuditLogOrderRequest.LogOrder
            {
                OrderId = x.OrderId,
                OldStatus = null, // Placeholder
                NewStatus = x.NewStatus
            }).ToArray()
        };
        
        await client.LogOrder(request, CancellationToken.None);
    }
}
