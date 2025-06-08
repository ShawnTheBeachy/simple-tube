namespace SimpleTube.RestApi.Infrastructure.Images;

internal static class RandomStringGenerator
{
    private static char[]? _cachedChars;
    private const string CharSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_";

    public static string GenerateRandomString(int length)
    {
        _cachedChars ??= CharSet.ToCharArray();
        var chars = new char[length];

        for (var i = 0; i < length; i++)
            chars[i] = _cachedChars[Random.Shared.Next(0, CharSet.Length)];

        return new string(chars);
    }
}
