
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Infrastructure.Data;
using MinhaEstante.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinhaEstante.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<MinhaEstanteDbContext>(
            options => options.UseSqlServer(connectionString));
        services.AddScoped<ILivroRepository, LivroRepository>();
        return services;
    }

    public static void AplicarMigrations(this IServiceProvider services)
    {
        using var escopo = services.CreateScope();
        var contexto = escopo.ServiceProvider.GetRequiredService<MinhaEstanteDbContext>();
        contexto.Database.Migrate();
    }
}
