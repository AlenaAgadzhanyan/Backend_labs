using System.Diagnostics;
using System.Text.Json;
using Dapper;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.CompilerServices;
using UniverseLabs.Oms.BLL.Services;
using UniverseLabs.Oms.Config;
using UniverseLabs.Oms.DAL;
using UniverseLabs.Oms.DAL.Interfaces;
using UniverseLabs.Oms.DAL.Repositories;
using UniverseLabs.Oms.Validators;
using UniverseLabs.Common;
using UniverseLabs.Oms.Jobs;

var builder = WebApplication.CreateBuilder(args);

DefaultTypeMap.MatchNamesWithUnderscores = true;
builder.Services.AddScoped<UnitOfWork>();

builder.Services.Configure<DbSettings>(builder.Configuration.GetSection(nameof(DbSettings)));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));

builder.Services.AddScoped<IAuditLogOrderRepository, AuditLogOrderRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();

builder.Services.AddScoped<AuditLogOrderService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<RabbitMqService>();

builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));
builder.Services.AddScoped<ValidatorFactory>();

// зависимость, которая автоматически подхватывает все контроллеры в проекте
builder.Services.AddControllers().AddJsonOptions(options => 
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddControllers();
// добавляем swagger
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<OrderGenerator>();
// собираем билдер в приложение
var app = builder.Build();

// добавляем 2 миддлвари для обработки запросов в сваггер
app.UseSwagger();
app.UseSwaggerUI();

// добавляем миддлварю для роутинга в нужный контроллер
app.MapControllers();

// вместо *** должен быть путь к проекту Migrations
// по сути в этот момент будет происходить накатка миграций на базу
UniverseLabs.Oms.Migrations.Program.Main([]);

// запускам приложение
app.Run();