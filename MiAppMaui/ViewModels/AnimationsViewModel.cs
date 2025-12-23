using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MiAppMaui.ViewModels;

public partial class AnimationsViewModel : BaseViewModel
{
    [ObservableProperty]
    private double fadeOpacity = 1.0;

    [ObservableProperty]
    private double slideX = 0;

    [ObservableProperty]
    private double scaleValue = 1.0;

    [ObservableProperty]
    private double rotationAngle = 0;

    [ObservableProperty]
    private bool isAnimating = false;

    public AnimationsViewModel()
    {
        Title = "Animaciones";
    }

    [RelayCommand]
    private async Task FadeAnimation()
    {
        if (IsAnimating) return;
        IsAnimating = true;

        // Fade Out
        for (double i = 1.0; i >= 0; i -= 0.05)
        {
            FadeOpacity = i;
            await Task.Delay(20);
        }

        // Fade In
        for (double i = 0; i <= 1.0; i += 0.05)
        {
            FadeOpacity = i;
            await Task.Delay(20);
        }

        IsAnimating = false;
    }

    [RelayCommand]
    private async Task SlideAnimation()
    {
        if (IsAnimating) return;
        IsAnimating = true;

        // Slide Right
        for (double i = 0; i <= 300; i += 10)
        {
            SlideX = i;
            await Task.Delay(10);
        }

        // Slide Back
        for (double i = 300; i >= 0; i -= 10)
        {
            SlideX = i;
            await Task.Delay(10);
        }

        IsAnimating = false;
    }

    [RelayCommand]
    private async Task ScaleAnimation()
    {
        if (IsAnimating) return;
        IsAnimating = true;

        // Scale Up
        for (double i = 1.0; i <= 2.0; i += 0.05)
        {
            ScaleValue = i;
            await Task.Delay(10);
        }

        // Scale Down
        for (double i = 2.0; i >= 1.0; i -= 0.05)
        {
            ScaleValue = i;
            await Task.Delay(10);
        }

        IsAnimating = false;
    }

    [RelayCommand]
    private async Task RotateAnimation()
    {
        if (IsAnimating) return;
        IsAnimating = true;

        // Rotate 360 degrees
        for (double i = 0; i <= 360; i += 5)
        {
            RotationAngle = i;
            await Task.Delay(5);
        }

        RotationAngle = 0;
        IsAnimating = false;
    }

    [RelayCommand]
    private async Task BounceAnimation()
    {
        if (IsAnimating) return;
        IsAnimating = true;

        // Bounce effect
        double[] bounceHeights = { 0, -100, 0, -70, 0, -40, 0, -20, 0 };
        
        foreach (var height in bounceHeights)
        {
            SlideX = height;
            await Task.Delay(100);
        }

        IsAnimating = false;
    }

    [RelayCommand]
    private async Task ComboAnimation()
    {
        if (IsAnimating) return;
        IsAnimating = true;

        // Animate all properties simultaneously
        var tasks = new List<Task>
        {
            AnimateFade(),
            AnimateScale(),
            AnimateRotate()
        };

        await Task.WhenAll(tasks);
        
        IsAnimating = false;
    }

    private async Task AnimateFade()
    {
        for (double i = 1.0; i >= 0.3; i -= 0.05)
        {
            FadeOpacity = i;
            await Task.Delay(20);
        }
        for (double i = 0.3; i <= 1.0; i += 0.05)
        {
            FadeOpacity = i;
            await Task.Delay(20);
        }
    }

    private async Task AnimateScale()
    {
        for (double i = 1.0; i <= 1.5; i += 0.05)
        {
            ScaleValue = i;
            await Task.Delay(20);
        }
        for (double i = 1.5; i >= 1.0; i -= 0.05)
        {
            ScaleValue = i;
            await Task.Delay(20);
        }
    }

    private async Task AnimateRotate()
    {
        for (double i = 0; i <= 360; i += 10)
        {
            RotationAngle = i;
            await Task.Delay(20);
        }
        RotationAngle = 0;
    }
}
