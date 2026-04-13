using FileForgeApi.Features.JsonToCsv;
using FileForgeApi.Shared.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;

namespace FileForgeApi.Tests.Features.JsonToCsv;

public class JsonToCsvEndpointTests : IAsyncDisposable
{
    private readonly IJsonToCsvService _service = Substitute.For<IJsonToCsvService>();
    private readonly IHost _host;
    private readonly HttpClient _client;

    public JsonToCsvEndpointTests()
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
                        app.UseEndpoints(endpoints => endpoints.MapJsonToCsv());
                    });
            })
            .Build();

        _host.Start();
        _client = _host.GetTestClient();
    }

    [Fact]
    public async Task Post_WithValidRequest_ReturnsOk()
    {
        var response = new JsonToCsvResponse("dGVzdA==");
        _service.ConvertAsync(Arg.Any<JsonToCsvRequest?>())
            .Returns(Task.FromResult(Result<JsonToCsvResponse>.Success(response)));

        var httpResponse = await _client.PostAsJsonAsync(
            "/api/json/to-csv",
            new JsonToCsvRequest([new Dictionary<string, string> { ["Col1"] = "val1" }]));

        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidRequest_ReturnsBadRequest()
    {
        _service.ConvertAsync(Arg.Any<JsonToCsvRequest?>())
            .Returns(Task.FromResult(Result<JsonToCsvResponse>.Failure("Error de validación")));

        var httpResponse = await _client.PostAsJsonAsync(
            "/api/json/to-csv",
            new JsonToCsvRequest([]));

        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }
}
