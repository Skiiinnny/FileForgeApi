namespace FileForgeApi.Features.JsonToExcel;

public static class DependencyInjection
{
    public static IServiceCollection AddJsonToExcel(this IServiceCollection services)
    {
        services.AddScoped<IJsonToExcelService, JsonToExcelService>();
        return services;
    }
}
