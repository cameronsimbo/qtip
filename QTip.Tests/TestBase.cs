using Microsoft.Extensions.DependencyInjection;
using QTip.Application.Abstractions;
using QTip.Infrastructure.Services;

namespace QTip.Tests;

public abstract class TestBase
{
    protected static ServiceProvider CreateServiceProvider(
        Action<IServiceCollection>? configureServices = null)
    {
        ServiceCollection services = new ServiceCollection();

        services.AddScoped<IEmailDetectionService, EmailDetectionService>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();

        if (configureServices is not null)
        {
            configureServices(services);
        }

        ServiceProvider provider = services.BuildServiceProvider();
        return provider;
    }
}