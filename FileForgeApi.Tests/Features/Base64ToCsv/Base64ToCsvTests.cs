using FileForgeApi.Features.Base64ToCsv;
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

namespace FileForgeApi.Tests.Features.Base64ToCsv;

public class Base64ToCsvTests : IAsyncDisposable
{
    private readonly IDocumentFetchService _fetchService = Substitute.For<IDocumentFetchService>();
    private readonly IHost _host;
    private readonly HttpClient _client;

    public Base64ToCsvTests()
    {
        var fetchService = _fetchService;
        _host = new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddScoped<IBase64ToCsvService, Base64ToCsvService>();
                        services.AddSingleton(fetchService);
                        services.AddSingleton(NullLoggerFactory.Instance);
                        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
                        services.AddRouting();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => endpoints.MapBase64ToCsv());
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
        var csvBytes = "Name,Age\nAlice,30\n"u8.ToArray();
        var base64 = Convert.ToBase64String(csvBytes);

        var response = await _client.PostAsJsonAsync("/api/base64/to-csv", new { base64Content = base64 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/csv", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("file.csv", response.Content.Headers.ContentDisposition?.FileName ?? "");
    }

    [Fact]
    public async Task Post_ValidBase64_WithCustomFilename_ReturnsOkWithCustomFilename()
    {
        var csvBytes = "Name,Age\nBob,25\n"u8.ToArray();
        var base64 = Convert.ToBase64String(csvBytes);

        var response = await _client.PostAsJsonAsync("/api/base64/to-csv", new { base64Content = base64, filename = "reporte.csv" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("reporte.csv", response.Content.Headers.ContentDisposition?.FileName ?? "");
    }

    [Fact]
    public async Task Post_EmptyBase64_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/base64/to-csv", new { base64Content = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_InvalidBase64_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/base64/to-csv", new { base64Content = "!!!not-base64!!!" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- Endpoint tests (documentUrl flow) ---

    [Fact]
    public async Task Post_ValidDocumentUrl_ReturnsOk()
    {
        var csvBytes = "Name,Age\nAlice,30\n"u8.ToArray();
        _fetchService.FetchAsync("https://example.com/file.csv")
            .Returns(Result<byte[]>.Success(csvBytes));

        var response = await _client.PostAsJsonAsync("/api/base64/to-csv", new { documentUrl = "https://example.com/file.csv" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_BothFieldsPresent_ReturnsBadRequest()
    {
        var csvBytes = "Name,Age\n"u8.ToArray();
        var response = await _client.PostAsJsonAsync("/api/base64/to-csv", new
        {
            base64Content = Convert.ToBase64String(csvBytes),
            documentUrl = "https://example.com/file.csv"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_NeitherFieldPresent_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/base64/to-csv", new { filename = "test.csv" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_DocumentUrlFetchFails_ReturnsBadRequest()
    {
        _fetchService.FetchAsync(Arg.Any<string>())
            .Returns(Result<byte[]>.Failure("La URL retornó el código HTTP 404."));

        var response = await _client.PostAsJsonAsync("/api/base64/to-csv", new { documentUrl = "https://example.com/notfound.csv" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- Validator tests ---

    [Fact]
    public void Validator_NullRequest_ReturnsFailure()
    {
        var result = Base64ToCsvValidator.Validate(null);

        Assert.False(result.IsSuccess);
        Assert.Contains("obligatorio", result.Error!);
    }

    [Fact]
    public void Validator_EmptyBase64_ReturnsFailure()
    {
        var result = Base64ToCsvValidator.Validate(new Base64ToCsvRequest(""));

        Assert.False(result.IsSuccess);
        Assert.Contains("exactamente uno", result.Error!);
    }

    [Fact]
    public void Validator_InvalidBase64_ReturnsFailure()
    {
        var result = Base64ToCsvValidator.Validate(new Base64ToCsvRequest("!!!invalid!!!"));

        Assert.False(result.IsSuccess);
        Assert.Contains("Base64", result.Error!);
    }

    [Fact]
    public void Validator_ValidBase64_ReturnsSuccessWithBytes()
    {
        var bytes = "hello,world\n"u8.ToArray();
        var result = Base64ToCsvValidator.Validate(new Base64ToCsvRequest(Convert.ToBase64String(bytes)));

        Assert.True(result.IsSuccess);
        Assert.Equal(bytes, result.Value.FileBytes);
        Assert.False(result.Value.UseUrl);
    }

    [Fact]
    public void Validator_BothFields_ReturnsFailure()
    {
        var bytes = "hello"u8.ToArray();
        var result = Base64ToCsvValidator.Validate(new Base64ToCsvRequest(Convert.ToBase64String(bytes), DocumentUrl: "https://example.com/f.csv"));

        Assert.False(result.IsSuccess);
        Assert.Contains("a la vez", result.Error!);
    }

    [Fact]
    public void Validator_ValidDocumentUrl_ReturnsSuccessWithUseUrl()
    {
        var result = Base64ToCsvValidator.Validate(new Base64ToCsvRequest(null, DocumentUrl: "https://example.com/file.csv"));

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.UseUrl);
    }

    [Fact]
    public void Validator_InvalidUrl_ReturnsFailure()
    {
        var result = Base64ToCsvValidator.Validate(new Base64ToCsvRequest(null, DocumentUrl: "not-a-url"));

        Assert.False(result.IsSuccess);
        Assert.Contains("URI", result.Error!);
    }

    // --- Service tests ---

    [Fact]
    public async Task Service_NullRequest_ReturnsBadRequest()
    {
        var logger = NullLogger<Base64ToCsvService>.Instance;
        var sut = new Base64ToCsvService(logger, _fetchService);

        var result = await sut.ConvertAsync(null);

        Assert.Equal(400, (result as Microsoft.AspNetCore.Http.IStatusCodeHttpResult)?.StatusCode);
    }

    [Fact]
    public async Task Service_ValidRequest_ReturnsFileResult()
    {
        var logger = NullLogger<Base64ToCsvService>.Instance;
        var sut = new Base64ToCsvService(logger, _fetchService);
        var bytes = "a,b\n1,2\n"u8.ToArray();

        var result = await sut.ConvertAsync(new Base64ToCsvRequest(Convert.ToBase64String(bytes)));

        Assert.IsAssignableFrom<IResult>(result);
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }
}
