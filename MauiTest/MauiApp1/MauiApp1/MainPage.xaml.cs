using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp1.Screens;
using Plugin.Maui.Biometric;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Syncfusion.Maui.Toolkit.BottomSheet;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
    float _time = 0f;
    IDispatcherTimer? _timer;
    MyMvvmCode model = new MyMvvmCode();
    

    public MainPage()
    {
        InitializeComponent();
        BindingContext = model;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 fps
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    private void OnBottomSheetButtonClicked(object sender, EventArgs e)
    {
        bottomSheet.FadeToAsync(1, 250);
        bottomSheet.Show();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (_timer != null)
        {
            _timer.Stop();
            _timer.Tick -= OnTimerTick;
            _timer = null;
        }
    }

    void OnTimerTick(object? sender, EventArgs e)
    {
        _time += 0.008f;               // control the speed here (lower = slower)
        MeshCanvas.InvalidateSurface();
    }

    void OnMeshPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        // Base color (matches your previous lavender-ish tone)
        canvas.Clear(new SKColor(0x9B, 0x8B, 0xC0));

        float w = info.Width;
        float h = info.Height;

        // ----- Soft drifting glows (larger radii = more continuous / meshy look) -----

        // Pink / rose
        DrawSoftGlow(canvas, w, h,
            cx: 0.20f + 0.14f * MathF.Sin(_time),
            cy: 0.25f + 0.12f * MathF.Cos(_time + 0.8f),
            radius: 0.78f,
            color: new SKColor(255, 120, 180, 190));

        // Magenta
        DrawSoftGlow(canvas, w, h,
            cx: 0.75f + 0.12f * MathF.Cos(_time + 1.5f),
            cy: 0.28f + 0.14f * MathF.Sin(_time - 0.4f),
            radius: 0.72f,
            color: new SKColor(255, 70, 200, 180));

        // Cyan / blue
        DrawSoftGlow(canvas, w, h,
            cx: 0.82f + 0.14f * MathF.Sin(_time + 2.1f),
            cy: 0.78f + 0.12f * MathF.Cos(_time + 1.2f),
            radius: 0.80f,
            color: new SKColor(50, 170, 255, 170));

        // Peach / coral
        DrawSoftGlow(canvas, w, h,
            cx: 0.25f + 0.12f * MathF.Cos(_time + 2.8f),
            cy: 0.78f + 0.14f * MathF.Sin(_time + 1.8f),
            radius: 0.74f,
            color: new SKColor(255, 130, 90, 180));
    }

    void DrawSoftGlow(SKCanvas canvas, float width, float height,
                      float cx, float cy, float radius, SKColor color)
    {
        var center = new SKPoint(cx * width, cy * height);
        float r = radius * Math.Max(width, height);

        using var paint = new SKPaint
        {
            IsAntialias = true,
            Shader = SKShader.CreateRadialGradient(
                center,
                r,
                new[] { color, color.WithAlpha(0) },
                new[] { 0.0f, 1.0f },
                SKShaderTileMode.Clamp)
        };

        canvas.DrawCircle(center, r, paint);
    }

    private async void BottomSheet_OnStateChanged(object? sender, StateChangedEventArgs e)
    {
        if (e.OldState != BottomSheetState.Collapsed && e.NewState == BottomSheetState.Collapsed)
        {
            await bottomSheet.FadeToAsync(0, 250);
            bottomSheet.IsOpen = false;
        }
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        Navigation.PushAsync(new TodoScreen());
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new EchoesSpacePage());
    }

    private void myTextbox_TextChanged(object sender, TextChangedEventArgs e)
    {
        model.UserName = e.NewTextValue;
    }

    private void ToggleTheeme_Clicked(object sender, EventArgs e)
    {
        ThemeManager.ToggleTheme();

        model.CurrentThemeName = ThemeManager.GetThemeName();
    }

    private void GotoGreeting_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Greeting());
    }

    private async void BioMetricsButton_OnClicked(object? sender, EventArgs e)
    {
        var authRequest = new AuthenticationRequest();
        authRequest.NegativeText = "Failed";
        authRequest.Title = "Authenticate";
        var authResult = await BiometricAuthenticationService.Default.AuthenticateAsync(
            authRequest, CancellationToken.None);

        String s = "Hello";
    }

    private void GoToColour_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ColourSlider());
    }

    private void GoToTip_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new TipScreen());
    }
}