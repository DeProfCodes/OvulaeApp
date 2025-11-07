using System.Diagnostics;
using System.Windows.Input;
using OvulaeApp.Helpers.UI;

namespace OvulaeApp.Views.Components.Controls.Buttons
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GradientButton : ContentView
    {
        public GradientButton()
        {
            try
            {
                InitializeComponent();

                this.Loaded += (s, e) =>
                {
                    ApplyIsFullWidth();
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GradientButton Init Error: {ex.Message}");
                //throw;
            }
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(GradientButton), default(string), propertyChanged: ForceUpdate);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(GradientButton), 12.0, propertyChanged: ForceUpdate);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty FontAttributesProperty =
            BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(GradientButton), FontAttributes.None, propertyChanged: ForceUpdate);

        public FontAttributes FontAttributes
        {
            get => (FontAttributes)GetValue(FontAttributesProperty);
            set => SetValue(FontAttributesProperty, value);
        }

        public static readonly BindableProperty TapCommandProperty =
            BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(GradientButton), null, propertyChanged: ForceUpdate);

        

        public static readonly BindableProperty GradientColorLeftProperty =
            BindableProperty.Create(nameof(GradientColorLeft), typeof(Color), typeof(GradientButton), Colors.Blue, propertyChanged: ForceUpdate);

        public Color GradientColorLeft
        {
            get => (Color)GetValue(GradientColorLeftProperty);
            set => SetValue(GradientColorLeftProperty, value);
        }

        public static readonly BindableProperty GradientColorRightProperty =
            BindableProperty.Create(nameof(GradientColorRight), typeof(Color), typeof(GradientButton), Colors.Purple, propertyChanged: ForceUpdate);

        public Color GradientColorRight
        {
            get => (Color)GetValue(GradientColorRightProperty);
            set => SetValue(GradientColorRightProperty, value);
        }

        public static readonly BindableProperty ButtonTextColorProperty =
            BindableProperty.Create(nameof(ButtonTextColor), typeof(Color), typeof(GradientButton), Colors.White, propertyChanged: ForceUpdate);

        public Color ButtonTextColor
        {
            get => (Color)GetValue(ButtonTextColorProperty);
            set => SetValue(ButtonTextColorProperty, value);
        }

        public static readonly BindableProperty StrokeProperty =
            BindableProperty.Create(nameof(Stroke), typeof(Brush), typeof(GradientButton), Brush.Transparent, propertyChanged: ForceUpdate);

        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        public static readonly BindableProperty StrokeThicknessProperty =
            BindableProperty.Create(nameof(StrokeThickness), typeof(double), typeof(GradientButton), 1.0, propertyChanged: ForceUpdate);

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius), typeof(float), typeof(GradientButton), 25f, propertyChanged: ForceUpdate);

        public float CornerRadius
        {
            get => (float)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly BindableProperty HasImageIconProperty =
            BindableProperty.Create(nameof(HasImageIcon), typeof(bool), typeof(GradientButton), false, propertyChanged: ForceUpdate);

        public bool HasImageIcon
        {
            get => (bool)GetValue(HasImageIconProperty);
            set => SetValue(HasImageIconProperty, value);
        }

        public static readonly BindableProperty ImageIconHeightProperty =
            BindableProperty.Create(nameof(ImageIconHeight), typeof(double), typeof(GradientButton), 0.0, propertyChanged: ForceUpdate);

        public double ImageIconHeight
        {
            get
            {
                var height = (double)GetValue(ImageIconHeightProperty);
                return height > 0 ? height : 14;
            }
            set => SetValue(ImageIconHeightProperty, value);
        }

        private double CalculateDefaultImageHeight()
        {
            return FontSize + 2; 
        }

        public static readonly BindableProperty ImageIconWidthProperty =
            BindableProperty.Create(nameof(ImageIconWidth), typeof(double), typeof(GradientButton), 14.0, propertyChanged: ForceUpdate);

        public double ImageIconWidth
        {
            get => (double)GetValue(ImageIconWidthProperty);
            set => SetValue(ImageIconWidthProperty, value);
        }

        public static readonly BindableProperty ImageIconSourceProperty =
            BindableProperty.Create(
                nameof(ImageIconSource),
                typeof(ImageSource),
                typeof(GradientButton),
                default(ImageSource), // don't use FromFile or any resource loading here
                propertyChanged: ForceUpdate);

        public ImageSource ImageIconSource
        {
            get => (ImageSource)GetValue(ImageIconSourceProperty);
            set => SetValue(ImageIconSourceProperty, value);
        }

        public static readonly BindableProperty ShadowBrushProperty =
            BindableProperty.Create(nameof(ShadowBrush), typeof(Brush), typeof(GradientButton), Brush.Black, propertyChanged: ForceUpdate);

        public static readonly BindableProperty ShadowOpacityProperty =
            BindableProperty.Create(nameof(ShadowOpacity), typeof(double), typeof(GradientButton), 0.0, propertyChanged: ForceUpdate);

        // Public accessors

        public static readonly BindableProperty IsButtonEnabledProperty =
            BindableProperty.Create(
                nameof(IsButtonEnabled),
                typeof(bool),
                typeof(GradientButton),
                true,
                propertyChanged: IsButtonEnabledChanged);

        public bool IsButtonEnabled
        {
            get => (bool)GetValue(IsButtonEnabledProperty);
            set => SetValue(IsButtonEnabledProperty, value);
        }

        private static void IsButtonEnabledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is GradientButton button && newValue is bool isEnabled)
            {
                button.MainBorder.IsEnabled = isEnabled; 
                button.MainBorder.Opacity = isEnabled ? 1.0 : 0.3;
            }
        }

        private static void ForceUpdate(BindableObject bindable, object oldValue, object newValue)
        {
            // Forces re-render or visual refresh if needed in the future
            // For now, just placeholder
        }


        public Brush ShadowBrush
        {
            get => (Brush)GetValue(ShadowBrushProperty);
            set => SetValue(ShadowBrushProperty, value);
        }

        public double ShadowOpacity
        {
            get => (double)GetValue(ShadowOpacityProperty);
            set => SetValue(ShadowOpacityProperty, value);
        }


        public ICommand TapCommand
        {
            get => (ICommand)GetValue(TapCommandProperty);
            set => SetValue(TapCommandProperty, value);
        }

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(GradientButton), null);

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly BindableProperty IsFullWidthProperty =
            BindableProperty.Create(nameof(IsFullWidth), typeof(bool), typeof(GradientButton), false, propertyChanged: OnLayoutChanged);

        public bool IsFullWidth
        {
            get => (bool)GetValue(IsFullWidthProperty);
            set => SetValue(IsFullWidthProperty, value);
        }

        private static void OnLayoutChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is GradientButton button)
                button.ApplyIsFullWidth();
        }

        private void ApplyIsFullWidth()
        {
            if (MainBorder == null)
                return;

            if (IsFullWidth)
            {
                MainBorder.HorizontalOptions = LayoutOptions.FillAndExpand;
                MainBorder.WidthRequest = -1;
            }
            else
            {
                MainBorder.HorizontalOptions = this.HorizontalOptions;
                MainBorder.WidthRequest = this.WidthRequest;
            }
        }

        public static readonly BindableProperty ButtonPaddingProperty =
            BindableProperty.Create(nameof(ButtonPadding), typeof(Thickness), typeof(CustomEntry), new Thickness(10,5), propertyChanged: OnLayoutChanged);

        public Thickness ButtonPadding
        {
            get => (Thickness)GetValue(ButtonPaddingProperty);
            set => SetValue(ButtonPaddingProperty, value);
        }

        public static readonly BindableProperty IconMarginProperty =
            BindableProperty.Create(nameof(IconMargin), typeof(Thickness), typeof(CustomEntry), new Thickness(0, 0, 5, 0), propertyChanged: OnLayoutChanged);

        public Thickness IconMargin
        {
            get => (Thickness)GetValue(IconMarginProperty);
            set => SetValue(IconMarginProperty, value);
        }

        // Optional event
        public event EventHandler Tapped;
        public event EventHandler<string> TappedWithParameter;

        private async void OnTappedInternal(object sender, EventArgs e)
        {
            if (!IsButtonEnabled) return;

            try
            {
                await VisualEventsHelper.TapDimEffect(MainBorder);

                if (TapCommand != null && TapCommand?.CanExecute(CommandParameter) == true)
                    TapCommand.Execute(CommandParameter);

                Tapped?.Invoke(this, EventArgs.Empty);
                TappedWithParameter?.Invoke(this, CommandParameter?.ToString());
            }
            catch (Exception ex)
            {
                // Optional: log error
            }
        }
    }
}
