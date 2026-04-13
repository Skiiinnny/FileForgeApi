using System.Text.Json;
using FileForgeApi.Features.JsonToExcelMultiSheet;

namespace FileForgeApi.Tests.Features.JsonToExcelMultiSheet;

public class JsonToExcelMultiSheetValidatorTests
{
    [Fact]
    public void Validate_NullRequest_ReturnsFailure()
    {
        var result = JsonToExcelMultiSheetValidator.Validate(null);

        Assert.False(result.IsSuccess);
        Assert.Contains("obligatorio", result.Error!);
    }

    [Fact]
    public void Validate_NullSheets_ReturnsFailure()
    {
        var request = new JsonToExcelMultiSheetRequest(null!);

        var result = JsonToExcelMultiSheetValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Sheets", result.Error!);
    }

    [Fact]
    public void Validate_EmptySheets_ReturnsFailure()
    {
        var request = new JsonToExcelMultiSheetRequest(new Dictionary<string, List<Dictionary<string, JsonElement>>>());

        var result = JsonToExcelMultiSheetValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("vacío", result.Error!);
    }

    [Fact]
    public void Validate_SheetWithEmptyRows_ReturnsFailure()
    {
        var request = new JsonToExcelMultiSheetRequest(new Dictionary<string, List<Dictionary<string, JsonElement>>>
        {
            ["Hoja1"] = []
        });

        var result = JsonToExcelMultiSheetValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Hoja1", result.Error!);
    }

    [Fact]
    public void Validate_SheetWithRowsWithoutKeys_ReturnsFailure()
    {
        var request = new JsonToExcelMultiSheetRequest(new Dictionary<string, List<Dictionary<string, JsonElement>>>
        {
            ["Hoja1"] = [new Dictionary<string, JsonElement>()]
        });

        var result = JsonToExcelMultiSheetValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("encabezados", result.Error!);
    }

    [Fact]
    public void Validate_ValidSheets_ReturnsSuccess()
    {
        var request = new JsonToExcelMultiSheetRequest(new Dictionary<string, List<Dictionary<string, JsonElement>>>
        {
            ["Hoja1"] = [new Dictionary<string, JsonElement>
            {
                ["A"] = JsonDocument.Parse("\"valor\"").RootElement.Clone()
            }]
        });

        var result = JsonToExcelMultiSheetValidator.Validate(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value!);
    }
}
