using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using OvulaeApp.Helpers.Styles;

namespace OvulaeApp.Views.Components.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Switch : ContentView
    {
        public static readonly BindableProperty IsToggledProperty =
            BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(Switch), false, BindingMode.TwoWay, propertyChanged: OnToggleChanged);

        public bool IsToggled
        {
            get => (bool)GetValue(IsToggledProperty);
            set => SetValue(IsToggledProperty, value);
        }

        public Color OnTrackColor { get; set; } = OvulaeColors.COLOR.ThemeClr2; 
        public Color OffTrackColor { get; set; } = OvulaeColors.COLOR.ThemeGray;

        public Color OnThumbColor { get; set; } = Colors.White;
        public Color OffThumbColor { get; set; } = Colors.White;

        private bool _layoutInitialized = false;

        public Switch()
        {
            InitializeComponent();
            this.SizeChanged += OnSwitchSizeChanged;
        }

        private void OnSwitchSizeChanged(object sender, EventArgs e)
        {
            if (!_layoutInitialized && this.Width > 0)
            {
                _layoutInitialized = true;
                UpdateVisuals();
            }
        }

        private void OnToggleTapped(object sender, TappedEventArgs e)
        {
            IsToggled = !IsToggled;
        }

        private static void OnToggleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (Switch)bindable;
            control.UpdateVisuals();
            control.Toggled?.Invoke(control, EventArgs.Empty);
        }

        public event EventHandler Toggled;

        private void UpdateVisuals()
        {
            try
            {
                double targetX = IsToggled ? this.Width - 28 : 0;
                Thumb.TranslateTo(targetX, 0, 150, Easing.CubicInOut);
                SwitchContainer.BackgroundColor = IsToggled ? OnTrackColor : OffTrackColor;
                Thumb.BackgroundColor = IsToggled ? OnThumbColor : OffThumbColor;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bar error: {ex.Message}");
            }
        }
    }
}