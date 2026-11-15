using Microsoft.Extensions.DependencyInjection;
using MinhaEstante.Application.Services;

namespace MinhaEstante.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<LivroService>();
        services.AddScoped<EstatisticasService>();
        return services;
    }
}
