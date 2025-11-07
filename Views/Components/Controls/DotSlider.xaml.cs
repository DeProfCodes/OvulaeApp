using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Controls.Xaml;
using OvulaeApp.Helpers.Styles;

namespace OvulaeApp.Views.Components.Controls
{
    public partial class DotSlider : ContentView
    {
        public DotSlider()
        {
            InitializeComponent();
            this.SizeChanged += (s, e) => BuildDots();
            BuildDots();
        }

        public static readonly BindableProperty DotCountProperty =
            BindableProperty.Create(
                nameof(DotCount),
                typeof(int),
                typeof(DotSlider),
                0,
                propertyChanged: OnDotPropertyChanged);

        public static readonly BindableProperty ActiveIndexProperty =
            BindableProperty.Create(
                nameof(ActiveIndex),
                typeof(int),
                typeof(DotSlider),
                0,
                propertyChanged: OnDotPropertyChanged);

        public int DotCount
        {
            get => (int)GetValue(DotCountProperty);
            set => SetValue(DotCountProperty, value);
        }

        public int ActiveIndex
        {
            get => (int)GetValue(ActiveIndexProperty);
            set => SetValue(ActiveIndexProperty, value);
        }

        private static void OnDotPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (DotSlider)bindable;
            control.BuildDots();
        }

        public enum SliderVisualMode
        {
            Dots,
            Loader
        }

        public static readonly BindableProperty VisualModeProperty =
            BindableProperty.Create(
                nameof(VisualMode),
                typeof(SliderVisualMode),
                typeof(DotSlider),
                SliderVisualMode.Dots,
                propertyChanged: OnDotPropertyChanged);

        public SliderVisualMode VisualMode
        {
            get => (SliderVisualMode)GetValue(VisualModeProperty);
            set => SetValue(VisualModeProperty, value);
        }

        public static readonly BindableProperty ActiveDotSizeProperty =
            BindableProperty.Create(
                nameof(ActiveDotSize),
                typeof(double),
                typeof(DotSlider),
                15.0,
        propertyChanged: OnDotPropertyChanged);

        public static readonly BindableProperty InactiveDotSizeProperty =
            BindableProperty.Create(
                nameof(InactiveDotSize),
                typeof(double),
                typeof(DotSlider),
                10.0,
                propertyChanged: OnDotPropertyChanged);

        public static readonly BindableProperty ActiveDotColorProperty =
            BindableProperty.Create(
                nameof(ActiveDotColor),
                typeof(Brush),
                typeof(DotSlider),
                OvulaeColors.BRUSH.BrushThemeClrMain,
                propertyChanged: OnDotPropertyChanged);

        public static readonly BindableProperty InactiveDotColorProperty =
            BindableProperty.Create(
                nameof(InactiveDotColor),
                typeof(Brush),
                typeof(DotSlider),
                OvulaeColors.BRUSH.BrushThemeGray,
                propertyChanged: OnDotPropertyChanged);

        public static readonly BindableProperty LoaderHeightProperty =
            BindableProperty.Create(
                nameof(LoaderHeight),
                typeof(double),
                typeof(DotSlider),
                6.0,
                propertyChanged: OnDotPropertyChanged);

        public static readonly BindableProperty LoaderCornerRadiusProperty =
            BindableProperty.Create(
                nameof(LoaderCornerRadius),
                typeof(float),
                typeof(DotSlider),
                3f,
                propertyChanged: OnDotPropertyChanged);

        public static readonly BindableProperty LoaderBackgroundColorProperty =
            BindableProperty.Create(
                nameof(LoaderBackgroundColor),
                typeof(Brush),
                typeof(DotSlider),
                OvulaeColors.BRUSH.BrushThemeGray,
                propertyChanged: OnDotPropertyChanged);

        public static readonly BindableProperty LoaderProgressColorProperty =
            BindableProperty.Create(
                nameof(LoaderProgressColor),
                typeof(Brush),
                typeof(DotSlider),
                OvulaeColors.BRUSH.BrushThemeClrMain,
                propertyChanged: OnDotPropertyChanged);

        public double ActiveDotSize
        {
            get => (double)GetValue(ActiveDotSizeProperty);
            set => SetValue(ActiveDotSizeProperty, value);
        }

        public double InactiveDotSize
        {
            get => (double)GetValue(InactiveDotSizeProperty);
            set => SetValue(InactiveDotSizeProperty, value);
        }

        public Brush ActiveDotColor
        {
            get => (Brush)GetValue(ActiveDotColorProperty);
            set => SetValue(ActiveDotColorProperty, value);
        }

        public Brush InactiveDotColor
        {
            get => (Brush)GetValue(InactiveDotColorProperty);
            set => SetValue(InactiveDotColorProperty, value);
        }

        public double LoaderHeight
        {
            get => (double)GetValue(LoaderHeightProperty);
            set => SetValue(LoaderHeightProperty, value);
        }

        public float LoaderCornerRadius
        {
            get => (float)GetValue(LoaderCornerRadiusProperty);
            set => SetValue(LoaderCornerRadiusProperty, value);
        }

        public Brush LoaderBackgroundColor
        {
            get => (Brush)GetValue(LoaderBackgroundColorProperty);
            set => SetValue(LoaderBackgroundColorProperty, value);
        }

        public Brush LoaderProgressColor
        {
            get => (Brush)GetValue(LoaderProgressColorProperty);
            set => SetValue(LoaderProgressColorProperty, value);
        }


        private void BuildDots()
        {
            DotsContainer.IsVisible = true;
            DotsContainer.Children.Clear();

            for (int i = 0; i < DotCount; i++)
            {
                if (VisualMode == SliderVisualMode.Dots)
                {
                    DotsContainer.Spacing = 10;
                    // Original dots (Ellipse)
                    DotsContainer.Children.Add(new Ellipse
                    {
                        WidthRequest = (i == ActiveIndex) ? ActiveDotSize : InactiveDotSize,
                        HeightRequest = (i == ActiveIndex) ? ActiveDotSize : InactiveDotSize,
                        Fill = (i == ActiveIndex) ? ActiveDotColor : InactiveDotColor,
                        Stroke = Brush.Transparent
                    });
                }
                else if (VisualMode == SliderVisualMode.Loader)
                {
                    DotsContainer.Spacing = 0;

                    DotsContainer.Children.Add(new BoxView
                    {
                        // Fill container equally
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        HeightRequest = LoaderHeight,
                        WidthRequest = InactiveDotSize,
                        CornerRadius = 0, // keep sharp edges for continuous loader
                        Color = (i <= ActiveIndex)
                            ? ((SolidColorBrush)ActiveDotColor).Color
                            : ((SolidColorBrush)InactiveDotColor).Color
                    });
                }
            }
        }
    }
}
