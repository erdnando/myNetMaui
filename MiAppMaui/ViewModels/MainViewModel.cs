using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MiAppMaui.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    [ObservableProperty]
    private int counter;

    [ObservableProperty]
    private string welcomeMessage = "¡Bienvenido a .NET MAUI!";

    public MainViewModel()
    {
        Title = "Home";
        Counter = 0;
    }

    [RelayCommand]
    private void IncrementCounter()
    {
        Counter++;
        WelcomeMessage = Counter == 1 
            ? $"Has hecho clic {Counter} vez" 
            : $"Has hecho clic {Counter} veces";
    }
}
