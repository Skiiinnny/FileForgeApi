using FileForgeApi.Features.CsvToJson;
using FileForgeApi.Shared.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;

namespace FileForgeApi.Tests.Features.CsvToJson;

public class CsvToJsonEndpointTests : IAsyncDisposable
{
    private readonly ICsvToJsonService _service = Substitute.For<ICsvToJsonService>();
    private readonly IHost _host;
    private readonly HttpClient _client;

    public CsvToJsonEndpointTests()
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
                        app.UseEndpoints(endpoints => endpoints.MapCsvToJson());
                    });
            })
            .Build();

        _host.Start();
        _client = _host.GetTestClient();
    }

    [Fact]
    public async Task Post_WithValidRequest_ReturnsOk()
    {
        var response = new CsvToJsonResponse(new List<Dictionary<string, string>>
        {
            new() { ["Col1"] = "val1" }
        });
        _service.ConvertAsync(Arg.Any<CsvToJsonRequest?>())
            .Returns(Task.FromResult(Result<CsvToJsonResponse>.Success(response)));

        var httpResponse = await _client.PostAsJsonAsync(
            "/api/csv/to-json",
            new CsvToJsonRequest("dGVzdA=="));

        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidRequest_ReturnsBadRequest()
    {
        _service.ConvertAsync(Arg.Any<CsvToJsonRequest?>())
            .Returns(Task.FromResult(Result<CsvToJsonResponse>.Failure("Error de validación")));

        var httpResponse = await _client.PostAsJsonAsync(
            "/api/csv/to-json",
            new CsvToJsonRequest(""));

        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }
}
