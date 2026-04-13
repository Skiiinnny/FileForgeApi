using FileForgeApi.Features.JsonToCsv;

namespace FileForgeApi.Tests.Features.JsonToCsv;

public class JsonToCsvValidatorTests
{
    [Fact]
    public void Validate_NullRequest_ReturnsFailure()
    {
        var result = JsonToCsvValidator.Validate(null);

        Assert.False(result.IsSuccess);
        Assert.Contains("obligatorio", result.Error!);
    }

    [Fact]
    public void Validate_NullRows_ReturnsFailure()
    {
        var request = new JsonToCsvRequest(null!);

        var result = JsonToCsvValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Rows", result.Error!);
    }

    [Fact]
    public void Validate_EmptyRows_ReturnsFailure()
    {
        var request = new JsonToCsvRequest([]);

        var result = JsonToCsvValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("vacía", result.Error!);
    }

    [Fact]
    public void Validate_RowsWithOnlyEmptyDictionaries_ReturnsFailure()
    {
        var request = new JsonToCsvRequest([new Dictionary<string, string>()]);

        var result = JsonToCsvValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("encabezados", result.Error!);
    }

    [Fact]
    public void Validate_ValidRows_ReturnsSuccess()
    {
        var request = new JsonToCsvRequest(
        [
            new Dictionary<string, string> { ["Nombre"] = "Alice", ["Edad"] = "30" }
        ]);

        var result = JsonToCsvValidator.Validate(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value!);
    }

    [Fact]
    public void Validate_SeparatorWithMultipleChars_ReturnsFailure()
    {
        var request = new JsonToCsvRequest(
            [new Dictionary<string, string> { ["A"] = "1" }],
            Separator: ";;");

        var result = JsonToCsvValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Separator", result.Error!);
    }

    [Fact]
    public void Validate_InvalidEncoding_ReturnsFailure()
    {
        var request = new JsonToCsvRequest(
            [new Dictionary<string, string> { ["A"] = "1" }],
            Encoding: "invalid-encoding");

        var result = JsonToCsvValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Encoding", result.Error!);
    }

    [Fact]
    public void Validate_InvalidNewLine_ReturnsFailure()
    {
        var request = new JsonToCsvRequest(
            [new Dictionary<string, string> { ["A"] = "1" }],
            NewLine: "invalid");

        var result = JsonToCsvValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("NewLine", result.Error!);
    }
}
