using Plugin.Maui.Biometric;

namespace MauiApp1;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        ThemeManager.SetTheme(ThemeManager.defaultTheme);
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(new MainPage()) { Title = "MauiApp1" });
    }
}
