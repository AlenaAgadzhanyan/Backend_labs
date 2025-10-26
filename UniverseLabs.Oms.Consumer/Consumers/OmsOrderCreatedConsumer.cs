using System.Text;
using UniverseLabs.Oms.Consumer.Clients;
using UniverseLabs.Oms.Consumer.Config;
using Microsoft.Extensions.Options;
using UniverseLabs.Oms.Models.Dto.V1.Requests;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UniverseLabs.Common;
using UniverseLabs.Messages;
using Microsoft.Extensions.Logging;
using System;

namespace UniverseLabs.Oms.Consumer.Consumers;

public class OmsOrderCreatedConsumer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<RabbitMqSettings> _rabbitMqSettings;
    private readonly ILogger<OmsOrderCreatedConsumer> _logger;
    private readonly ConnectionFactory _factory;
    private IConnection _connection;
    private IChannel _channel;
    private AsyncEventingBasicConsumer _consumer;
    
    public enum OrderStatus
    {
        Created,
        Processing,
        Completed,
        Cancelled
    }
    
    public OmsOrderCreatedConsumer(IOptions<RabbitMqSettings> rabbitMqSettings, IServiceProvider serviceProvider, ILogger<OmsOrderCreatedConsumer> logger)
    {
        _rabbitMqSettings = rabbitMqSettings;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _factory = new ConnectionFactory { HostName = rabbitMqSettings.Value.HostName, Port = rabbitMqSettings.Value.Port };
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("OmsOrderCreatedConsumer starting.");
        try
        {
            _connection = await _factory.CreateConnectionAsync(cancellationToken);
            _logger.LogInformation("RabbitMQ connection established.");
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("RabbitMQ channel created.");

            await _channel.QueueDeclareAsync(
                queue: _rabbitMqSettings.Value.OrderCreatedQueue, 
                durable: false, 
                exclusive: false,
                autoDelete: false,
                arguments: null, 
                cancellationToken: cancellationToken);
            _logger.LogInformation("Queue '{QueueName}' declared.", _rabbitMqSettings.Value.OrderCreatedQueue);

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.ReceivedAsync += async (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Received message: {MessagePayload}", message);
                var order = message.FromJson<OrderCreatedMessage>();

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var client = scope.ServiceProvider.GetRequiredService<OmsClient>();
                    _logger.LogInformation("Calling LogOrder for OrderId: {OrderId}", order.Id);
                    await client.LogOrder(new V1AuditLogOrderRequest
                    {
                        Orders = order.OrderItems.Select(x => 
                            new V1AuditLogOrderRequest.LogOrder
                            {
                                OrderId = order.Id,
                                OrderItemId = x.Id,
                                CustomerId = order.CustomerId,
                                OrderStatus = nameof(OrderStatus.Created)
                            }).ToArray()
                    }, CancellationToken.None);
                    _logger.LogInformation("Successfully processed LogOrder for OrderId: {OrderId}", order.Id);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error processing message for order id {OrderId}", order.Id);
                }
            };
            
            await _channel.BasicConsumeAsync(
                queue: _rabbitMqSettings.Value.OrderCreatedQueue, 
                autoAck: true, 
                consumer: _consumer,
                cancellationToken: cancellationToken);
            _logger.LogInformation("Consumer started and waiting for messages on queue '{QueueName}'.", _rabbitMqSettings.Value.OrderCreatedQueue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while starting the OmsOrderCreatedConsumer.");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("OmsOrderCreatedConsumer stopping.");
        await Task.CompletedTask;
        _connection?.Dispose();
        _channel?.Dispose();
    }
}