using System.Text.Json;
using FileForgeApi.Features.JsonToExcel;

namespace FileForgeApi.Tests.Features.JsonToExcel;

public class JsonToExcelValidatorTests
{
    [Fact]
    public void Validate_NullRequest_ReturnsFailure()
    {
        var result = JsonToExcelValidator.Validate(null);

        Assert.False(result.IsSuccess);
        Assert.Contains("obligatorio", result.Error!);
    }

    [Fact]
    public void Validate_NullRows_ReturnsFailure()
    {
        var request = new JsonToExcelRequest(null!);

        var result = JsonToExcelValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Rows", result.Error!);
    }

    [Fact]
    public void Validate_EmptyRows_ReturnsFailure()
    {
        var request = new JsonToExcelRequest([]);

        var result = JsonToExcelValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("vacía", result.Error!);
    }

    [Fact]
    public void Validate_RowsWithOnlyEmptyDictionaries_ReturnsFailure()
    {
        var request = new JsonToExcelRequest([new Dictionary<string, JsonElement>()]);

        var result = JsonToExcelValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("encabezados", result.Error!);
    }

    [Fact]
    public void Validate_ValidRows_ReturnsSuccess()
    {
        var request = new JsonToExcelRequest(
        [
            new Dictionary<string, JsonElement>
            {
                ["Nombre"] = JsonDocument.Parse("\"Alice\"").RootElement.Clone(),
                ["Edad"] = JsonDocument.Parse("30").RootElement.Clone()
            }
        ]);

        var result = JsonToExcelValidator.Validate(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value!);
    }
}
