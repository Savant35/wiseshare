using Microsoft.Extensions.DependencyInjection;
using Wiseshare.Application.services.PortfolioServices;
using Wiseshare.Application.Services;
using WiseShare.Application.Authentication;

namespace WiseShare.Application;

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
        return services;
    }
}
