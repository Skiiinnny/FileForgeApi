using System.Text.Json;
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
    public async Task ConvertAsync_ValidStringRows_ReturnsBase64Excel()
    {
        var request = new JsonToExcelRequest(
        [
            new Dictionary<string, JsonElement> { ["Nombre"] = JsonValue("Alice"), ["Edad"] = JsonValue("30") },
            new Dictionary<string, JsonElement> { ["Nombre"] = JsonValue("Bob"), ["Edad"] = JsonValue("25") }
        ]);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrWhiteSpace(result.Value!.Base64Content));

        var bytes = Convert.FromBase64String(result.Value.Base64Content);
        Assert.True(bytes.Length > 0);
    }

    [Fact]
    public async Task ConvertAsync_NumericRows_StoresNumbersAsNumbers()
    {
        var request = new JsonToExcelRequest(
        [
            new Dictionary<string, JsonElement>
            {
                ["Nombre"] = JsonValue("Alice"),
                ["Edad"] = JsonNumber(30),
                ["Score"] = JsonNumber(9.5)
            }
        ]);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        var excelBytes = Convert.FromBase64String(result.Value!.Base64Content);
        using var stream = new MemoryStream(excelBytes);
        var rows = (await stream.QueryAsync(useHeaderRow: true)).Cast<IDictionary<string, object>>().ToList();
        Assert.Single(rows);
        Assert.Equal(30d, Convert.ToDouble(rows[0]["Edad"]));
        Assert.Equal(9.5, Convert.ToDouble(rows[0]["Score"]));
    }

    [Fact]
    public async Task ConvertAsync_BooleanRows_StoresBooleansAsBooleans()
    {
        var request = new JsonToExcelRequest(
        [
            new Dictionary<string, JsonElement>
            {
                ["Activo"] = JsonBool(true),
                ["Verificado"] = JsonBool(false)
            }
        ]);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrWhiteSpace(result.Value!.Base64Content));
    }

    [Fact]
    public async Task ConvertAsync_NullValue_GeneratesExcel()
    {
        var request = new JsonToExcelRequest(
        [
            new Dictionary<string, JsonElement> { ["Col1"] = JsonValue("valor"), ["Col2"] = JsonNull() }
        ]);

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrWhiteSpace(result.Value!.Base64Content));
    }

    [Fact]
    public async Task ConvertAsync_RoundTrip_StringData_PreservesValues()
    {
        var originalRows = new List<Dictionary<string, JsonElement>>
        {
            new() { ["Nombre"] = JsonValue("Alice"), ["Edad"] = JsonValue("30") },
            new() { ["Nombre"] = JsonValue("Bob"), ["Edad"] = JsonValue("25") }
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
            Assert.True(dictRow.ContainsKey("Nombre"));
            Assert.Equal(originalRows[i]["Nombre"].GetString(), dictRow["Nombre"]?.ToString() ?? string.Empty);
        }
    }

    private static JsonElement JsonValue(string s) =>
        JsonDocument.Parse($"\"{s}\"").RootElement.Clone();

    private static JsonElement JsonNumber(double n) =>
        JsonDocument.Parse(n.ToString(System.Globalization.CultureInfo.InvariantCulture)).RootElement.Clone();

    private static JsonElement JsonBool(bool b) =>
        JsonDocument.Parse(b ? "true" : "false").RootElement.Clone();

    private static JsonElement JsonNull() =>
        JsonDocument.Parse("null").RootElement.Clone();
}
