using FileForgeApi.Features.Base64ToExcel;
using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;

namespace FileForgeApi.Tests.Features.Base64ToExcel;

public class Base64ToExcelTests : IAsyncDisposable
{
    private readonly IDocumentFetchService _fetchService = Substitute.For<IDocumentFetchService>();
    private readonly IHost _host;
    private readonly HttpClient _client;

    public Base64ToExcelTests()
    {
        var fetchService = _fetchService;
        _host = new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddScoped<IBase64ToExcelService, Base64ToExcelService>();
                        services.AddSingleton(fetchService);
                        services.AddSingleton(NullLoggerFactory.Instance);
                        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
                        services.AddRouting();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => endpoints.MapBase64ToExcel());
                    });
            })
            .Build();

        _host.Start();
        _client = _host.GetTestClient();
    }

    // --- Endpoint tests (base64 flow) ---

    [Fact]
    public async Task Post_ValidBase64_WithoutFilename_ReturnsOkWithDefaultFilename()
    {
        var bytes = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
        var base64 = Convert.ToBase64String(bytes);

        var response = await _client.PostAsJsonAsync("/api/base64/to-excel", new { base64Content = base64 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("file.xlsx", response.Content.Headers.ContentDisposition?.FileName ?? "");
    }

    [Fact]
    public async Task Post_ValidBase64_WithCustomFilename_ReturnsOkWithCustomFilename()
    {
        var bytes = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
        var base64 = Convert.ToBase64String(bytes);

        var response = await _client.PostAsJsonAsync("/api/base64/to-excel", new { base64Content = base64, filename = "datos.xlsx" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("datos.xlsx", response.Content.Headers.ContentDisposition?.FileName ?? "");
    }

    [Fact]
    public async Task Post_EmptyBase64_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/base64/to-excel", new { base64Content = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_InvalidBase64_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/base64/to-excel", new { base64Content = "!!!not-base64!!!" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- Endpoint tests (documentUrl flow) ---

    [Fact]
    public async Task Post_ValidDocumentUrl_ReturnsOk()
    {
        var bytes = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
        _fetchService.FetchAsync("https://example.com/file.xlsx")
            .Returns(Result<byte[]>.Success(bytes));

        var response = await _client.PostAsJsonAsync("/api/base64/to-excel", new { documentUrl = "https://example.com/file.xlsx" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_BothFields_ReturnsBadRequest()
    {
        var bytes = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
        var response = await _client.PostAsJsonAsync("/api/base64/to-excel", new
        {
            base64Content = Convert.ToBase64String(bytes),
            documentUrl = "https://example.com/file.xlsx"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_NeitherField_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/base64/to-excel", new { filename = "test.xlsx" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_DocumentUrlFetchFails_ReturnsBadRequest()
    {
        _fetchService.FetchAsync(Arg.Any<string>())
            .Returns(Result<byte[]>.Failure("La URL retornó el código HTTP 404."));

        var response = await _client.PostAsJsonAsync("/api/base64/to-excel", new { documentUrl = "https://example.com/notfound.xlsx" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- Validator tests ---

    [Fact]
    public void Validator_NullRequest_ReturnsFailure()
    {
        var result = Base64ToExcelValidator.Validate(null);

        Assert.False(result.IsSuccess);
        Assert.Contains("obligatorio", result.Error!);
    }

    [Fact]
    public void Validator_EmptyBase64_ReturnsFailure()
    {
        var result = Base64ToExcelValidator.Validate(new Base64ToExcelRequest(""));

        Assert.False(result.IsSuccess);
        Assert.Contains("exactamente uno", result.Error!);
    }

    [Fact]
    public void Validator_InvalidBase64_ReturnsFailure()
    {
        var result = Base64ToExcelValidator.Validate(new Base64ToExcelRequest("!!!invalid!!!"));

        Assert.False(result.IsSuccess);
        Assert.Contains("Base64", result.Error!);
    }

    [Fact]
    public void Validator_ValidBase64_ReturnsSuccessWithBytes()
    {
        var bytes = new byte[] { 1, 2, 3, 4, 5 };
        var result = Base64ToExcelValidator.Validate(new Base64ToExcelRequest(Convert.ToBase64String(bytes)));

        Assert.True(result.IsSuccess);
        Assert.Equal(bytes, result.Value.FileBytes);
        Assert.False(result.Value.UseUrl);
    }

    [Fact]
    public void Validator_BothFields_ReturnsFailure()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var result = Base64ToExcelValidator.Validate(new Base64ToExcelRequest(Convert.ToBase64String(bytes), DocumentUrl: "https://example.com/f.xlsx"));

        Assert.False(result.IsSuccess);
        Assert.Contains("a la vez", result.Error!);
    }

    [Fact]
    public void Validator_ValidDocumentUrl_ReturnsSuccessWithUseUrl()
    {
        var result = Base64ToExcelValidator.Validate(new Base64ToExcelRequest(null, DocumentUrl: "https://example.com/file.xlsx"));

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.UseUrl);
    }

    [Fact]
    public void Validator_InvalidUrl_ReturnsFailure()
    {
        var result = Base64ToExcelValidator.Validate(new Base64ToExcelRequest(null, DocumentUrl: "not-a-url"));

        Assert.False(result.IsSuccess);
        Assert.Contains("URI", result.Error!);
    }

    // --- Service tests ---

    [Fact]
    public async Task Service_NullRequest_ReturnsBadRequest()
    {
        var logger = NullLogger<Base64ToExcelService>.Instance;
        var sut = new Base64ToExcelService(logger, _fetchService);

        var result = await sut.ConvertAsync(null);

        Assert.Equal(400, (result as Microsoft.AspNetCore.Http.IStatusCodeHttpResult)?.StatusCode);
    }

    [Fact]
    public async Task Service_ValidRequest_ReturnsFileResult()
    {
        var logger = NullLogger<Base64ToExcelService>.Instance;
        var sut = new Base64ToExcelService(logger, _fetchService);
        var bytes = new byte[] { 0x50, 0x4B, 0x03, 0x04 };

        var result = await sut.ConvertAsync(new Base64ToExcelRequest(Convert.ToBase64String(bytes)));

        Assert.IsAssignableFrom<IResult>(result);
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }
}
