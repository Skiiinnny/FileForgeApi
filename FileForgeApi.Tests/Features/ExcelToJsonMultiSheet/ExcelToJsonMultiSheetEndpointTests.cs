using FileForgeApi.Features.ExcelToJsonMultiSheet;
using FileForgeApi.Shared.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;

namespace FileForgeApi.Tests.Features.ExcelToJsonMultiSheet;

public class ExcelToJsonMultiSheetEndpointTests : IAsyncDisposable
{
    private readonly IExcelToJsonMultiSheetService _service = Substitute.For<IExcelToJsonMultiSheetService>();
    private readonly IHost _host;
    private readonly HttpClient _client;

    public ExcelToJsonMultiSheetEndpointTests()
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
                        app.UseEndpoints(endpoints => endpoints.MapExcelToJsonMultiSheet());
                    });
            })
            .Build();

        _host.Start();
        _client = _host.GetTestClient();
    }

    [Fact]
    public async Task Post_WithValidRequest_ReturnsOk()
    {
        var response = new ExcelToJsonMultiSheetResponse(new Dictionary<string, List<Dictionary<string, string>>>
        {
            ["Sheet1"] = [new Dictionary<string, string> { ["Col1"] = "val1" }]
        });
        _service.ConvertAsync(Arg.Any<ExcelToJsonMultiSheetRequest?>())
            .Returns(Task.FromResult(Result<ExcelToJsonMultiSheetResponse>.Success(response)));

        var httpResponse = await _client.PostAsJsonAsync(
            "/api/excel/to-json/multi-sheet",
            new ExcelToJsonMultiSheetRequest("dGVzdA=="));

        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidRequest_ReturnsBadRequest()
    {
        _service.ConvertAsync(Arg.Any<ExcelToJsonMultiSheetRequest?>())
            .Returns(Task.FromResult(Result<ExcelToJsonMultiSheetResponse>.Failure("Error de validación")));

        var httpResponse = await _client.PostAsJsonAsync(
            "/api/excel/to-json/multi-sheet",
            new ExcelToJsonMultiSheetRequest(""));

        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }
}
