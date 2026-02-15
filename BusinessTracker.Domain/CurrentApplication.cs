namespace BusinessTracker.Domain;

/// <summary>
/// Отдельный класс для реализации общих методов в рамках приложения.
/// </summary>
public static class CurrentApplication
{
    // ASCII Art
    private static readonly string[] AsciiArt =
    [
        @"            (@@@@@_",
        @"            |     @@",
        @"     |||    ) ~/~ @@",
        @"     |  '   [  ^  ]",
        @"     \__/\   `----",
        @"      \   \~~~     ~~~~\",
        @" ----------------------------"
    ];

    /// <summary>
    /// Отобразить логотип приложения.
    /// </summary>
    public static void ShowLogo()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        foreach (var line in AsciiArt)
        {
            Console.WriteLine(line);
        }

        Console.ResetColor();
        Console.WriteLine($"The Personal Account Software (c) by @GorevArtem\nVersion: {CurrentVersion()}");
    }

    /// <summary>
    /// Получить текущую версию сборки приложения.
    /// </summary>
    /// <returns>string</returns>
    public static string CurrentVersion()
    {
        var assembly = typeof(CurrentApplication).Assembly;
        var version = assembly.GetName().Version?.ToString() ?? "1.0.0.0";
        return version;
    }
}