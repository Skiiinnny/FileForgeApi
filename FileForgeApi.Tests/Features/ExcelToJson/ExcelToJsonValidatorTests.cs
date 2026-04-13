using FileForgeApi.Features.ExcelToJson;

namespace FileForgeApi.Tests.Features.ExcelToJson;

public class ExcelToJsonValidatorTests
{
    [Fact]
    public void Validate_NullRequest_ReturnsFailure()
    {
        var result = ExcelToJsonValidator.Validate(null);

        Assert.False(result.IsSuccess);
        Assert.Contains("obligatorio", result.Error!);
    }

    [Fact]
    public void Validate_EmptyBase64_ReturnsFailure()
    {
        var request = new ExcelToJsonRequest("");

        var result = ExcelToJsonValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("exactamente uno", result.Error!);
    }

    [Fact]
    public void Validate_WhitespaceBase64_ReturnsFailure()
    {
        var request = new ExcelToJsonRequest("   ");

        var result = ExcelToJsonValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("exactamente uno", result.Error!);
    }

    [Fact]
    public void Validate_InvalidBase64_ReturnsFailure()
    {
        var request = new ExcelToJsonRequest("not-valid-base64!!!");

        var result = ExcelToJsonValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("Base64 válida", result.Error!);
    }

    [Fact]
    public void Validate_EmptyFileAfterDecode_ReturnsFailure()
    {
        var request = new ExcelToJsonRequest(Convert.ToBase64String(new byte[] { 0 }));
        var singleByteResult = ExcelToJsonValidator.Validate(request);
        Assert.True(singleByteResult.IsSuccess);

        var emptyBase64Request = new ExcelToJsonRequest(Convert.ToBase64String(Array.Empty<byte>()));
        var emptyResult = ExcelToJsonValidator.Validate(emptyBase64Request);
        Assert.False(emptyResult.IsSuccess);
    }

    [Fact]
    public void Validate_ValidBase64_ReturnsSuccessWithBytes()
    {
        var bytes = new byte[] { 1, 2, 3, 4 };
        var request = new ExcelToJsonRequest(Convert.ToBase64String(bytes));

        var result = ExcelToJsonValidator.Validate(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(bytes, result.Value.FileBytes);
        Assert.False(result.Value.UseUrl);
    }

    [Fact]
    public void Validate_Base64WithWhitespace_TrimsAndDecodes()
    {
        var bytes = new byte[] { 10, 20, 30 };
        var base64WithSpaces = $"  {Convert.ToBase64String(bytes)}  ";
        var request = new ExcelToJsonRequest(base64WithSpaces);

        var result = ExcelToJsonValidator.Validate(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(bytes, result.Value.FileBytes);
    }

    [Fact]
    public void Validate_BothFields_ReturnsFailure()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var request = new ExcelToJsonRequest(Convert.ToBase64String(bytes), "https://example.com/f.xlsx");

        var result = ExcelToJsonValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("a la vez", result.Error!);
    }

    [Fact]
    public void Validate_ValidDocumentUrl_ReturnsSuccessWithUseUrl()
    {
        var request = new ExcelToJsonRequest(null, "https://example.com/file.xlsx");

        var result = ExcelToJsonValidator.Validate(request);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.UseUrl);
    }

    [Fact]
    public void Validate_InvalidUrl_ReturnsFailure()
    {
        var request = new ExcelToJsonRequest(null, "not-a-url");

        var result = ExcelToJsonValidator.Validate(request);

        Assert.False(result.IsSuccess);
        Assert.Contains("URI", result.Error!);
    }
}
