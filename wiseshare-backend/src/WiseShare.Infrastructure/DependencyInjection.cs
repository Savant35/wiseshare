using EntityFramework.Exceptions.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wiseshare.Application.Common.Email;
using Wiseshare.Application.Common.Interfaces.Authentication;
using Wiseshare.Application.Common.Interfaces.Email;
using Wiseshare.Application.Common.Interfaces.Services;
using Wiseshare.Application.PropertyServices;
using Wiseshare.Application.Repository;
using Wiseshare.Application.services.InvestmentServices;
using Wiseshare.Application.services.PortfolioServices;
using Wiseshare.Application.services.UserServices;
using Wiseshare.Application.Services;
using Wiseshare.Infrastructure.Authentication;
using Wiseshare.Infrastructure.Email;
using Wiseshare.Infrastructure.Persistence;
using Wiseshare.Infrastructure.Persistence.Repositories;
using Wiseshare.Infrastructure.Services;

namespace Wiseshare.Infrastructure;

public static class DependencyInjection{
    public static IServiceCollection AddInfrastructure(
         this IServiceCollection services,
         ConfigurationManager configuration)
    {
        // Use the in-memory repository for property (testing)
        //services.AddSingleton<IPropertyRepository, InMemoryPropertyRepository>();

        // Use the database-backed repository for user (real DB)
        services.AddDbContext<WiseshareDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("WiseshareDatabase"))
            .UseExceptionProcessor());

        // Register user repository for DB interactions
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

         // Register property repository for DB interactions
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IPropertyService, PropertyService>();

         // Register wallet repository for DB interactions
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IWalletService, WalletService>();

         // Register portfolio repository for DB interactions
        services.AddScoped<IPortfolioRepository, PortfolioRepository>();
        services.AddScoped<IPortfolioService, PortfolioService>();

         // Register investment repository for DB interactions
        services.AddScoped<IInvestmentRepository, InvestmentRepository>();
        services.AddScoped<IInvestmentService, InvestmentService>();
        
        // Register payment repository for DB interactions
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPaymentService, PaymentService>();


        // Authentication and JWT - Change the lifetime to Scoped
        services.Configure<JwtSettings>(configuration.GetSection("jwtSettings"));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        //services.AddScoped<IAuthenticationService, AuthenticationService>(); 

        services.AddScoped<IEmailVerificationService, EmailVerificationService>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();


        // DateTime Provider
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}