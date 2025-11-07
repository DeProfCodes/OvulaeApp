using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Controls.Xaml;
using OvulaeApp.Helpers.Styles;

namespace OvulaeApp.Views.Components.Shared;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class SelectComponent2 : ContentView
{
    public SelectComponent2()
    {
        InitializeComponent();
    }

    // --- Existing Properties ---
    public static readonly BindableProperty OptionListProperty =
        BindableProperty.Create(nameof(OptionList), typeof(string), typeof(SelectComponent2), string.Empty, propertyChanged: OnLayoutTrigger);

    public static readonly BindableProperty OptionSpacingProperty =
        BindableProperty.Create(nameof(OptionSpacing), typeof(double), typeof(SelectComponent2), 10.0, propertyChanged: OnLayoutTrigger);

    public static readonly BindableProperty OptionFontSizeProperty =
        BindableProperty.Create(nameof(OptionFontSize), typeof(double), typeof(SelectComponent2), 16.0, propertyChanged: OnLayoutTrigger);

    public static readonly BindableProperty SelectionTypeProperty =
        BindableProperty.Create(nameof(SelectionType), typeof(SelectionType), typeof(SelectComponent2), SelectionType.Single, propertyChanged: OnLayoutTrigger);

    public static readonly BindableProperty IsHorizontalProperty =
        BindableProperty.Create(nameof(IsHorizontal), typeof(bool), typeof(SelectComponent2), false, propertyChanged: OnLayoutTrigger);

    public static readonly BindableProperty TextHorizontalAlignmentProperty =
        BindableProperty.Create(nameof(TextHorizontalAlignment), typeof(TextAlignment), typeof(SelectComponent2), TextAlignment.Center, propertyChanged: OnLayoutTrigger);

    public static readonly BindableProperty BorderPaddingProperty =
        BindableProperty.Create(nameof(BorderPadding), typeof(Thickness), typeof(SelectComponent2), new Thickness(15, 10), propertyChanged: OnLayoutTrigger);

    public static readonly BindableProperty OptionsHorizontalAlignmentProperty =
        BindableProperty.Create(nameof(OptionsHorizontalAlignment), typeof(LayoutOptions), typeof(SelectComponent2), LayoutOptions.Center, propertyChanged: OnLayoutTrigger);

    public static readonly BindableProperty IsFullWidthProperty =
        BindableProperty.Create(nameof(IsFullWidth), typeof(bool), typeof(SelectComponent2), false, propertyChanged: OnLayoutTrigger);

    public static readonly BindableProperty FixedWidthProperty =
        BindableProperty.Create(nameof(FixedWidth), typeof(bool), typeof(SelectComponent2), false, propertyChanged: OnLayoutTrigger);

    public static readonly BindableProperty OptionCornerRadiusProperty =
        BindableProperty.Create(nameof(OptionCornerRadius), typeof(double), typeof(SelectComponent2), 10.0, propertyChanged: OnLayoutTrigger);

    // --- New Properties ---
    public static readonly BindableProperty ItemsPerRowProperty =
        BindableProperty.Create(nameof(ItemsPerRow), typeof(int), typeof(SelectComponent2), -1, propertyChanged: OnLayoutTrigger);

    public static readonly BindableProperty EnableWrappingProperty =
        BindableProperty.Create(nameof(EnableWrapping), typeof(bool), typeof(SelectComponent2), false, propertyChanged: OnLayoutTrigger);

    // --- Properties ---
    public string OptionList { get => (string)GetValue(OptionListProperty); set => SetValue(OptionListProperty, value); }
    public double OptionSpacing { get => (double)GetValue(OptionSpacingProperty); set => SetValue(OptionSpacingProperty, value); }
    public double OptionFontSize { get => (double)GetValue(OptionFontSizeProperty); set => SetValue(OptionFontSizeProperty, value); }
    public SelectionType SelectionType { get => (SelectionType)GetValue(SelectionTypeProperty); set => SetValue(SelectionTypeProperty, value); }
    public bool IsHorizontal { get => (bool)GetValue(IsHorizontalProperty); set => SetValue(IsHorizontalProperty, value); }
    public TextAlignment TextHorizontalAlignment { get => (TextAlignment)GetValue(TextHorizontalAlignmentProperty); set => SetValue(TextHorizontalAlignmentProperty, value); }
    public Thickness BorderPadding { get => (Thickness)GetValue(BorderPaddingProperty); set => SetValue(BorderPaddingProperty, value); }
    public LayoutOptions OptionsHorizontalAlignment { get => (LayoutOptions)GetValue(OptionsHorizontalAlignmentProperty); set => SetValue(OptionsHorizontalAlignmentProperty, value); }
    public bool IsFullWidth { get => (bool)GetValue(IsFullWidthProperty); set => SetValue(IsFullWidthProperty, value); }
    public bool FixedWidth { get => (bool)GetValue(FixedWidthProperty); set => SetValue(FixedWidthProperty, value); }
    public double OptionCornerRadius { get => (double)GetValue(OptionCornerRadiusProperty); set => SetValue(OptionCornerRadiusProperty, value); }

    public int ItemsPerRow { get => (int)GetValue(ItemsPerRowProperty); set => SetValue(ItemsPerRowProperty, value); }
    public bool EnableWrapping { get => (bool)GetValue(EnableWrappingProperty); set => SetValue(EnableWrappingProperty, value); }

    // --- Public API ---
    public event EventHandler SelectionChanged;
    public int SelectedIndex { get; private set; } = -1;
    public string SelectedValue { get; private set; } = null;
    public List<int> SelectedIndexes { get; private set; } = new();
    public List<string> SelectedValues { get; private set; } = new();

    private static void OnLayoutTrigger(BindableObject bindable, object oldVal, object newVal)
    {
        if (bindable is SelectComponent2 control)
            control.RenderOptions();
    }

    private void RenderOptions()
    {
        OptionsContainer.Children.Clear();

        var items = OptionList?.Split('|') ?? Array.Empty<string>();
        double containerWidth = this.Width;

        if (OptionsContainer is StackLayout stack)
        {
            stack.Orientation = IsHorizontal ? StackOrientation.Horizontal : StackOrientation.Vertical;
        }

        foreach (var (option, index) in items.Select((val, idx) => (val, idx)))
        {
            double itemWidth = -1;

            if (FixedWidth && items.Length > 0)
                itemWidth = this.Width / items.Length;
            else if (ItemsPerRow > 0 && EnableWrapping)
                itemWidth = (this.Width / ItemsPerRow) - OptionSpacing;

            var label = new Label
            {
                Text = option,
                FontSize = OptionFontSize,
                HorizontalTextAlignment = TextHorizontalAlignment,
                TextColor = Colors.Black
            };

            var border = new Border
            {
                Padding = BorderPadding,
                Stroke = OvulaeColors.COLOR.ThemeClrMain,
                StrokeShape = new RoundRectangle { CornerRadius = OptionCornerRadius },
                Content = label,
                Margin = IsHorizontal ? new Thickness(0, 0, OptionSpacing, 0) : new Thickness(0, 0, 0, OptionSpacing),
                HorizontalOptions = (!IsHorizontal && IsFullWidth) ? LayoutOptions.FillAndExpand : OptionsHorizontalAlignment
            };

            if (itemWidth > 0)
                border.WidthRequest = itemWidth;

            var capturedOption = option;
            var capturedIndex = index;

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => OnOptionTapped(capturedOption, capturedIndex);
            border.GestureRecognizers.Add(tapGesture);

            OptionsContainer.Children.Add(border);
        }
    }

    private void OnOptionTapped(string value, int index)
    {
        if (SelectionType == SelectionType.Single)
        {
            SelectedIndex = index;
            SelectedValue = value;

            foreach (var child in OptionsContainer.Children.OfType<Border>())
                SetUnselected(child);

            SetSelected(OptionsContainer.Children[index] as Border);
        }
        else
        {
            var border = OptionsContainer.Children[index] as Border;

            if (SelectedIndexes.Contains(index))
            {
                SelectedIndexes.Remove(index);
                SelectedValues.Remove(value);
                SetUnselected(border);
            }
            else
            {
                SelectedIndexes.Add(index);
                SelectedValues.Add(value);
                SetSelected(border);
            }
        }

        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SetSelected(Border border)
    {
        border.BackgroundColor = OvulaeColors.COLOR.ThemeClr2;
        border.Stroke = OvulaeColors.BRUSH.BrushThemeClr2;

        if (border.Content is Label lbl)
        {
            lbl.TextColor = Colors.White;
            lbl.FontAttributes = FontAttributes.Bold;
        }
    }

    private void SetUnselected(Border border)
    {
        border.BackgroundColor = Colors.Transparent;
        border.Stroke = OvulaeColors.BRUSH.BrushThemeClrMain;

        if (border.Content is Label lbl)
        {
            lbl.TextColor = Colors.Black;
            lbl.FontAttributes = FontAttributes.None;
        }
    }

    public void SelectValue(string valueToSelect, bool deferIfNotReady = true)
    {
        var items = OptionList?.Split('|') ?? Array.Empty<string>();
        var index = Array.IndexOf(items, valueToSelect);

        if (index >= 0)
        {
            if (OptionsContainer.Children.Count == 0 && deferIfNotReady)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SelectValue(valueToSelect, false);
                });
            }
            else if (OptionsContainer.Children.Count > index)
            {
                OnOptionTapped(items[index], index);
            }
        }
    }

    public List<string> GetSelectedValues() => SelectionType == SelectionType.Single ? new() { SelectedValue } : SelectedValues;
    public List<int> GetSelectedIndexes() => SelectionType == SelectionType.Single ? new() { SelectedIndex } : SelectedIndexes;
}
