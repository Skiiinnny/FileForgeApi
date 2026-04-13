using System.Net;
using FileForgeApi.Shared.Documents;

namespace FileForgeApi.Tests.Shared.Documents;

public class UrlDocumentFetchServiceTests
{
    private static UrlDocumentFetchService CreateSut(HttpMessageHandler handler)
    {
        var client = new HttpClient(handler);
        return new UrlDocumentFetchService(client);
    }

    [Fact]
    public async Task FetchAsync_ValidPublicUrl_ReturnsSuccess()
    {
        var content = new byte[] { 1, 2, 3, 4 };
        var sut = CreateSut(new FakeHttpHandler(HttpStatusCode.OK, content));

        var result = await sut.FetchAsync("https://example.com/file.xlsx");

        Assert.True(result.IsSuccess);
        Assert.Equal(content, result.Value);
    }

    [Fact]
    public async Task FetchAsync_FtpScheme_ReturnsFailure()
    {
        var sut = CreateSut(new FakeHttpHandler(HttpStatusCode.OK, Array.Empty<byte>()));

        var result = await sut.FetchAsync("ftp://example.com/file.xlsx");

        Assert.False(result.IsSuccess);
        Assert.Contains("HTTP", result.Error!);
    }

    [Fact]
    public async Task FetchAsync_FileScheme_ReturnsFailure()
    {
        var sut = CreateSut(new FakeHttpHandler(HttpStatusCode.OK, Array.Empty<byte>()));

        var result = await sut.FetchAsync("file:///C:/file.xlsx");

        Assert.False(result.IsSuccess);
        Assert.Contains("HTTP", result.Error!);
    }

    [Fact]
    public async Task FetchAsync_PrivateIpLoopback_ReturnsFailure()
    {
        var sut = CreateSut(new FakeHttpHandler(HttpStatusCode.OK, Array.Empty<byte>()));

        var result = await sut.FetchAsync("http://127.0.0.1/file.xlsx");

        Assert.False(result.IsSuccess);
        Assert.Contains("no permitida", result.Error!);
    }

    [Fact]
    public async Task FetchAsync_PrivateIpRfc1918_10x_ReturnsFailure()
    {
        var sut = CreateSut(new FakeHttpHandler(HttpStatusCode.OK, Array.Empty<byte>()));

        var result = await sut.FetchAsync("http://10.0.0.1/file.xlsx");

        Assert.False(result.IsSuccess);
        Assert.Contains("no permitida", result.Error!);
    }

    [Fact]
    public async Task FetchAsync_PrivateIp192168_ReturnsFailure()
    {
        var sut = CreateSut(new FakeHttpHandler(HttpStatusCode.OK, Array.Empty<byte>()));

        var result = await sut.FetchAsync("http://192.168.1.1/file.xlsx");

        Assert.False(result.IsSuccess);
        Assert.Contains("no permitida", result.Error!);
    }

    [Fact]
    public async Task FetchAsync_PrivateIp172_16_ReturnsFailure()
    {
        var sut = CreateSut(new FakeHttpHandler(HttpStatusCode.OK, Array.Empty<byte>()));

        var result = await sut.FetchAsync("http://172.16.0.1/file.xlsx");

        Assert.False(result.IsSuccess);
        Assert.Contains("no permitida", result.Error!);
    }

    [Fact]
    public async Task FetchAsync_LinkLocal169_254_ReturnsFailure()
    {
        var sut = CreateSut(new FakeHttpHandler(HttpStatusCode.OK, Array.Empty<byte>()));

        var result = await sut.FetchAsync("http://169.254.1.1/file.xlsx");

        Assert.False(result.IsSuccess);
        Assert.Contains("no permitida", result.Error!);
    }

    [Fact]
    public async Task FetchAsync_NonSuccessStatusCode_ReturnsFailure()
    {
        var sut = CreateSut(new FakeHttpHandler(HttpStatusCode.NotFound, Array.Empty<byte>()));

        var result = await sut.FetchAsync("https://example.com/file.xlsx");

        Assert.False(result.IsSuccess);
        Assert.Contains("404", result.Error!);
    }

    [Fact]
    public async Task FetchAsync_ServerError_ReturnsFailureWithStatusCode()
    {
        var sut = CreateSut(new FakeHttpHandler(HttpStatusCode.InternalServerError, Array.Empty<byte>()));

        var result = await sut.FetchAsync("https://example.com/file.xlsx");

        Assert.False(result.IsSuccess);
        Assert.Contains("500", result.Error!);
    }

    [Fact]
    public async Task FetchAsync_EmptyResponseBody_ReturnsFailure()
    {
        var sut = CreateSut(new FakeHttpHandler(HttpStatusCode.OK, Array.Empty<byte>()));

        var result = await sut.FetchAsync("https://example.com/file.xlsx");

        Assert.False(result.IsSuccess);
        Assert.Contains("vacío", result.Error!);
    }

    [Fact]
    public async Task FetchAsync_Timeout_ReturnsFailure()
    {
        var sut = CreateSut(new TimeoutHttpHandler());

        var result = await sut.FetchAsync("https://example.com/file.xlsx");

        Assert.False(result.IsSuccess);
        Assert.Contains("tiempo límite", result.Error!);
    }

    [Fact]
    public async Task FetchAsync_InvalidUrl_ReturnsFailure()
    {
        var sut = CreateSut(new FakeHttpHandler(HttpStatusCode.OK, Array.Empty<byte>()));

        var result = await sut.FetchAsync("not-a-url");

        Assert.False(result.IsSuccess);
    }

    private sealed class FakeHttpHandler(HttpStatusCode statusCode, byte[] content) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new ByteArrayContent(content)
            };
            return Task.FromResult(response);
        }
    }

    private sealed class TimeoutHttpHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new TaskCanceledException("Timeout");
        }
    }
}
