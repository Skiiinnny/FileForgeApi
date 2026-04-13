using FileForgeApi.Features.ExcelToJson;
using FileForgeApi.Features.JsonToExcel;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;
using NSubstitute;

namespace FileForgeApi.Tests.Features.JsonToExcel;

public class JsonToExcelServiceTests
{
    private readonly ILogger<JsonToExcelService> _logger = Substitute.For<ILogger<JsonToExcelService>>();
    private readonly JsonToExcelService _sut;

    public JsonToExcelServiceTests()
    {
        _sut = new JsonToExcelService(_logger);
    }

    [Fact]
    public async Task ConvertAsync_NullRequest_ReturnsFailure()
    {
        var result = await _sut.ConvertAsync(null);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task ConvertAsync_EmptyRows_ReturnsFailure()
    {
        var request = new JsonToExcelRequest([]);

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ConvertAsync_ValidRows_ReturnsBase64Excel()
    {
        var request = new JsonToExcelRequest(
        [
            new Dictionary<string, string> { ["Nombre"] = "Alice", ["Edad"] = "30" },
            new Dictionary<string, string> { ["Nombre"] = "Bob", ["Edad"] = "25" }
        ]);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrWhiteSpace(result.Value!.Base64Content));

        var bytes = Convert.FromBase64String(result.Value.Base64Content);
        Assert.True(bytes.Length > 0);
    }

    [Fact]
    public async Task ConvertAsync_RowsWithEmptyValues_GeneratesExcel()
    {
        var request = new JsonToExcelRequest(
        [
            new Dictionary<string, string> { ["Col1"] = "valor", ["Col2"] = "" }
        ]);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrWhiteSpace(result.Value!.Base64Content));
    }

    [Fact]
    public async Task ConvertAsync_RoundTrip_JsonToExcelToJson_PreservesData()
    {
        var originalRows = new List<Dictionary<string, string>>
        {
            new() { ["Nombre"] = "Alice", ["Edad"] = "30" },
            new() { ["Nombre"] = "Bob", ["Edad"] = "25" }
        };
        var request = new JsonToExcelRequest(originalRows);

        var toExcelResult = await _sut.ConvertAsync(request);
        Assert.True(toExcelResult.IsSuccess);

        var excelBytes = Convert.FromBase64String(toExcelResult.Value!.Base64Content);
        using var stream = new MemoryStream(excelBytes);
        var asyncRows = await stream.QueryAsync(useHeaderRow: true);
        var readBack = asyncRows.Cast<object>().ToList();

        Assert.Equal(originalRows.Count, readBack.Count);

        for (int i = 0; i < originalRows.Count; i++)
        {
            var dictRow = (IDictionary<string, object>)readBack[i];
            foreach (var kvp in originalRows[i])
            {
                Assert.True(dictRow.ContainsKey(kvp.Key));
                Assert.Equal(kvp.Value, dictRow[kvp.Key]?.ToString() ?? string.Empty);
            }
        }
    }
}
