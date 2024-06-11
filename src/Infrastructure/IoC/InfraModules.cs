using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Shop.Infrastructure.Database;
using Shop.Infrastructure.MessageBroker;
using Shop.Infrastructure.MessageBroker.Models;

namespace Shop.Infrastructure.IoC;

public static class InfraModules
{
    public static void AddInfraModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Database"));
        });
    }

    public static void AddSwaggerDocs(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Bearer token here",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
            });
        });
    }

    public static void AddSwaggerEndpoints(this WebApplication app)
    {
        app.UseSwagger();

        app.UseSwaggerUI();
    }

    public static void AddMassTransitServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IConsumer, ProductConsumer>();

        services.AddOptions<MassTransitHostOptions>();

        services.Configure<MessageBrokerSettings>(
            configuration.GetSection("MessageBroker")
        );

        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<MessageBrokerSettings>>().Value);

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumer<ProductConsumer>();

            busConfigurator.UsingRabbitMq((context, configurator) =>
            {
                MessageBrokerSettings settings = context.GetRequiredService<MessageBrokerSettings>();

                configurator.Host(new Uri(settings.Host), h =>
                {
                    h.Username(settings.Username);
                    h.Password(settings.Password);
                });

                configurator.ReceiveEndpoint("product-queue", e => e.ConfigureConsumer<ProductConsumer>(context));
            });
        });
    }
}
