using MiAppMaui.ViewModels;

namespace MiAppMaui.Views;

public partial class UIControlsPage : ContentPage
{
    public UIControlsPage(UIControlsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
