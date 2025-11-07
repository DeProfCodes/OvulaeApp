using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using OvulaeApp.Helpers.Styles;
using System;

namespace OvulaeApp.Views.Components.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DatePicker : ContentView
    {
        public DatePicker()
        {
            InitializeComponent();

            PickerContainer.Padding = new Thickness(4, 0);
        }

        public static readonly BindableProperty SelectedDateProperty =
            BindableProperty.Create(nameof(SelectedDate), typeof(DateTime), typeof(DatePicker), DateTime.Today, BindingMode.TwoWay);

        public static readonly BindableProperty DateColorProperty =
            BindableProperty.Create(nameof(DateColor), typeof(Color), typeof(DatePicker), Colors.Black);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(DatePicker), 16.0);

        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius), typeof(float), typeof(DatePicker), 8f);

        public static readonly BindableProperty StrokeProperty =
            BindableProperty.Create(nameof(Stroke), typeof(Brush), typeof(DatePicker), Brush.Transparent);

        public static readonly BindableProperty PickerBackgroundProperty =
            BindableProperty.Create(nameof(PickerBackground), typeof(Brush), typeof(DatePicker), OvulaeColors.BRUSH.BrushThemeLightGray);

        public static readonly BindableProperty DropdownIconSourceProperty =
            BindableProperty.Create(nameof(DropdownIconSource), typeof(ImageSource), typeof(NumericStepper2),
                ImageSource.FromFile("chev_down.png"));

        public ImageSource DropdownIconSource 
        { 
            get => (ImageSource)GetValue(DropdownIconSourceProperty); 
            set => SetValue(DropdownIconSourceProperty, value);  
        }

        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        public Brush PickerBackground
        {
            get => (Brush)GetValue(PickerBackgroundProperty);
            set => SetValue(PickerBackgroundProperty, value);
        }

        public static readonly BindableProperty StrokeThicknessProperty =
            BindableProperty.Create(nameof(StrokeThickness), typeof(double), typeof(DatePicker), 1.0);

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public float CornerRadius
        {
            get => (float)GetValue(CornerRadiusProperty);
            set { SetValue(CornerRadiusProperty, value); OnPropertyChanged();  }
        }

        public DateTime SelectedDate
        {
            get => (DateTime)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public Color DateColor
        {
            get => (Color)GetValue(DateColorProperty);
            set => SetValue(DateColorProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        private void OnIconTapped(object sender, TappedEventArgs e)
        {
            InnerDatePicker.Focus(); // This will open the native date picker
        }
    }
}