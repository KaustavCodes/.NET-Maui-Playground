using MauiApp1.ViewModels;

namespace MauiApp1.Screens;

public partial class TipScreen : ContentPage
{
    TipViewModel ViewModel => (TipViewModel)BindingContext;
    public TipScreen()
	{
		InitializeComponent();

		BindingContext = new MauiApp1.ViewModels.TipViewModel();
	}

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
	{
		var picker = (Picker)sender;
		var selectedValue = picker.SelectedItem.ToString();
		if (!string.IsNullOrEmpty(selectedValue))
			ViewModel.TipPercentage = int.Parse(selectedValue);
	}
}