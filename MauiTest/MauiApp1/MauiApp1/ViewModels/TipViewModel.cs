
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiApp1.ViewModels;

public partial class TipViewModel: ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalTip))]
    [NotifyPropertyChangedFor(nameof(TotalAmountPerUser))]
    public partial int TotalAmount { get; set; } = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalTip))]
    [NotifyPropertyChangedFor(nameof(TotalAmountPerUser))]
    public partial int TotalUsers { get; set; } = 1;


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalTip))]
    [NotifyPropertyChangedFor(nameof(TotalAmountPerUser))]
    public partial int TipPercentage { get; set; } = 15;


    public String TotalTip => $"${(TotalAmount * TipPercentage / 100) / TotalUsers}";

    public String TotalAmountPerUser => $"${(TotalAmount + (TotalAmount * TipPercentage / 100)) / TotalUsers}";
}