using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.Shapes;
#if IOS || MACCATALYST
using UIKit;
#endif

namespace MauiApp1.CustomControls;

public partial class SearchableDropdown : ContentView
{
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(
            nameof(ItemsSource),
            typeof(System.Collections.IEnumerable),
            typeof(SearchableDropdown),
            null,
            propertyChanged: OnItemsSourceChanged);

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(
            nameof(SelectedItem),
            typeof(object),
            typeof(SearchableDropdown),
            null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnSelectedItemChanged);

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(
            nameof(Placeholder),
            typeof(string),
            typeof(SearchableDropdown),
            "Select an item...",
            propertyChanged: OnPlaceholderChanged);

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(SearchableDropdown),
            string.Empty,
            propertyChanged: OnTitleChanged);

    public static readonly BindableProperty DisplayMemberPathProperty =
        BindableProperty.Create(
            nameof(DisplayMemberPath),
            typeof(string),
            typeof(SearchableDropdown),
            string.Empty,
            propertyChanged: OnDisplayMemberPathChanged);

    public System.Collections.IEnumerable? ItemsSource
    {
        get => (System.Collections.IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string DisplayMemberPath
    {
        get => (string)GetValue(DisplayMemberPathProperty);
        set => SetValue(DisplayMemberPathProperty, value);
    }

    public Border ControlSelectorBorder => SelectorBorder;

    public SearchableDropdownViewModel ViewModel { get; } = new();

    private SearchableDropdownOverlayView? _activeOverlay;

    public SearchableDropdown()
    {
        InitializeComponent();
        ViewModel.Placeholder = Placeholder;
        ViewModel.Title = Title;
        ViewModel.DisplayMemberPath = DisplayMemberPath;
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        ViewModel.ToggleDropdownRequested += OnToggleDropdownRequested;
    }

    private void OnToggleDropdownRequested(object? sender, EventArgs e)
    {
        if (ViewModel.IsDropdownOpen)
        {
            OpenOverlayPopup();
        }
        else
        {
            CloseOverlayPopup();
        }
    }

    public void OpenOverlayPopup()
    {
        CloseOverlayPopup();

        var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (mainPage == null) return;

        Grid? rootGrid = GetRootGrid(mainPage);
        if (rootGrid == null) return;

        var targetBounds = GetBoundsRelativeToRoot(SelectorBorder, rootGrid);
        double targetY = targetBounds.Y;
        double targetX = targetBounds.X;
        double targetHeight = SelectorBorder.Height > 0 ? SelectorBorder.Height : 48;
        double targetWidth = targetBounds.Width > 0 ? targetBounds.Width : SelectorBorder.Width;

        _activeOverlay = new SearchableDropdownOverlayView(this, ViewModel, targetX, targetY, targetWidth, targetHeight, rootGrid);
        Grid.SetRowSpan(_activeOverlay, 99);
        Grid.SetColumnSpan(_activeOverlay, 99);
        rootGrid.Children.Add(_activeOverlay);
    }

    private Rect GetBoundsRelativeToRoot(VisualElement element, VisualElement root)
    {
        var elementBounds = GetAbsoluteBounds(element);
        var rootBounds = GetAbsoluteBounds(root);

        if (elementBounds.Width > 0 && rootBounds.Width > 0)
        {
            double relativeX = elementBounds.X - rootBounds.X;
            double relativeY = elementBounds.Y - rootBounds.Y;
            return new Rect(
                relativeX,
                relativeY,
                elementBounds.Width > 0 ? elementBounds.Width : (element.Width > 0 ? element.Width : 0),
                elementBounds.Height > 0 ? elementBounds.Height : (element.Height > 0 ? element.Height : 48));
        }

        return new Rect(
            GetXRelativeToContainer(element, root),
            GetYRelativeToContainer(element, root),
            element.Width > 0 ? element.Width : 0,
            element.Height > 0 ? element.Height : 48);
    }

    private Rect GetAbsoluteBounds(VisualElement element)
    {
        if (element == null) return Rect.Zero;

#if IOS || MACCATALYST
        if (element.Handler?.PlatformView is UIView nativeView)
        {
            var window = nativeView.Window;
            if (window != null)
            {
                var boundsInWindow = nativeView.ConvertRectToView(nativeView.Bounds, window);
                return new Rect(boundsInWindow.X, boundsInWindow.Y, boundsInWindow.Width, boundsInWindow.Height);
            }
        }
#elif ANDROID
        if (element.Handler?.PlatformView is Android.Views.View nativeView)
        {
            int[] location = new int[2];
            nativeView.GetLocationInWindow(location);
            var displayMetrics = nativeView.Context?.Resources?.DisplayMetrics;
            float density = displayMetrics?.Density ?? 1f;
            return new Rect(location[0] / density, location[1] / density, nativeView.Width / density, nativeView.Height / density);
        }
#endif

        return Rect.Zero;
    }

    public void CloseOverlayPopup()
    {
        if (_activeOverlay != null)
        {
            _activeOverlay.Dismiss();
            _activeOverlay = null;
        }
    }

    private Grid? GetRootGrid(Page? page)
    {
        if (page == null) return null;

        if (page is NavigationPage navPage)
        {
            return GetRootGrid(navPage.CurrentPage);
        }
        if (page is Shell shell)
        {
            return GetRootGrid(shell.CurrentPage);
        }
        if (page is TabbedPage tabbedPage)
        {
            return GetRootGrid(tabbedPage.CurrentPage);
        }
        if (page is FlyoutPage flyoutPage)
        {
            return GetRootGrid(flyoutPage.Detail);
        }
        if (page is ContentPage contentPage)
        {
            if (contentPage.Content is Grid g) return g;
            if (contentPage.Content != null)
            {
                var rootGrid = new Grid();
                var originalContent = contentPage.Content;
                contentPage.Content = null;
                rootGrid.Children.Add(originalContent);
                contentPage.Content = rootGrid;
                return rootGrid;
            }
        }
        return null;
    }

    public double GetXRelativeToContainer(VisualElement element, VisualElement container)
    {
        double x = 0;
        VisualElement? current = element;
        while (current != null && current != container && !(current is Page))
        {
            x += current.X;
            if (current is ScrollView sv)
            {
                x -= sv.ScrollX;
            }
            current = current.Parent as VisualElement;
        }
        return x;
    }

    public double GetYRelativeToContainer(VisualElement element, VisualElement container)
    {
        double y = 0;
        VisualElement? current = element;
        while (current != null && current != container && !(current is Page))
        {
            y += current.Y;
            if (current is ScrollView sv)
            {
                y -= sv.ScrollY;
            }
            current = current.Parent as VisualElement;
        }
        return y;
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.SelectedItem))
        {
            if (!Equals(SelectedItem, ViewModel.SelectedItem))
            {
                SelectedItem = ViewModel.SelectedItem;
            }
        }
    }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SearchableDropdown control)
        {
            control.ViewModel.UpdateItemsSource(newValue as System.Collections.IEnumerable);
        }
    }

    private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SearchableDropdown control)
        {
            if (!Equals(control.ViewModel.SelectedItem, newValue))
            {
                control.ViewModel.SelectedItem = newValue;
            }
        }
    }

    private static void OnPlaceholderChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SearchableDropdown control)
        {
            control.ViewModel.Placeholder = newValue as string ?? "Select an item...";
        }
    }

    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SearchableDropdown control)
        {
            control.ViewModel.Title = newValue as string ?? string.Empty;
        }
    }

    private static void OnDisplayMemberPathChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SearchableDropdown control)
        {
            control.ViewModel.DisplayMemberPath = newValue as string ?? string.Empty;
        }
    }
}

public partial class SearchableDropdownViewModel : ObservableObject
{
    public event EventHandler? ToggleDropdownRequested;

    private IEnumerable<object>? _rawItemsSource;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isDropdownOpen;

    [ObservableProperty]
    private object? _selectedItem;

    [ObservableProperty]
    private string _placeholder = "Select an item...";

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _displayMemberPath = string.Empty;

    public ObservableCollection<object> FilteredItems { get; } = new();

    public string SelectedItemDisplayText
    {
        get
        {
            if (SelectedItem == null) return Placeholder;
            string display = GetItemDisplayText(SelectedItem);
            return string.IsNullOrEmpty(display) ? Placeholder : display;
        }
    }

    public bool HasSelectedItem => SelectedItem != null;

    public bool HasTitle => !string.IsNullOrWhiteSpace(Title);

    public bool IsEmptyResult => FilteredItems.Count == 0;

    public bool HasSearchText => !string.IsNullOrEmpty(SearchText);

    partial void OnSearchTextChanged(string value)
    {
        OnPropertyChanged(nameof(HasSearchText));
        ApplyFilter();
    }

    partial void OnSelectedItemChanged(object? value)
    {
        OnPropertyChanged(nameof(SelectedItemDisplayText));
        OnPropertyChanged(nameof(HasSelectedItem));
    }

    partial void OnPlaceholderChanged(string value)
    {
        OnPropertyChanged(nameof(SelectedItemDisplayText));
    }

    partial void OnTitleChanged(string value)
    {
        OnPropertyChanged(nameof(HasTitle));
    }

    partial void OnDisplayMemberPathChanged(string value)
    {
        OnPropertyChanged(nameof(SelectedItemDisplayText));
        ApplyFilter();
    }

    public void UpdateItemsSource(System.Collections.IEnumerable? items)
    {
        _rawItemsSource = items?.Cast<object>();
        ApplyFilter();
    }

    public void ApplyFilter()
    {
        FilteredItems.Clear();
        if (_rawItemsSource == null)
        {
            OnPropertyChanged(nameof(IsEmptyResult));
            return;
        }

        string query = SearchText?.Trim() ?? string.Empty;
        foreach (var item in _rawItemsSource)
        {
            if (item == null) continue;
            string display = GetItemDisplayText(item);
            if (string.IsNullOrEmpty(query) || display.Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                FilteredItems.Add(item);
            }
        }
        OnPropertyChanged(nameof(IsEmptyResult));
    }

    public string GetItemDisplayText(object? item)
    {
        if (item == null) return string.Empty;
        if (!string.IsNullOrEmpty(DisplayMemberPath))
        {
            var prop = item.GetType().GetProperty(DisplayMemberPath, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                var val = prop.GetValue(item);
                return val?.ToString() ?? string.Empty;
            }
        }
        return item.ToString() ?? string.Empty;
    }

    [RelayCommand]
    private void ToggleDropdown()
    {
        IsDropdownOpen = !IsDropdownOpen;
        if (IsDropdownOpen)
        {
            SearchText = string.Empty;
            ApplyFilter();
        }
        ToggleDropdownRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void SelectItem(object? item)
    {
        SelectedItem = item;
        IsDropdownOpen = false;
        ToggleDropdownRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void ClearSelection()
    {
        SelectedItem = null;
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SearchText = string.Empty;
    }
}

public class SearchableDropdownOverlayView : Grid
{
    private readonly SearchableDropdown _parentDropdown;
    private readonly SearchableDropdownViewModel _viewModel;
    private readonly Grid _targetRootGrid;

    public SearchableDropdownOverlayView(SearchableDropdown parentDropdown, SearchableDropdownViewModel viewModel, double targetX, double targetY, double targetWidth, double targetHeight, Grid targetRootGrid)
    {
        _parentDropdown = parentDropdown;
        _viewModel = viewModel;
        _targetRootGrid = targetRootGrid;
        BindingContext = _viewModel;

        ZIndex = 99999;
        InputTransparent = false;
        HorizontalOptions = LayoutOptions.Fill;
        VerticalOptions = LayoutOptions.Fill;

        var bgView = new BoxView
        {
            Color = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        var bgTap = new TapGestureRecognizer();
        bgTap.Command = new Command(Dismiss);
        bgView.GestureRecognizers.Add(bgTap);
        Children.Add(bgView);

        double topMargin = targetY + targetHeight + 2;

        var popupCard = new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(12) },
            Stroke = Color.FromArgb("#D1D5DB"),
            StrokeThickness = 1,
            BackgroundColor = Colors.White,
            Padding = new Thickness(12),
            VerticalOptions = LayoutOptions.Start,
            Shadow = new Shadow
            {
                Brush = Colors.Black,
                Offset = new Point(0, 6),
                Radius = 12,
                Opacity = 0.4f
            }
        };

        if (targetWidth > 0)
        {
            popupCard.HorizontalOptions = LayoutOptions.Start;
            popupCard.WidthRequest = targetWidth;
            popupCard.Margin = new Thickness(targetX, topMargin, 0, 20);
        }
        else
        {
            popupCard.HorizontalOptions = LayoutOptions.Fill;
            popupCard.Margin = new Thickness(30, topMargin, 30, 20);
        }

        var stackLayout = new VerticalStackLayout { Spacing = 8 };

        var searchBorder = new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(8) },
            Stroke = Color.FromArgb("#CCCCCC"),
            BackgroundColor = Color.FromArgb("#F9F9FC"),
            Padding = new Thickness(8, 2),
            HorizontalOptions = LayoutOptions.Fill
        };

        var searchGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            },
            ColumnSpacing = 6
        };

        var searchIcon = new Label
        {
            Text = "🔍",
            FontSize = 14,
            TextColor = Color.FromArgb("#9CA3AF"),
            VerticalOptions = LayoutOptions.Center
        };
        searchGrid.Children.Add(searchIcon);
        Grid.SetColumn(searchIcon, 0);

        var entry = new Entry
        {
            Placeholder = "Type to search...",
            PlaceholderColor = Color.FromArgb("#9CA3AF"),
            FontSize = 14,
            TextColor = Color.FromArgb("#1F2937"),
            BackgroundColor = Colors.Transparent,
            VerticalOptions = LayoutOptions.Center
        };
        entry.SetBinding(Entry.TextProperty, new Binding(nameof(_viewModel.SearchText), BindingMode.TwoWay));
        searchGrid.Children.Add(entry);
        Grid.SetColumn(entry, 1);

        var clearSearchBtn = new Label
        {
            Text = "✕",
            FontSize = 14,
            TextColor = Color.FromArgb("#9CA3AF"),
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(6)
        };
        clearSearchBtn.SetBinding(VisualElement.IsVisibleProperty, nameof(_viewModel.HasSearchText));
        var clearSearchTap = new TapGestureRecognizer();
        clearSearchTap.Command = _viewModel.ClearSearchCommand;
        clearSearchBtn.GestureRecognizers.Add(clearSearchTap);
        searchGrid.Children.Add(clearSearchBtn);
        Grid.SetColumn(clearSearchBtn, 2);

        searchBorder.Content = searchGrid;
        stackLayout.Children.Add(searchBorder);

        var collectionView = new CollectionView
        {
            HeightRequest = 220,
            SelectionMode = SelectionMode.None
        };
        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(_viewModel.FilteredItems));

        collectionView.ItemTemplate = new DataTemplate(() =>
        {
            var itemBorder = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(6) },
                Stroke = Colors.Transparent,
                BackgroundColor = Colors.Transparent,
                Padding = new Thickness(10, 10),
                Margin = new Thickness(0, 1)
            };

            var itemGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };

            var itemText = new Label
            {
                FontSize = 14,
                TextColor = Color.FromArgb("#374151"),
                VerticalOptions = LayoutOptions.Center
            };
            itemText.SetBinding(Label.TextProperty, new Binding(".", converter: new ItemDisplayTextConverter(), converterParameter: _viewModel.DisplayMemberPath));
            itemGrid.Children.Add(itemText);

            var itemTap = new TapGestureRecognizer();
            itemTap.Command = new Command<object>((item) =>
            {
                _viewModel.SelectItemCommand.Execute(item);
                Dismiss();
            });
            itemTap.SetBinding(TapGestureRecognizer.CommandParameterProperty, new Binding("."));
            itemBorder.GestureRecognizers.Add(itemTap);

            itemBorder.Content = itemGrid;
            return itemBorder;
        });

        collectionView.EmptyView = new Label
        {
            Text = "No matching items found",
            FontSize = 13,
            TextColor = Color.FromArgb("#9CA3AF"),
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 15)
        };

        stackLayout.Children.Add(collectionView);
        popupCard.Content = stackLayout;
        Children.Add(popupCard);
    }

    public void Dismiss()
    {
        _viewModel.IsDropdownOpen = false;
        if (_targetRootGrid.Children.Contains(this))
        {
            _targetRootGrid.Children.Remove(this);
        }
    }
}

public class ItemDisplayTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) return string.Empty;
        string path = parameter as string ?? string.Empty;
        if (!string.IsNullOrEmpty(path))
        {
            var prop = value.GetType().GetProperty(path, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                return prop.GetValue(value)?.ToString() ?? string.Empty;
            }
        }
        return value.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}