namespace FileForgeApi.Features.JsonToCsv;

public static class DependencyInjection
{
    public static IServiceCollection AddJsonToCsv(this IServiceCollection services)
    {
        services.AddScoped<IJsonToCsvService, JsonToCsvService>();
        return services;
    }
}
