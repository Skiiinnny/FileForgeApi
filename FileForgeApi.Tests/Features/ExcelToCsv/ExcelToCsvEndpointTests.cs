using FileForgeApi.Features.ExcelToCsv;
using FileForgeApi.Shared.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;

namespace FileForgeApi.Tests.Features.ExcelToCsv;

public class ExcelToCsvEndpointTests : IAsyncDisposable
{
    private readonly IExcelToCsvService _service = Substitute.For<IExcelToCsvService>();
    private readonly IHost _host;
    private readonly HttpClient _client;

    public ExcelToCsvEndpointTests()
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
                        app.UseEndpoints(endpoints => endpoints.MapExcelToCsv());
                    });
            })
            .Build();

        _host.Start();
        _client = _host.GetTestClient();
    }

    [Fact]
    public async Task Post_WithValidRequest_ReturnsOk()
    {
        var response = new ExcelToCsvResponse("dGVzdA==");
        _service.ConvertAsync(Arg.Any<ExcelToCsvRequest?>())
            .Returns(Task.FromResult(Result<ExcelToCsvResponse>.Success(response)));

        var httpResponse = await _client.PostAsJsonAsync(
            "/api/excel/to-csv",
            new ExcelToCsvRequest("dGVzdA=="));

        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidRequest_ReturnsBadRequest()
    {
        _service.ConvertAsync(Arg.Any<ExcelToCsvRequest?>())
            .Returns(Task.FromResult(Result<ExcelToCsvResponse>.Failure("Error de validación")));

        var httpResponse = await _client.PostAsJsonAsync(
            "/api/excel/to-csv",
            new ExcelToCsvRequest(""));

        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }
}
