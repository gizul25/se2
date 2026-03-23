using System;
using Avalonia;
using SE2.Data;

namespace SE2;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        //SDM sdm = new();
        //sdm.Load("winter");
        AM aM = new();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
