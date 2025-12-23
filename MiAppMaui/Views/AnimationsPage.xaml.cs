using MiAppMaui.ViewModels;

namespace MiAppMaui.Views;

public partial class AnimationsPage : ContentPage
{
    public AnimationsPage(AnimationsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
