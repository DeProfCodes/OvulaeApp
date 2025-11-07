using Microsoft.Maui.Controls.Xaml;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.Helpers.UI;

namespace OvulaeApp.Views.Components.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NumericStepper : ContentView
    {
        public NumericStepper()
        {
            InitializeComponent();

            // Force update of visuals from bindable properties
            EntryBorder.Background = EntryBackground;
            EntryField.TextColor = EntryTextColor;

            OuterBorder.Background = OuterBackground;
            OuterBorder.Stroke = OuterStroke;

            DecreaseBorder.Background = DecreaseBackground;
            IncreaseBorder.Background = IncreaseBackground;

            OuterBorder.HeightRequest = 35;
            OuterBorder.WidthRequest = 95;
        }

        // Bindable Properties
        public static readonly BindableProperty OuterBackgroundProperty =
            BindableProperty.Create(
        nameof(OuterBackground),
        typeof(Brush),
        typeof(NumericStepper),
        OvulaeColors.BRUSH.BrushThemeClrLight,
        propertyChanged: OnVisualChanged);

        public static readonly BindableProperty EntryBackgroundProperty =
            BindableProperty.Create(
                nameof(EntryBackground),
                typeof(Brush),
                typeof(NumericStepper),
                OvulaeColors.BRUSH.BrushThemeLightGray,
                propertyChanged: OnVisualChanged);

        public static readonly BindableProperty DecreaseBackgroundProperty =
            BindableProperty.Create(
                nameof(DecreaseBackground),
                typeof(Brush),
                typeof(NumericStepper),
                OvulaeColors.BRUSH.BrushThemeClrLight,
                propertyChanged: OnVisualChanged);

        public static readonly BindableProperty IncreaseBackgroundProperty =
            BindableProperty.Create(
                nameof(IncreaseBackground),
                typeof(Brush),
                typeof(NumericStepper),
                OvulaeColors.BRUSH.BrushThemeClrLight,
                propertyChanged: OnVisualChanged);

        public static readonly BindableProperty EntryTextColorProperty =
            BindableProperty.Create(
                nameof(EntryTextColor),
                typeof(Color),
                typeof(NumericStepper),
                OvulaeColors.COLOR.ThemeClr2,
                propertyChanged: OnVisualChanged);

        public static readonly BindableProperty OuterStrokeProperty =
            BindableProperty.Create(
                nameof(OuterStroke),
                typeof(Brush),
                typeof(NumericStepper),
                OvulaeColors.BRUSH.BrushThemeClrMain,
                propertyChanged: OnVisualChanged);

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(int), typeof(NumericStepper), 0, BindingMode.TwoWay);

        public static readonly BindableProperty MinProperty =
            BindableProperty.Create(nameof(Min), typeof(int), typeof(NumericStepper), 0);

        public static readonly BindableProperty MaxProperty =
            BindableProperty.Create(nameof(Max), typeof(int), typeof(NumericStepper), 999);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(NumericStepper), 14.0);

        public static readonly BindableProperty DecreaseIconSourceProperty =
            BindableProperty.Create(nameof(DecreaseIconSource), typeof(ImageSource), typeof(NumericStepper2),
                ImageSource.FromFile("chev_down.png"));

        public static readonly BindableProperty IncreaseIconSourceProperty =
            BindableProperty.Create(nameof(IncreaseIconSource), typeof(ImageSource), typeof(NumericStepper2),
                ImageSource.FromFile("chev_up.png"));

        public static readonly BindableProperty IconWidthProperty =
           BindableProperty.Create(nameof(IconWidth), typeof(double), typeof(NumericStepper), 15.0);

        public new double IconWidth
        {
            get => (double)GetValue(IconWidthProperty);
            set => SetValue(IconWidthProperty, value);
        }

        public static readonly BindableProperty IconContainerWidthProperty =
           BindableProperty.Create(nameof(IconContainerWidth), typeof(double), typeof(NumericStepper), 30.0);

        public new double IconContainerWidth
        {
            get => (double)GetValue(IconContainerWidthProperty);
            set => SetValue(IconContainerWidthProperty, value);
        }

        public static readonly BindableProperty EntryContainerWidthProperty =
           BindableProperty.Create(nameof(EntryContainerWidth), typeof(double), typeof(NumericStepper), 35.0);

        public new double EntryContainerWidth
        {
            get => (double)GetValue(EntryContainerWidthProperty);
            set => SetValue(EntryContainerWidthProperty, value);
        }

        public static readonly BindableProperty EntryHeightProperty =
           BindableProperty.Create(nameof(EntryHeight), typeof(double), typeof(NumericStepper), 20.0);

        public new double EntryHeight
        {
            get => (double)GetValue(EntryHeightProperty);
            set => SetValue(EntryHeightProperty, value);
        }

        public ImageSource DecreaseIconSource { get => (ImageSource)GetValue(DecreaseIconSourceProperty); set => SetValue(DecreaseIconSourceProperty, value); }
        public ImageSource IncreaseIconSource { get => (ImageSource)GetValue(IncreaseIconSourceProperty); set => SetValue(IncreaseIconSourceProperty, value); }

        public Color EntryTextColor
        {
            get => (Color)GetValue(EntryTextColorProperty);
            set => SetValue(EntryTextColorProperty, value);
        }

        public Brush OuterStroke
        {
            get => (Brush)GetValue(OuterStrokeProperty);
            set => SetValue(OuterStrokeProperty, value);
        }

        public Brush EntryBackground
        {
            get => (Brush)GetValue(EntryBackgroundProperty);
            set => SetValue(EntryBackgroundProperty, value);
        }

        public Brush OuterBackground
        {
            get => (Brush)GetValue(OuterBackgroundProperty);
            set => SetValue(OuterBackgroundProperty, value);
        }

        public Brush DecreaseBackground
        {
            get => (Brush)GetValue(DecreaseBackgroundProperty);
            set => SetValue(DecreaseBackgroundProperty, value);
        }

        public Brush IncreaseBackground
        {
            get => (Brush)GetValue(IncreaseBackgroundProperty);
            set => SetValue(IncreaseBackgroundProperty, value);
        }

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public int Min
        {
            get => (int)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public int Max
        {
            get => (int)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        public new double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        private static void OnVisualChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is NumericStepper stepper)
            {
                stepper.ApplyVisuals();
            }
        }

        private void ApplyVisuals()
        {
            if (OuterBorder != null)
            {
                OuterBorder.Background = OuterBackground;
                OuterBorder.Stroke = OuterStroke;
            }

            if (EntryField != null)
            {
                EntryBorder.Background = EntryBackground;
                EntryField.TextColor = EntryTextColor;
            }

            if (DecreaseBorder != null)
                DecreaseBorder.Background = DecreaseBackground;

            if (IncreaseBorder != null)
                IncreaseBorder.Background = IncreaseBackground;
        }

        public event EventHandler<int>? ValueChanged;

        private void OnDecreaseTapped(object sender, EventArgs e)
        {
            VisualEventsHelper.TapDimEffect(DecreaseBorder);

            if (Value > Min)
                Value--;
            EntryField.Text = Value.ToString();
            ValueChanged?.Invoke(this, Value);
        }

        private void OnIncreaseTapped(object sender, EventArgs e)
        {
            VisualEventsHelper.TapDimEffect(IncreaseBorder);

            if (Value < Max)
                Value++;
            EntryField.Text = Value.ToString();
            ValueChanged?.Invoke(this, Value);
        }

        private void EntryField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(e.NewTextValue, out var newValue))
            {
                if (newValue >= Min && newValue <= Max)
                {
                    Value = newValue;
                    ValueChanged?.Invoke(this, newValue);
                }
            }
        }

        private void EntryField_Unfocused(object sender, FocusEventArgs e)
        {
            EntryField.Text = Value.ToString(); // Reset to valid value
        }
    }
}
