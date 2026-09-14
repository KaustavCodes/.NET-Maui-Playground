using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiApp1;

public class TechnologyItem
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public override string ToString() => $"{Title} [{Category}]";
}

public partial class MyMvvmCode : ObservableObject
{
    [ObservableProperty]
    private string userName = "User";

    [ObservableProperty]
    private string currentThemeName = Application.Current.UserAppTheme == AppTheme.Light ? "Light" : "Dark";

    [ObservableProperty]
    private object? selectedCountry;

    [ObservableProperty]
    private object? selectedTechnology;

    public ObservableCollection<string> Countries { get; } = new()
    {
        "Argentina",
        "Australia",
        "Austria",
        "Belgium",
        "Brazil",
        "Canada",
        "Chile",
        "China",
        "Colombia",
        "Denmark",
        "Egypt",
        "Finland",
        "France",
        "Germany",
        "Greece",
        "India",
        "Indonesia",
        "Ireland",
        "Israel",
        "Italy",
        "Japan",
        "Malaysia",
        "Mexico",
        "Netherlands",
        "New Zealand",
        "Norway",
        "Peru",
        "Poland",
        "Portugal",
        "Singapore",
        "South Africa",
        "South Korea",
        "Spain",
        "Sweden",
        "Switzerland",
        "Thailand",
        "Turkey",
        "United Kingdom",
        "United States",
        "Vietnam"
    };

    public ObservableCollection<TechnologyItem> Technologies { get; } = new()
    {
        new TechnologyItem { Title = ".NET MAUI", Category = "Mobile & Desktop UI" },
        new TechnologyItem { Title = "Blazor Hybrid", Category = "Web & Desktop" },
        new TechnologyItem { Title = "ASP.NET Core", Category = "Backend & Web APIs" },
        new TechnologyItem { Title = "Flutter", Category = "Cross-Platform UI" },
        new TechnologyItem { Title = "React Native", Category = "Mobile Development" },
        new TechnologyItem { Title = "SwiftUI", Category = "iOS Native UI" },
        new TechnologyItem { Title = "Jetpack Compose", Category = "Android Native UI" },
        new TechnologyItem { Title = "Next.js", Category = "Full-stack React Framework" },
        new TechnologyItem { Title = "Vue.js", Category = "Frontend Web Framework" },
        new TechnologyItem { Title = "Angular", Category = "Enterprise Web Framework" },
        new TechnologyItem { Title = "Node.js", Category = "JavaScript Runtime" },
        new TechnologyItem { Title = "Python / FastAPI", Category = "Backend APIs" },
        new TechnologyItem { Title = "Go / Gin", Category = "Microservices" },
        new TechnologyItem { Title = "Rust / Actix", Category = "Systems & Web APIs" }
    };
}