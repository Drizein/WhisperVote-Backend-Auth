using Application.UseCases;
using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<UCRegister, UCRegister>();
        services.AddTransient<UCUserRole, UCUserRole>();
        services.AddTransient<UCLogin, UCLogin>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IIdentRepository, IdentRepository>();
        services.AddScoped<CDbContext, CDbContext>();
        return services;
    }
}