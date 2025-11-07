using Microsoft.Maui.Controls;
using OvulaeApp.Helpers.UI;
using System;
using System.Threading.Tasks;

namespace OvulaeApp.Views.Components.Controls;

public partial class CustomProgressBar : ContentView
{
    private double _currentWidth = 0;

    public CustomProgressBar()
    {
        InitializeComponent();
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        // Ensure progress is applied after layout
        SetProgress(Progress, animate: false);
    }

    public static readonly BindableProperty ProgressProperty =
        BindableProperty.Create(
            nameof(Progress),
            typeof(double),
            typeof(CustomProgressBar),
            0.0,
            propertyChanged: OnProgressChanged,
            coerceValue: CoerceProgress);

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    private static object CoerceProgress(BindableObject bindable, object value)
    {
        var progress = (double)value;
        return Math.Max(0, Math.Min(100, progress)); // Clamp 0 to 100
    }

    private static void OnProgressChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomProgressBar)bindable;
        control.SetProgress((double)newValue, animate: true);
    }

    private async void SetProgress(double progress, bool animate)
    {
        try
        {
            // Wait until layout is ready
            if (TrackBar.Width <= 0)
            {
                await Task.Delay(50); // wait briefly for layout
                SetProgress(progress, animate);
                return;
            }

            double targetWidth = (progress / 100.0) * TrackBar.Width;

            if (animate)
            {
                await FillBar.WidthRequestTo(targetWidth, 300, Easing.CubicInOut);
            }
            else
            {
                FillBar.WidthRequest = targetWidth;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Bar error: {ex.Message}");
        }
    }
}
