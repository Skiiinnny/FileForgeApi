namespace FileForgeApi.Features.Base64ToJson;

public static class DependencyInjection
{
    public static IServiceCollection AddBase64ToJson(this IServiceCollection services)
    {
        services.AddScoped<IBase64ToJsonService, Base64ToJsonService>();
        return services;
    }
}
