using System.Text;

namespace FileForgeApi.Shared.Encoding;

public static class EncodingHelper
{
    public static bool TryGetEncoding(string? name, out System.Text.Encoding? encoding)
    {
        encoding = null;
        if (string.IsNullOrWhiteSpace(name))
            return false;

        try
        {
            encoding = System.Text.Encoding.GetEncoding(name.Trim());
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
