using System.Security.Cryptography;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Layouts;

namespace MauiApp1;

public partial class EchoesSpacePage : ContentPage
{
	private readonly Random _random = new();
    
	private readonly List<string> _poemsPool = new()
    {
        "I open my eyes to the quiet grey,\nReady to face another day.",
        "The coffee warms my shaking hands,\nAs sunlight stretches 'cross the lands.",
        "A gentle breeze, a quiet mind,\nLeaving yesterday far behind.",
        "In the silence of the dawn,\nThe worries of the night are gone.",
        "Step by step, the path appears,\nSoftly calming all my fears.",
        "Listen to the falling rain,\nWashing away the silent pain.",
        "Stars fading in the morning light,\nWhispering peace to the fading night.",
        "A quiet heart, a gentle sigh,\nWatching clouds float in the sky.",
        "The echoes of a distant bell,\nStories only the silent tell."
    };

    private readonly HashSet<string> _flaggedPoems = new();
    private readonly List<FloatingPoem> _activePoems = new();

    public EchoesSpacePage()
    {
        InitializeComponent();
    }

	private void OnThemeToggleClicked(object? sender, EventArgs e)
	{
		if (Application.Current!.UserAppTheme == AppTheme.Dark)
		{
			Application.Current.UserAppTheme = AppTheme.Light;
		}
		else
		{
			Application.Current.UserAppTheme = AppTheme.Dark;
		}
	}

	private void OnHelpClicked(object? sender, EventArgs e)
	{
		
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
        Navigation.PopAsync(true);
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StartFloatingPoems();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopAllAnimations();
    }

    private void StartFloatingPoems()
    {
        // Clear previous
        PoemsContainer.Children.Clear();
        _activePoems.Clear();

        // Create 4 floating poems (one per quadrant)
        CreateFloatingPoem(zoneX: 0.05, zoneY: 0.12, zoneW: 0.45, zoneH: 0.28);
        CreateFloatingPoem(zoneX: 0.50, zoneY: 0.12, zoneW: 0.45, zoneH: 0.28);
        CreateFloatingPoem(zoneX: 0.05, zoneY: 0.60, zoneW: 0.45, zoneH: 0.28);
        CreateFloatingPoem(zoneX: 0.50, zoneY: 0.60, zoneW: 0.45, zoneH: 0.28);
    }

    private void CreateFloatingPoem(double zoneX, double zoneY, double zoneW, double zoneH)
    {
        var label = new Label
        {
            FontFamily = "SourceSerif4", // or your font
            FontSize = 13,
            FontAttributes = FontAttributes.Italic,
            TextColor = Application.Current.RequestedTheme == AppTheme.Dark
                ? Colors.White
                : Color.FromArgb("#0F172A"),
            LineHeight = 1.4,
            Opacity = 0,
            MaximumWidthRequest = 180
        };

        // Make it tappable for flagging
        var tap = new TapGestureRecognizer();
        tap.Tapped += async (s, e) => await OnPoemTapped(label);
        label.GestureRecognizers.Add(tap);

        AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.None);
        PoemsContainer.Children.Add(label);

        var floating = new FloatingPoem
        {
            Label = label,
            ZoneX = zoneX,
            ZoneY = zoneY,
            ZoneW = zoneW,
            ZoneH = zoneH
        };

        _activePoems.Add(floating);
        StartPoemCycle(floating);
    }

    private async void StartPoemCycle(FloatingPoem poem)
    {
        while (true)
        {
            // Pick a poem that hasn't been flagged
            var available = _poemsPool.Where(p => !_flaggedPoems.Contains(p)).ToList();
            if (available.Count == 0)
            {
                poem.Label.Text = "";
                poem.Label.Opacity = 0;
                await Task.Delay(2000);
                continue;
            }

            poem.Label.Text = available[_random.Next(available.Count)];

            // Generate random start & end positions inside the zone
            var (startX, startY, endX, endY) = GeneratePositions(poem);

            // Set initial position
            AbsoluteLayout.SetLayoutBounds(poem.Label,
                new Rect(startX, startY, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            // Duration between 9–14 seconds (same as Flutter)
            uint duration = (uint)(9000 + _random.Next(6000));

            // === Animation ===
            var animation = new Animation();

            // Opacity: fade in → hold → fade out
            animation.Add(0.00, 0.15, new Animation(v => poem.Label.Opacity = v, 0, 0.85, Easing.CubicIn));
            animation.Add(0.15, 0.85, new Animation(v => poem.Label.Opacity = v, 0.85, 0.85)); // hold
            animation.Add(0.85, 1.00, new Animation(v => poem.Label.Opacity = v, 0.85, 0, Easing.CubicOut));

            // Position movement
            animation.Add(0, 1, new Animation(v =>
            {
                double x = startX + (endX - startX) * v;
                double y = startY + (endY - startY) * v;
                AbsoluteLayout.SetLayoutBounds(poem.Label,
                    new Rect(x, y, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            }, 0, 1, Easing.SinInOut));

            // Run the animation
            var tcs = new TaskCompletionSource<bool>();
            animation.Commit(
                owner: this,
                name: $"Poem_{poem.Label.GetHashCode()}",
                length: duration,
                finished: (v, c) => tcs.TrySetResult(true));

            await tcs.Task;

            // Small pause before next cycle
            await Task.Delay(300);
        }
    }

    private (double startX, double startY, double endX, double endY) GeneratePositions(FloatingPoem poem)
    {
        double pageWidth = PoemsContainer.Width > 0 ? PoemsContainer.Width : 400;
        double pageHeight = PoemsContainer.Height > 0 ? PoemsContainer.Height : 800;

        double zoneLeft = pageWidth * poem.ZoneX;
        double zoneTop = pageHeight * poem.ZoneY;
        double zoneWidth = pageWidth * poem.ZoneW;
        double zoneHeight = pageHeight * poem.ZoneH;

        double sx = zoneLeft + _random.NextDouble() * (zoneWidth * 0.4);
        double sy = zoneTop + _random.NextDouble() * (zoneHeight * 0.4);
        double ex = zoneLeft + (0.5 + _random.NextDouble() * 0.4) * zoneWidth;
        double ey = zoneTop + (0.5 + _random.NextDouble() * 0.4) * zoneHeight;

        // Randomly swap direction
        if (_random.Next(2) == 0)
            return (ex, ey, sx, sy);

        return (sx, sy, ex, ey);
    }

    private async Task OnPoemTapped(Label label)
    {
        bool confirm = await DisplayAlert(
            "Flag anonymous entry?",
            "If you find this entry inappropriate or offensive, you can flag it to remove it from your space.",
            "Flag & Remove",
            "Cancel");

        if (!confirm) return;

        string text = label.Text;
        if (string.IsNullOrEmpty(text)) return;

        _flaggedPoems.Add(text);

        // Immediately hide this one
        this.AbortAnimation($"Poem_{label.GetHashCode()}");
        label.Opacity = 0;
        label.Text = "";

        await DisplayAlert("", "Poem flagged and removed from your view.", "OK");
    }

    private void StopAllAnimations()
    {
        foreach (var poem in _activePoems)
        {
            this.AbortAnimation($"Poem_{poem.Label.GetHashCode()}");
        }
    }

    private class FloatingPoem
    {
        public Label Label { get; set; }
        public double ZoneX { get; set; }
        public double ZoneY { get; set; }
        public double ZoneW { get; set; }
        public double ZoneH { get; set; }
    }
}