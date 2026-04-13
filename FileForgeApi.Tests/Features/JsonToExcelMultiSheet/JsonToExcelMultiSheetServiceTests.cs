using System.Text.Json;
using FileForgeApi.Features.ExcelToJsonMultiSheet;
using FileForgeApi.Features.JsonToExcelMultiSheet;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;
using NSubstitute;

namespace FileForgeApi.Tests.Features.JsonToExcelMultiSheet;

public class JsonToExcelMultiSheetServiceTests
{
    private readonly ILogger<JsonToExcelMultiSheetService> _logger =
        Substitute.For<ILogger<JsonToExcelMultiSheetService>>();
    private readonly JsonToExcelMultiSheetService _sut;

    public JsonToExcelMultiSheetServiceTests()
    {
        _sut = new JsonToExcelMultiSheetService(_logger);
    }

    [Fact]
    public async Task ConvertAsync_NullRequest_ReturnsFailure()
    {
        var result = await _sut.ConvertAsync(null);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task ConvertAsync_EmptySheets_ReturnsFailure()
    {
        var request = new JsonToExcelMultiSheetRequest(new Dictionary<string, List<Dictionary<string, JsonElement>>>());

        var result = await _sut.ConvertAsync(request);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ConvertAsync_ValidStringSheets_ReturnsBase64Excel()
    {
        var request = new JsonToExcelMultiSheetRequest(new Dictionary<string, List<Dictionary<string, JsonElement>>>
        {
            ["Sheet1"] =
            [
                new Dictionary<string, JsonElement> { ["Nombre"] = JsonValue("Alice"), ["Edad"] = JsonValue("30") },
                new Dictionary<string, JsonElement> { ["Nombre"] = JsonValue("Bob"), ["Edad"] = JsonValue("25") }
            ]
        });

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrWhiteSpace(result.Value!.Base64Content));

        var bytes = Convert.FromBase64String(result.Value.Base64Content);
        Assert.True(bytes.Length > 0);
    }

    [Fact]
    public async Task ConvertAsync_NumericValues_StoresTypedValues()
    {
        var request = new JsonToExcelMultiSheetRequest(new Dictionary<string, List<Dictionary<string, JsonElement>>>
        {
            ["Sheet1"] =
            [
                new Dictionary<string, JsonElement>
                {
                    ["Nombre"] = JsonValue("Alice"),
                    ["Edad"] = JsonNumber(30),
                    ["Activo"] = JsonBool(true)
                }
            ]
        });

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrWhiteSpace(result.Value!.Base64Content));
    }

    [Fact]
    public async Task ConvertAsync_MultipleSheets_GeneratesExcelWithAllSheets()
    {
        var request = new JsonToExcelMultiSheetRequest(new Dictionary<string, List<Dictionary<string, JsonElement>>>
        {
            ["Hoja1"] =
            [
                new Dictionary<string, JsonElement> { ["Col1"] = JsonValue("A1"), ["Col2"] = JsonValue("B1") }
            ],
            ["Hoja2"] =
            [
                new Dictionary<string, JsonElement> { ["X"] = JsonNumber(42), ["Y"] = JsonBool(false) }
            ]
        });

        var result = await _sut.ConvertAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        var excelBytes = Convert.FromBase64String(result.Value!.Base64Content);
        var excelRequest = new ExcelToJsonMultiSheetRequest(Convert.ToBase64String(excelBytes));
        var excelService = new ExcelToJsonMultiSheetService(
            Substitute.For<ILogger<ExcelToJsonMultiSheetService>>(),
            Substitute.For<FileForgeApi.Shared.Documents.IDocumentFetchService>());
        var readResult = await excelService.ConvertAsync(excelRequest);

        Assert.True(readResult.IsSuccess);
        Assert.Equal(2, readResult.Value!.Sheets.Count);
        Assert.True(readResult.Value.Sheets.ContainsKey("Hoja1"));
        Assert.True(readResult.Value.Sheets.ContainsKey("Hoja2"));
        Assert.Equal("A1", readResult.Value.Sheets["Hoja1"][0]["Col1"].GetString());
    }

    private static JsonElement JsonValue(string s) =>
        JsonDocument.Parse($"\"{s}\"").RootElement.Clone();

    private static JsonElement JsonNumber(double n) =>
        JsonDocument.Parse(n.ToString(System.Globalization.CultureInfo.InvariantCulture)).RootElement.Clone();

    private static JsonElement JsonBool(bool b) =>
        JsonDocument.Parse(b ? "true" : "false").RootElement.Clone();
}
