using System.Text.Json;
using FileForgeApi.Features.ExcelToJson;
using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Results;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;
using NSubstitute;

namespace FileForgeApi.Tests.Features.ExcelToJson;

public class ExcelToJsonServiceTests
{
    private readonly ILogger<ExcelToJsonService> _logger = Substitute.For<ILogger<ExcelToJsonService>>();
    private readonly IDocumentFetchService _fetchService = Substitute.For<IDocumentFetchService>();
    private readonly ExcelToJsonService _sut;

    public ExcelToJsonServiceTests()
    {
        _sut = new ExcelToJsonService(_logger, _fetchService);
    }

    [Fact]
    public async Task ConvertAsync_NullRequest_ReturnsFailure()
    {
        var result = await _sut.ConvertAsync(null);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task ConvertAsync_EmptyBase64_ReturnsFailure()
    {
        var request = new ExcelToJsonRequest("");

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ConvertAsync_InvalidExcelContent_ReturnsFailure()
    {
        var invalidBytes = new byte[] { 1, 2, 3, 4 };
        var request = new ExcelToJsonRequest(Convert.ToBase64String(invalidBytes));

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("No se pudo leer el Excel", result.Error!);
    }

    [Fact]
    public async Task ConvertAsync_InferTypesFalse_ReturnsStringElements()
    {
        var base64 = CreateTestExcelBase64(new[]
        {
            new Dictionary<string, object> { ["Nombre"] = "Alice", ["Edad"] = 30 },
            new Dictionary<string, object> { ["Nombre"] = "Bob", ["Edad"] = 25 }
        });
        var request = new ExcelToJsonRequest(base64, InferTypes: false);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Rows.Count);
        Assert.Equal(JsonValueKind.String, result.Value.Rows[0]["Nombre"].ValueKind);
        Assert.Equal("Alice", result.Value.Rows[0]["Nombre"].GetString());
        Assert.Equal(JsonValueKind.String, result.Value.Rows[0]["Edad"].ValueKind);
        Assert.Equal("30", result.Value.Rows[0]["Edad"].GetString());
    }

    [Fact]
    public async Task ConvertAsync_InferTypesTrue_ReturnsTypedElements()
    {
        var base64 = CreateTestExcelBase64(new[]
        {
            new Dictionary<string, object> { ["Nombre"] = "Alice", ["Edad"] = 30 }
        });
        var request = new ExcelToJsonRequest(base64, InferTypes: true);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value!.Rows);
        Assert.Equal(JsonValueKind.String, result.Value.Rows[0]["Nombre"].ValueKind);
        Assert.Equal(JsonValueKind.Number, result.Value.Rows[0]["Edad"].ValueKind);
        Assert.Equal(30, result.Value.Rows[0]["Edad"].GetInt32());
    }

    [Fact]
    public async Task ConvertAsync_InferTypesDefault_TreatsValuesAsStrings()
    {
        var base64 = CreateTestExcelBase64(new[]
        {
            new Dictionary<string, object> { ["Valor"] = 42 }
        });
        var request = new ExcelToJsonRequest(base64);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(JsonValueKind.String, result.Value!.Rows[0]["Valor"].ValueKind);
    }

    [Fact]
    public async Task ConvertAsync_ExcelWithNullValues_ReturnsNullElements()
    {
        var base64 = CreateTestExcelBase64(new[]
        {
            new Dictionary<string, object> { ["Col1"] = "valor", ["Col2"] = (object)null! }
        });
        var request = new ExcelToJsonRequest(base64);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Rows);
        Assert.Equal("valor", result.Value.Rows[0]["Col1"].GetString());
    }

    [Fact]
    public async Task ConvertAsync_EmptyExcel_ReturnsEmptyList()
    {
        var base64 = CreateTestExcelBase64(Array.Empty<Dictionary<string, object>>());
        var request = new ExcelToJsonRequest(base64);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value!.Rows);
    }

    [Fact]
    public async Task ConvertAsync_DocumentUrlSuccess_ReturnsRows()
    {
        var excelBytes = CreateTestExcelBytes(new[]
        {
            new Dictionary<string, object> { ["A"] = "1", ["B"] = "2" }
        });
        _fetchService.FetchAsync("https://example.com/file.xlsx")
            .Returns(Result<byte[]>.Success(excelBytes));

        var request = new ExcelToJsonRequest(null, "https://example.com/file.xlsx");

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value!.Rows);
    }

    [Fact]
    public async Task ConvertAsync_DocumentUrlFetchFails_ReturnsFailure()
    {
        _fetchService.FetchAsync(Arg.Any<string>())
            .Returns(Result<byte[]>.Failure("La URL retornó el código HTTP 404."));

        var request = new ExcelToJsonRequest(null, "https://example.com/notfound.xlsx");

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("404", result.Error!);
    }

    private static string CreateTestExcelBase64(IEnumerable<Dictionary<string, object>> rows)
    {
        using var stream = new MemoryStream();
        stream.SaveAs(rows);
        return Convert.ToBase64String(stream.ToArray());
    }

    private static byte[] CreateTestExcelBytes(IEnumerable<Dictionary<string, object>> rows)
    {
        using var stream = new MemoryStream();
        stream.SaveAs(rows);
        return stream.ToArray();
    }
}
