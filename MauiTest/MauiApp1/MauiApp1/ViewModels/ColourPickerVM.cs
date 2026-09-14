using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiApp1.ViewModels
{
    public partial class ColourPickerVM : ObservableObject
    {
        public string ColourRLabel => $"R: {ColourR}";
        public string ColourGLabel => $"G: {ColourG}";
        public string ColourBLabel => $"B: {ColourB}";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ColourRLabel))]
        [NotifyPropertyChangedFor(nameof(GetColourHex))]
        [NotifyPropertyChangedFor(nameof(GetColour))]
        public partial int ColourR { get; set; } = 50;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ColourGLabel))]
        [NotifyPropertyChangedFor(nameof(GetColourHex))]
        [NotifyPropertyChangedFor(nameof(GetColour))]
        public partial int ColourG { get; set; } = 50;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ColourBLabel))]
        [NotifyPropertyChangedFor(nameof(GetColourHex))]
        [NotifyPropertyChangedFor(nameof(GetColour))]
        public partial int ColourB { get; set; } = 50;

        public string GetColourHex => $"#{((int)ColourR):X2}{((int)ColourG):X2}{((int)ColourB):X2}";

        public Color GetColour => Color.FromRgb(ColourR, ColourG, ColourB);
    }
}