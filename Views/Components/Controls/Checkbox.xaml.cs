using Microsoft.Maui.Controls.Xaml;

namespace OvulaeApp.Views.Components.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Checkbox : ContentView
    {
        public static readonly BindableProperty IsCheckedProperty =
            BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(Checkbox), false, propertyChanged: OnCheckedChanged);

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public event EventHandler<bool> CheckedChanged;

        public Checkbox()
        {
            InitializeComponent();
        }

        private static void OnCheckedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var checkbox = (Checkbox)bindable;
            checkbox.CheckImage.IsVisible = (bool)newValue;
            checkbox.CheckedChanged?.Invoke(checkbox, (bool)newValue);
        }

        private void OnCheckboxTapped(object sender, EventArgs e)
        {
            IsChecked = !IsChecked;
        }

        public void CheckUncheck(bool check)
        {
            CheckImage.IsVisible = check;
            IsChecked = check;
            CheckedChanged?.Invoke(this, check);
        }
    }
}
