using Microsoft.Extensions.DependencyInjection;
using Wiseshare.Application.Authentication;
using Wiseshare.Application.PropertyServices;
using Wiseshare.Application.services.InvestmentServices;
using Wiseshare.Application.services.PortfolioServices;
using Wiseshare.Application.Services;

namespace Wiseshare.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        //services.AddSingleton<IPropertyService, PropertyService>();
        services.AddScoped<IPropertyService, PropertyService>();

        services.AddScoped<IWalletService, WalletService>();

        services.AddScoped<IPortfolioService, PortfolioService>();

        services.AddScoped<IInvestmentService, InvestmentService>();
        return services;
    }
}
