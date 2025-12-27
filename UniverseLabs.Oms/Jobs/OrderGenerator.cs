
using AutoFixture;
using UniverseLabs.Oms.BLL.Models;
using UniverseLabs.Oms.BLL.Services;
using UniverseLabs.Oms.Models.Enums;

namespace UniverseLabs.Oms.Jobs;

public class OrderGenerator(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly Random _random = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(5000, stoppingToken); 

        var fixture = new Fixture();
        using var scope = serviceProvider.CreateScope();
        var orderService = scope.ServiceProvider.GetRequiredService<OrderService>();

        while (!stoppingToken.IsCancellationRequested)
        {
            var orders = Enumerable.Range(1, 50)
                .Select(_ =>
                {
                    var orderItem = fixture.Build<OrderItemUnit>()
                        .With(x => x.PriceCurrency, "RUB")
                        .With(x => x.PriceCents, 1000)
                        .Create();

                    var order = fixture.Build<OrderUnit>()
                        .With(x => x.TotalPriceCurrency, "RUB")
                        .With(x => x.TotalPriceCents, 1000)
                        .With(x => x.OrderItems, [orderItem])
                        .Create();

                    return order;
                })
                .ToArray();

            await orderService.BatchInsert(orders, stoppingToken);

            var ordersToUpdateCount = _random.Next(orders.Length / 2);
            var ordersToUpdate =
                orders.OrderBy(x => _random.Next()).Take(ordersToUpdateCount).ToArray();

            if (ordersToUpdate.Length != 0)
            {
                var statuses = Enum.GetValues<OrderStatus>();
                await orderService.BatchUpdateStatus(ordersToUpdate.Select(x =>
                {
                    var randomStatus = statuses[_random.Next(statuses.Length)];
                    return (x.Id, randomStatus);
                }).ToArray(), stoppingToken);
            }

            await Task.Delay(250, stoppingToken);
        }
    }
}
