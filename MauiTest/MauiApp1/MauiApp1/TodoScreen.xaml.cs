using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1;

public partial class TodoScreen : ContentPage
{
    public TodoScreen()
    {
        InitializeComponent();
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        Debug.WriteLine("Button_OnClicked");
        // Navigate to Main Page
        Navigation.PopAsync(true);
    }
    
    private void OnSettingsClicked(object? sender, EventArgs e)
    {
        // Do nothing for now.
        
        Navigation.PopAsync(true);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StartAnimations();
    }

    private void StartAnimations()
    {
        // Floating orbs
        AnimateOrb(Orb1, -25, 5000);
        AnimateOrb(Orb2, 30, 7000);
        AnimateOrb(Orb3, -18, 6000);
        AnimateBox();

        // Continuous rotation
        var rotate = new Animation(v => RotatingRing.Rotation = v, 0, 360);
        rotate.Commit(this, "RotateRing", length: 18000, repeat: () => true);
    }

    private void AnimateBox()
    {
        var animation = new Animation();

        // Scale up
        animation.Add(0, 0.5, new Animation(
            v => MainCard.Scale = v,
            start: 1.0,
            end: 1.10,
            easing: Easing.BounceIn));

        // Scale down
        animation.Add(0.5, 1.0, new Animation(
            v => MainCard.Scale = v,
            start: 1.10,
            end: 1.0,
            easing: Easing.BounceOut));

        // Run forever
        animation.Commit(
            owner: this,
            name: "PulseAnimation",
            length: 2200,          // duration of one full cycle (ms)
            repeat: () => true);   // keep repeating
    }

    private void AnimateOrb(VisualElement orb, double offset, uint duration)
    {
        var animation = new Animation();
        animation.Add(0, 0.5, new Animation(v => orb.TranslationY = v, 0, offset, Easing.SinInOut));
        animation.Add(0.5, 1, new Animation(v => orb.TranslationY = v, offset, 0, Easing.SinInOut));

        animation.Commit(this, $"Orb_{orb.Id}", length: duration, repeat: () => true);
    }
}