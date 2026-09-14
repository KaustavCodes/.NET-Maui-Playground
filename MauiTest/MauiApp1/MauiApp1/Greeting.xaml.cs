using MauiApp1.ViewModels;

namespace MauiApp1;

public partial class Greeting : ContentPage
{
	private readonly MainViewModel _viewModel;

    public Greeting()
    {
        InitializeComponent();

        _viewModel = new MainViewModel();
        BindingContext = _viewModel;   // ← This connects the ViewModel to the page
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        _viewModel.Name = string.Empty;
    }
}