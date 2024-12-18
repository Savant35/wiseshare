namespace WiseShare.Api;

public static class PresentationDependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        return services;
    }
}
