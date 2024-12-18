using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wiseshare.Application.Common.Interfaces.Authentication;
using Wiseshare.Application.Repository;
using WiseShare.Application.Authentication;
using WiseShare.Application.Common.Interfaces.Services;
using WiseShare.Infrastructure.Authentication;
using WiseShare.Infrastructure.Persistence;
using WiseShare.Infrastructure.Persistence.Repositories;
using WiseShare.Infrastructure.Repository;
using WiseShare.Infrastructure.Services;

namespace WiseShare.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
         this IServiceCollection services,
         ConfigurationManager configuration)
    {
        // Use the in-memory repository for property (testing)
        services.AddSingleton<IPropertyRepository, InMemoryPropertyRepository>();

        // Use the database-backed repository for user (real DB)
        services.AddDbContext<WiseShareDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("WiseShareDatabase")));

        // Register user repository for DB interactions
        services.AddScoped<IUserRepository, UserRepository>();

        // Authentication and JWT - Change the lifetime to Scoped
        services.Configure<JwtSettings>(configuration.GetSection("jwtSettings"));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        //services.AddScoped<IAuthenticationService, AuthenticationService>(); 

        // DateTime Provider
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}