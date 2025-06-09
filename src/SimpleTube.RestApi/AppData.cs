namespace SimpleTube.RestApi;

internal static class AppData
{
    public static string GetFile(string file) =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SimpleTube",
            file
        );

    public static string GetFolder(string folder) =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SimpleTube",
            folder
        );
}
