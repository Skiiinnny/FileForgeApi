using Amazon.Lambda.Serialization.SystemTextJson;
using FileForgeApi.Features.Base64ToCsv;
using FileForgeApi.Features.Base64ToExcel;
using FileForgeApi.Features.Base64ToJson;
using FileForgeApi.Features.CsvToExcel;
using FileForgeApi.Features.CsvToJson;
using FileForgeApi.Features.ExcelAppendRows;
using FileForgeApi.Features.ExcelFromTemplate;
using FileForgeApi.Features.ExcelMetadata;
using FileForgeApi.Features.ExcelToCsv;
using FileForgeApi.Features.ExcelToJson;
using FileForgeApi.Features.ExcelToJsonMultiSheet;
using FileForgeApi.Features.JsonToCsv;
using FileForgeApi.Features.JsonToExcel;
using FileForgeApi.Features.CsvToExcelMultiSheet;
using FileForgeApi.Features.JsonToExcelMultiSheet;
using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Middleware;
using FileForgeApi.Shared.Serialization;

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddHttpClient<UrlDocumentFetchService>()
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
        client.MaxResponseContentBufferSize = 50 * 1024 * 1024;
    });
builder.Services.AddScoped<IDocumentFetchService, UrlDocumentFetchService>();

builder.Services.AddBase64ToCsv();
builder.Services.AddBase64ToExcel();
builder.Services.AddBase64ToJson();
builder.Services.AddCsvToExcel();
builder.Services.AddCsvToExcelMultiSheet();
builder.Services.AddCsvToJson();
builder.Services.AddExcelAppendRows();
builder.Services.AddExcelFromTemplate();
builder.Services.AddExcelMetadata();
builder.Services.AddExcelToCsv();
builder.Services.AddExcelToJson();
builder.Services.AddExcelToJsonMultiSheet();
builder.Services.AddJsonToCsv();
builder.Services.AddJsonToExcel();
builder.Services.AddJsonToExcelMultiSheet();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

#if DEBUG
builder.WebHost.UseKestrelHttpsConfiguration();
builder.Services.AddRouting();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "FileForge API",
        Version = "v1",
        Description = "API para conversión y procesamiento de archivos"
    });
});
#endif

builder.Services.AddAWSLambdaHosting(
    LambdaEventSource.RestApi,
    new SourceGeneratorLambdaJsonSerializer<AppJsonSerializerContext>()
);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

app.UseExceptionHandler();

#if DEBUG
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "FileForge API v1");
    options.RoutePrefix = string.Empty;
});
#endif

app.MapBase64ToCsv();
app.MapBase64ToExcel();
app.MapBase64ToJson();
app.MapCsvToExcel();
app.MapCsvToExcelMultiSheet();
app.MapCsvToJson();
app.MapExcelAppendRows();
app.MapExcelFromTemplate();
app.MapExcelMetadata();
app.MapExcelToCsv();
app.MapExcelToJson();
app.MapExcelToJsonMultiSheet();
app.MapJsonToCsv();
app.MapJsonToExcel();
app.MapJsonToExcelMultiSheet();

app.Run();
