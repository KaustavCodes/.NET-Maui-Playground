namespace MauiApp1.Screens;

public partial class ColourSlider : ContentPage
{
	public ColourSlider()
	{
		InitializeComponent();

		BindingContext = new MauiApp1.ViewModels.ColourPickerVM();
	}

    private void GoBackButton_Clicked(object sender, EventArgs e)
	{
		Navigation.PopAsync();
	}

    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
	{
		
	}

    private void RandomColour_Clicked(object sender, EventArgs e)
	{
		int r = new Random().Next(0, 256);
		int g = new Random().Next(0, 256);
		int b = new Random().Next(0, 256);

		ColourRSlider.Value = r;
		ColourGSlider.Value = g;
		ColourBSlider.Value = b;
	}
}