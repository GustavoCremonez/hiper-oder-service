using Hiper.Application.Interfaces;
using Hiper.Domain.Interfaces;
using Hiper.Infrastructure.Data;
using Hiper.Infrastructure.Messaging;
using Hiper.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hiper.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMQ"));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

        services.AddHostedService<OrderCreatedConsumer>();

        return services;
    }
}
