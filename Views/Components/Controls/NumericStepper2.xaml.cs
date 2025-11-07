using System.ComponentModel;
using System.Runtime.CompilerServices;
using OvulaeApp.Helpers.Styles;

namespace OvulaeApp.Views.Components.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NumericStepper2 : ContentView, INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public NumericStepper2()
        {
            InitializeComponent();
            this.BindingContext = this;

            ApplyVisuals();

        }

        private void ApplyVisuals()
        {
            EntryField.Background = EntryBackground;
            EntryField.TextColor = EntryTextColor;
            EntryField.FontSize = FontSize;
            EntryField.FontAttributes = FontAttributes;
            EntryField.FontFamily = FontFamily;

            // Force style updates if SameStyleForButtons
            if (SameStyleForButtons)
            {
                RightButtonBackground = DecreaseButtonBackground;
                RightButtonStroke = DecreaseButtonStroke;
            }

            OnPropertyChanged(nameof(DecreaseButtonBackground));
            OnPropertyChanged(nameof(DecreaseButtonStroke));
            OnPropertyChanged(nameof(RightButtonBackground));
            OnPropertyChanged(nameof(RightButtonStroke));
            OnPropertyChanged(nameof(OuterStrokeColor));
            OnPropertyChanged(nameof(OuterBackgroundColor));
            OnPropertyChanged(nameof(EntryBackground));
            OnPropertyChanged(nameof(EntryTextColor));
            OnPropertyChanged(nameof(FontSize));
            OnPropertyChanged(nameof(Value));
        }

        private static void OnVisualChanged(BindableObject bindable, object oldVal, object newVal)
        {
            if (bindable is NumericStepper2 stepper)
                stepper.ApplyVisuals();
        }

        // === Core Properties ===
        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(int), typeof(NumericStepper2), 0, BindingMode.TwoWay, propertyChanged: OnVisualChanged);

        public static readonly BindableProperty MinProperty =
            BindableProperty.Create(nameof(Min), typeof(int), typeof(NumericStepper2), 0);

        public static readonly BindableProperty MaxProperty =
            BindableProperty.Create(nameof(Max), typeof(int), typeof(NumericStepper2), 999);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(NumericStepper2), 14.0);

        public static readonly BindableProperty SameStyleForButtonsProperty =
            BindableProperty.Create(nameof(SameStyleForButtons), typeof(bool), typeof(NumericStepper2), false, propertyChanged: OnVisualChanged);

        // === Visual Defaults ===
        public static readonly BindableProperty DecreaseButtonBackgroundProperty =
            BindableProperty.Create(nameof(DecreaseButtonBackground), typeof(Brush), typeof(NumericStepper2),
                OvulaeColors.BRUSH.BrushThemeClrLight, propertyChanged: OnVisualChanged);

        public static readonly BindableProperty DecreaseButtonStrokeProperty =
            BindableProperty.Create(nameof(DecreaseButtonStroke), typeof(Brush), typeof(NumericStepper2),
                Brush.Transparent, propertyChanged: OnVisualChanged);

        public static readonly BindableProperty RightButtonBackgroundProperty =
            BindableProperty.Create(nameof(RightButtonBackground), typeof(Brush), typeof(NumericStepper2),
                OvulaeColors.BRUSH.BrushThemeClrLight, propertyChanged: OnVisualChanged);

        public static readonly BindableProperty RightButtonStrokeProperty =
            BindableProperty.Create(nameof(RightButtonStroke), typeof(Brush), typeof(NumericStepper2),
                Brush.Transparent, propertyChanged: OnVisualChanged);

        public static readonly BindableProperty EntryBackgroundProperty =
            BindableProperty.Create(nameof(EntryBackground), typeof(Brush), typeof(NumericStepper2),
                OvulaeColors.BRUSH.BrushThemeLightGray, propertyChanged: OnVisualChanged);

        public static readonly BindableProperty EntryTextColorProperty =
            BindableProperty.Create(nameof(EntryTextColor), typeof(Color), typeof(NumericStepper2),
                OvulaeColors.COLOR.ThemeClr2, propertyChanged: OnVisualChanged);

        public static readonly BindableProperty OuterBackgroundColorProperty =
            BindableProperty.Create(nameof(OuterBackgroundColor), typeof(Brush), typeof(NumericStepper2),
                OvulaeColors.BRUSH.BrushThemeClrLight, propertyChanged: OnVisualChanged);

        public static readonly BindableProperty OuterStrokeColorProperty =
            BindableProperty.Create(nameof(OuterStrokeColor), typeof(Brush), typeof(NumericStepper2),
                OvulaeColors.BRUSH.BrushThemeClrMain, propertyChanged: OnVisualChanged);

        public static readonly BindableProperty FontFamilyProperty =
            BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(NumericStepper2), "Arial Black");

        public static readonly BindableProperty FontAttributesProperty =
            BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(NumericStepper2), FontAttributes.Bold);

        public static readonly BindableProperty ButtonPaddingProperty =
            BindableProperty.Create(nameof(ButtonPadding), typeof(Thickness), typeof(NumericStepper2), new Thickness(5));

        public static readonly BindableProperty RightButtonPaddingProperty =
            BindableProperty.Create(nameof(RightButtonPadding), typeof(Thickness), typeof(NumericStepper2), new Thickness(5));

        public static readonly BindableProperty ButtonWidthProperty =
            BindableProperty.Create(nameof(ButtonWidth), typeof(double), typeof(NumericStepper2), 30.0);

        public static readonly BindableProperty EntryPaddingProperty =
            BindableProperty.Create(nameof(EntryPadding), typeof(Thickness), typeof(NumericStepper2), new Thickness(0, 5));

        public static readonly BindableProperty EntryWidthProperty =
            BindableProperty.Create(nameof(EntryWidth), typeof(double), typeof(NumericStepper2), 35.0);

        public static readonly BindableProperty EntryStrokeColorProperty =
            BindableProperty.Create(nameof(EntryStrokeColor), typeof(Brush), typeof(NumericStepper2), Brush.Transparent);

        public static readonly BindableProperty OuterHeightProperty =
            BindableProperty.Create(nameof(OuterHeight), typeof(double), typeof(NumericStepper2), 35.0);

        public static readonly BindableProperty OuterWidthProperty =
            BindableProperty.Create(nameof(OuterWidth), typeof(double), typeof(NumericStepper2), 95.0);

        public static readonly BindableProperty DecreaseIconSourceProperty =
            BindableProperty.Create(nameof(DecreaseIconSource), typeof(ImageSource), typeof(NumericStepper2),
                ImageSource.FromFile("chev_down.png"));

        public static readonly BindableProperty IncreaseIconSourceProperty =
            BindableProperty.Create(nameof(IncreaseIconSource), typeof(ImageSource), typeof(NumericStepper2),
                ImageSource.FromFile("chev_up.png"));

        public static readonly BindableProperty IconWidthProperty =
            BindableProperty.Create(nameof(IconWidth), typeof(double), typeof(NumericStepper2), 15.0);

        // === Property Accessors ===
        public int Value { get => (int)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public int Min { get => (int)GetValue(MinProperty); set => SetValue(MinProperty, value); }
        public int Max { get => (int)GetValue(MaxProperty); set => SetValue(MaxProperty, value); }
        public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }
        public bool SameStyleForButtons { get => (bool)GetValue(SameStyleForButtonsProperty); set => SetValue(SameStyleForButtonsProperty, value); }

        public Brush DecreaseButtonBackground { get => (Brush)GetValue(DecreaseButtonBackgroundProperty); set => SetValue(DecreaseButtonBackgroundProperty, value); }
        public Brush DecreaseButtonStroke { get => (Brush)GetValue(DecreaseButtonStrokeProperty); set => SetValue(DecreaseButtonStrokeProperty, value); }
        public Brush RightButtonBackground { get => (Brush)GetValue(RightButtonBackgroundProperty); set => SetValue(RightButtonBackgroundProperty, value); }
        public Brush RightButtonStroke { get => (Brush)GetValue(RightButtonStrokeProperty); set => SetValue(RightButtonStrokeProperty, value); }

        public Brush EntryBackground { get => (Brush)GetValue(EntryBackgroundProperty); set => SetValue(EntryBackgroundProperty, value); }
        public Color EntryTextColor { get => (Color)GetValue(EntryTextColorProperty); set => SetValue(EntryTextColorProperty, value); }

        public Brush OuterBackgroundColor { get => (Brush)GetValue(OuterBackgroundColorProperty); set => SetValue(OuterBackgroundColorProperty, value); }
        public Brush OuterStrokeColor { get => (Brush)GetValue(OuterStrokeColorProperty); set => SetValue(OuterStrokeColorProperty, value); }

        public string FontFamily { get => (string)GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }
        public FontAttributes FontAttributes { get => (FontAttributes)GetValue(FontAttributesProperty); set => SetValue(FontAttributesProperty, value); }

        public Thickness ButtonPadding { get => (Thickness)GetValue(ButtonPaddingProperty); set => SetValue(ButtonPaddingProperty, value); }
        public Thickness RightButtonPadding { get => (Thickness)GetValue(RightButtonPaddingProperty); set => SetValue(RightButtonPaddingProperty, value); }

        public double ButtonWidth { get => (double)GetValue(ButtonWidthProperty); set => SetValue(ButtonWidthProperty, value); }
        public Thickness EntryPadding { get => (Thickness)GetValue(EntryPaddingProperty); set => SetValue(EntryPaddingProperty, value); }
        public double EntryWidth { get => (double)GetValue(EntryWidthProperty); set => SetValue(EntryWidthProperty, value); }
        public Brush EntryStrokeColor { get => (Brush)GetValue(EntryStrokeColorProperty); set => SetValue(EntryStrokeColorProperty, value); }
        public double OuterHeight { get => (double)GetValue(OuterHeightProperty); set => SetValue(OuterHeightProperty, value); }
        public double OuterWidth { get => (double)GetValue(OuterWidthProperty); set => SetValue(OuterWidthProperty, value); }

        public ImageSource DecreaseIconSource { get => (ImageSource)GetValue(DecreaseIconSourceProperty); set => SetValue(DecreaseIconSourceProperty, value); }
        public ImageSource IncreaseIconSource { get => (ImageSource)GetValue(IncreaseIconSourceProperty); set => SetValue(IncreaseIconSourceProperty, value); }
        public double IconWidth { get => (double)GetValue(IconWidthProperty); set => SetValue(IconWidthProperty, value); }

        // === Event ===
        public event EventHandler<int>? ValueChanged;

        private void OnDecreaseTapped(object sender, EventArgs e)
        {
            if (Value > Min)
                Value--;

            EntryField.Text = Value.ToString();
            ValueChanged?.Invoke(this, Value);
        }

        private void OnIncreaseTapped(object sender, EventArgs e)
        {
            if (Value < Max)
                Value++;

            EntryField.Text = Value.ToString();
            ValueChanged?.Invoke(this, Value);
        }

        private void EntryField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(e.NewTextValue, out var newValue) && newValue >= Min && newValue <= Max)
            {
                Value = newValue;
                ValueChanged?.Invoke(this, newValue);
            }
            else
            {
                EntryField.Text = Value.ToString(); // Revert invalid input
            }
        }

        private void EntryField_Unfocused(object sender, FocusEventArgs e)
        {
            EntryField.Text = Value.ToString();
        }
    }
}