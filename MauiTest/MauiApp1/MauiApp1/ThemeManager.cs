namespace MauiApp1;


public static class ThemeManager
{
    public static AppTheme defaultTheme = AppTheme.Dark;

    public static String GetThemeName()
    {
        return  Application.Current?.UserAppTheme switch
        {
            AppTheme.Dark => "Dark",
            AppTheme.Light => "Light",
            _ => "Dark"
        };
    }
    public static void SetTheme(AppTheme theme)
    {
        var app = Application.Current;
        if (app?.Resources?.MergedDictionaries == null)
            return;

        // Clear existing theme dictionaries
        app.Resources.MergedDictionaries.Clear();

        // Load the correct theme
        if (theme == AppTheme.Dark)
        {
            app.Resources.MergedDictionaries.Add(
                new DarkTheme());          // ← Use the class, not Source
            app.UserAppTheme = AppTheme.Dark;
        }
        else
        {
            app.Resources.MergedDictionaries.Add(
                new LightTheme());
            app.UserAppTheme = AppTheme.Light;
        }
    }

    public static void ToggleTheme()
    {
        var current = Application.Current?.UserAppTheme ?? AppTheme.Unspecified;

        if (current == AppTheme.Dark)
            SetTheme(AppTheme.Light);
        else
            SetTheme(AppTheme.Dark);
    }
}