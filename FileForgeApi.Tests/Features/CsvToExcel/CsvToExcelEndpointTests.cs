using FileForgeApi.Features.CsvToExcel;
using FileForgeApi.Shared.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;

namespace FileForgeApi.Tests.Features.CsvToExcel;

public class CsvToExcelEndpointTests : IAsyncDisposable
{
    private readonly ICsvToExcelService _service = Substitute.For<ICsvToExcelService>();
    private readonly IHost _host;
    private readonly HttpClient _client;

    public CsvToExcelEndpointTests()
    {
        _host = new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddSingleton(_service);
                        services.AddRouting();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => endpoints.MapCsvToExcel());
                    });
            })
            .Build();

        _host.Start();
        _client = _host.GetTestClient();
    }

    [Fact]
    public async Task Post_WithValidRequest_ReturnsOk()
    {
        var response = new CsvToExcelResponse("dGVzdA==");
        _service.ConvertAsync(Arg.Any<CsvToExcelRequest?>())
            .Returns(Task.FromResult(Result<CsvToExcelResponse>.Success(response)));

        var httpResponse = await _client.PostAsJsonAsync(
            "/api/csv/to-excel",
            new CsvToExcelRequest("dGVzdA=="));

        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidRequest_ReturnsBadRequest()
    {
        _service.ConvertAsync(Arg.Any<CsvToExcelRequest?>())
            .Returns(Task.FromResult(Result<CsvToExcelResponse>.Failure("Error de validación")));

        var httpResponse = await _client.PostAsJsonAsync(
            "/api/csv/to-excel",
            new CsvToExcelRequest(""));

        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }
}
