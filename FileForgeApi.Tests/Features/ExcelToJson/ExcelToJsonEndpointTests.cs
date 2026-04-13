using FileForgeApi.Features.ExcelToJson;
using FileForgeApi.Shared.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;

namespace FileForgeApi.Tests.Features.ExcelToJson;

public class ExcelToJsonEndpointTests : IAsyncDisposable
{
    private readonly IExcelToJsonService _service = Substitute.For<IExcelToJsonService>();
    private readonly IHost _host;
    private readonly HttpClient _client;

    public ExcelToJsonEndpointTests()
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
                        app.UseEndpoints(endpoints => endpoints.MapExcelToJson());
                    });
            })
            .Build();

        _host.Start();
        _client = _host.GetTestClient();
    }

    [Fact]
    public async Task Post_WithValidRequest_ReturnsOk()
    {
        var response = new ExcelToJsonResponse(new List<Dictionary<string, string>>
        {
            new() { ["Col1"] = "val1" }
        });
        _service.ConvertAsync(Arg.Any<ExcelToJsonRequest?>())
            .Returns(Task.FromResult(Result<ExcelToJsonResponse>.Success(response)));

        var httpResponse = await _client.PostAsJsonAsync(
            "/api/excel/to-json",
            new ExcelToJsonRequest("dGVzdA=="));

        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidRequest_ReturnsBadRequest()
    {
        _service.ConvertAsync(Arg.Any<ExcelToJsonRequest?>())
            .Returns(Task.FromResult(Result<ExcelToJsonResponse>.Failure("Error de validación")));

        var httpResponse = await _client.PostAsJsonAsync(
            "/api/excel/to-json",
            new ExcelToJsonRequest(""));

        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }
}
