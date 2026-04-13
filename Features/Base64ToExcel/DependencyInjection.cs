namespace FileForgeApi.Features.Base64ToExcel;

public static class DependencyInjection
{
    public static IServiceCollection AddBase64ToExcel(this IServiceCollection services)
    {
        services.AddScoped<IBase64ToExcelService, Base64ToExcelService>();
        return services;
    }
}
