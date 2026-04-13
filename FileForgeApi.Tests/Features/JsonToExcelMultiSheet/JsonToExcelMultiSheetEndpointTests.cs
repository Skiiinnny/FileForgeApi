using FileForgeApi.Features.JsonToExcelMultiSheet;
using FileForgeApi.Shared.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;

namespace FileForgeApi.Tests.Features.JsonToExcelMultiSheet;

public class JsonToExcelMultiSheetEndpointTests : IAsyncDisposable
{
    private readonly IJsonToExcelMultiSheetService _service = Substitute.For<IJsonToExcelMultiSheetService>();
    private readonly IHost _host;
    private readonly HttpClient _client;

    public JsonToExcelMultiSheetEndpointTests()
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
                        app.UseEndpoints(endpoints => endpoints.MapJsonToExcelMultiSheet());
                    });
            })
            .Build();

        _host.Start();
        _client = _host.GetTestClient();
    }

    [Fact]
    public async Task Post_WithValidRequest_ReturnsOk()
    {
        var response = new JsonToExcelMultiSheetResponse("dGVzdA==");
        _service.ConvertAsync(Arg.Any<JsonToExcelMultiSheetRequest?>())
            .Returns(Task.FromResult(Result<JsonToExcelMultiSheetResponse>.Success(response)));

        var request = new JsonToExcelMultiSheetRequest(new Dictionary<string, List<Dictionary<string, string>>>
        {
            ["Sheet1"] = [new Dictionary<string, string> { ["Col1"] = "val1" }]
        });
        var httpResponse = await _client.PostAsJsonAsync("/api/json/to-excel/multi-sheet", request);

        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidRequest_ReturnsBadRequest()
    {
        _service.ConvertAsync(Arg.Any<JsonToExcelMultiSheetRequest?>())
            .Returns(Task.FromResult(Result<JsonToExcelMultiSheetResponse>.Failure("Error de validación")));

        var request = new JsonToExcelMultiSheetRequest(new Dictionary<string, List<Dictionary<string, string>>>());
        var httpResponse = await _client.PostAsJsonAsync("/api/json/to-excel/multi-sheet", request);

        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }
}
