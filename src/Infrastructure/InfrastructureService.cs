using Application.Contracts.Services;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureService
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddScoped<IAiService, GeminiAiService>();

        return services;
    }
}