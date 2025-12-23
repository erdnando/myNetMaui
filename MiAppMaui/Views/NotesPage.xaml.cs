using MiAppMaui.ViewModels;
using CommunityToolkit.Maui;

namespace MiAppMaui.Views;

public partial class NotesPage : ContentPage
{
    public NotesPage(NotesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
