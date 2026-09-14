using CommunityToolkit.Mvvm.ComponentModel;
using MauiApp1;

namespace MauiApp1.ViewModels;

public class MainViewModel : ObservableObject
{
    private string _name = "";
    public string Name
    {
        get => _name;
        set
        {
            if (SetProperty(ref _name, value))
            {
                // Also update the greeting when name changes
                OnPropertyChanged(nameof(Greeting));
            }
        }
    }

    public string Greeting => string.IsNullOrWhiteSpace(Name)
        ? "Hello, stranger!"
        : $"Hello, {Name}!";
}