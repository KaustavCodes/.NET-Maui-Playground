using Microsoft.Extensions.Logging;
using MauiApp1.Shared.Services;
using MauiApp1.Services;
using DotNet.Meteor.HotReload.Plugin;
using MauiIcons.Material;
using Plugin.Maui.Biometric;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;

namespace MauiApp1;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseMaterialMauiIcons()
            #if DEBUG
                .EnableHotReload()
            #endif
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });
        
        builder.Services.AddSingleton<IBiometric>(BiometricAuthenticationService.Default);

        builder.ConfigureSyncfusionToolkit();

        // Add device-specific services used by the MauiApp1.Shared project
        builder.Services.AddSingleton<IFormFactor, FormFactor>();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
