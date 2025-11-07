using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System;

namespace OvulaeApp.Views.Components.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomTimePicker : ContentView
    {
        public CustomTimePicker()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty SelectedTimeProperty =
            BindableProperty.Create(nameof(SelectedTime), typeof(TimeSpan), typeof(CustomTimePicker), TimeSpan.Zero, BindingMode.TwoWay);

        public static readonly BindableProperty TimeColorProperty =
            BindableProperty.Create(nameof(TimeColor), typeof(Color), typeof(CustomTimePicker), Colors.Black);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(CustomTimePicker), 16.0);

        public TimeSpan SelectedTime
        {
            get => (TimeSpan)GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }

        public Color TimeColor
        {
            get => (Color)GetValue(TimeColorProperty);
            set => SetValue(TimeColorProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        private void OnIconTapped(object sender, TappedEventArgs e)
        {
            Dispatcher.Dispatch(() =>
            {
                try
                {
                    InnerTimePicker.Focus(); // Still call Focus, but in the UI thread dispatcher
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to open TimePicker: {ex.Message}");
                }
            });
        }
    }
}