using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using ZXing.Net.Maui.Controls;

namespace Madia;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

#if ANDROID || IOS || MACCATALYST
        builder.UseBarcodeReader();
#endif

        builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        });

        return builder.Build();
    }
}
