using Avalonia;
using SE2.Domain;

namespace SE2;

class Program
{
    public static void Main(string[] args)
    {
        DM.Load();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
