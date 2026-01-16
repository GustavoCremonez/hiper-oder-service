using System.Reflection;
using Hiper.API.Middlewares;
using Hiper.Application.UseCases.CancelOrder;
using Hiper.Application.UseCases.CreateOrder;
using Hiper.Application.UseCases.GetOrderById;
using Hiper.Application.UseCases.GetOrders;
using Hiper.Application.UseCases.UpdateOrderStatus;
using Hiper.Infrastructure.Data;
using Hiper.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hiper Order System API",
        Version = "v1",
        Description = "API para gerenciamento de pedidos com arquitetura limpa e mensageria assíncrona",
        Contact = new OpenApiContact
        {
            Name = "Hiper Software SA",
            Email = "contato@hiper.com"
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<CreateOrderHandler>();
builder.Services.AddScoped<GetOrdersHandler>();
builder.Services.AddScoped<GetOrderByIdHandler>();
builder.Services.AddScoped<UpdateOrderStatusHandler>();
builder.Services.AddScoped<CancelOrderHandler>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
