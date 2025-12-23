using CommunityToolkit.Mvvm.ComponentModel;

namespace MiAppMaui.ViewModels;

/// <summary>
/// ViewModel base que proporciona funcionalidad común para todos los ViewModels
/// </summary>
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    public bool IsNotBusy => !IsBusy;
}
