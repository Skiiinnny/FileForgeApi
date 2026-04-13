namespace FileForgeApi.Features.Base64ToCsv;

public static class DependencyInjection
{
    public static IServiceCollection AddBase64ToCsv(this IServiceCollection services)
    {
        services.AddScoped<IBase64ToCsvService, Base64ToCsvService>();
        return services;
    }
}
