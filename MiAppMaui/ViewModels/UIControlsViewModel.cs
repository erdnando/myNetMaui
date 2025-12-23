using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MiAppMaui.ViewModels;

public partial class UIControlsViewModel : BaseViewModel
{
    [ObservableProperty]
    private double sliderValue = 50;

    [ObservableProperty]
    private bool switchValue = true;

    [ObservableProperty]
    private double stepperValue = 5;

    [ObservableProperty]
    private string selectedColor = "Azul";

    [ObservableProperty]
    private string feedbackMessage = "Interactúa con los controles";

    public UIControlsViewModel()
    {
        Title = "Controles UI";
    }

    [RelayCommand]
    private void GradientButtonClicked()
    {
        FeedbackMessage = "🎨 ¡Botón Gradiente presionado!";
    }

    [RelayCommand]
    private void GlassButtonClicked()
    {
        FeedbackMessage = "✨ ¡Botón Glass presionado!";
    }

    [RelayCommand]
    private void NeumorphicButtonClicked()
    {
        FeedbackMessage = "🎭 ¡Botón Neumorphic presionado!";
    }

    [RelayCommand]
    private void ColorSelected(string color)
    {
        SelectedColor = color;
        FeedbackMessage = $"🎨 Color seleccionado: {color}";
    }

    partial void OnSliderValueChanged(double value)
    {
        FeedbackMessage = $"📊 Slider: {value:F0}%";
    }

    partial void OnSwitchValueChanged(bool value)
    {
        FeedbackMessage = value ? "✅ Switch: ON" : "❌ Switch: OFF";
    }

    partial void OnStepperValueChanged(double value)
    {
        FeedbackMessage = $"🔢 Stepper: {value:F0}";
    }
}
