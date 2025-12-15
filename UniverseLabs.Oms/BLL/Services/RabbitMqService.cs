using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using UniverseLabs.Oms.Config;
using UniverseLabs.Common;

namespace UniverseLabs.Oms.BLL.Services;
public class RabbitMqService(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqService> logger)
{
    private readonly ConnectionFactory _factory = new() { HostName = settings.Value.HostName, Port = settings.Value.Port };
    private readonly ILogger<RabbitMqService> _logger = logger;

    public async Task Publish<T>(IEnumerable<T> enumerable, string queue, CancellationToken token)
    {
        _logger.LogInformation("Attempting to publish to queue: {QueueName}", queue);

        await using var connection = await _factory.CreateConnectionAsync(token);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: token);
        await channel.QueueDeclareAsync(
            queue: queue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: token);

        foreach (var message in enumerable)
        {
            var messageStr = message.ToJson();
            _logger.LogInformation("Publishing message: {MessagePayload}", messageStr);
            var body = Encoding.UTF8.GetBytes(messageStr);
            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queue,
                body: body,
                cancellationToken: token);
        }
        _logger.LogInformation("Successfully published messages to queue: {QueueName}", queue);
    }
}