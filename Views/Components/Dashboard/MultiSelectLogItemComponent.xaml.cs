using System.Collections.ObjectModel;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.ViewModels.Dashboard;

namespace OvulaeApp.Views.Components.Dashboard;

public partial class MultiSelectLogItemComponent : ContentView
{
    public ObservableCollection<SelectableItem> Items { get; set; } = new();

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(MultiSelectLogItemComponent), string.Empty, propertyChanged: OnTitleChanged);

    public static readonly BindableProperty TitleTextColorProperty =
        BindableProperty.Create(nameof(TitleTextColor), typeof(Color), typeof(MultiSelectLogItemComponent), Colors.Black);

    public static readonly BindableProperty TitleFontSizeProperty =
        BindableProperty.Create(nameof(TitleFontSize), typeof(double), typeof(MultiSelectLogItemComponent), 14.0);

    public static readonly BindableProperty OptionsProperty =
        BindableProperty.Create(nameof(Options), typeof(string), typeof(MultiSelectLogItemComponent), propertyChanged: OnOptionsChanged);

    public static readonly BindableProperty OptionsSourceProperty =
        BindableProperty.Create(nameof(OptionsSource), typeof(IEnumerable<string>), typeof(MultiSelectLogItemComponent), propertyChanged: OnOptionsSourceChanged);

    public static readonly BindableProperty SelectedColorProperty =
        BindableProperty.Create(nameof(SelectedColor), typeof(Brush), typeof(MultiSelectLogItemComponent), OvulaeColors.BRUSH.BrushThemeClrLight2);

    public static readonly BindableProperty UnselectedColorProperty =
        BindableProperty.Create(nameof(UnselectedColor), typeof(Brush), typeof(MultiSelectLogItemComponent), OvulaeColors.BRUSH.BrushThemeGray);

    public static readonly BindableProperty SelectedTextColorProperty =
        BindableProperty.Create(nameof(SelectedTextColor), typeof(Color), typeof(MultiSelectLogItemComponent), Colors.White);

    public static readonly BindableProperty UnselectedTextColorProperty =
        BindableProperty.Create(nameof(UnselectedTextColor), typeof(Color), typeof(MultiSelectLogItemComponent), Colors.Black);

    public static readonly BindableProperty ItemFontSizeProperty =
        BindableProperty.Create(nameof(ItemFontSize), typeof(double), typeof(MultiSelectLogItemComponent), 14.0);

    public static readonly BindableProperty IsSingleSelectProperty =
    BindableProperty.Create(nameof(IsSingleSelect), typeof(bool), typeof(MultiSelectLogItemComponent), false);

    public bool IsSingleSelect
    {
        get => (bool)GetValue(IsSingleSelectProperty);
        set => SetValue(IsSingleSelectProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public Color TitleTextColor
    {
        get => (Color)GetValue(TitleTextColorProperty);
        set => SetValue(TitleTextColorProperty, value);
    }

    public double TitleFontSize
    {
        get => (double)GetValue(TitleFontSizeProperty);
        set => SetValue(TitleFontSizeProperty, value);
    }

    public string Options
    {
        get => (string)GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
    }

    public IEnumerable<string> OptionsSource
    {
        get => (IEnumerable<string>)GetValue(OptionsSourceProperty);
        set => SetValue(OptionsSourceProperty, value);
    }

    public Brush SelectedColor
    {
        get => (Brush)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }
    public Brush UnselectedColor
    {
        get => (Brush)GetValue(UnselectedColorProperty);
        set => SetValue(UnselectedColorProperty, value);
    }

    public Color SelectedTextColor
    {
        get => (Color)GetValue(SelectedTextColorProperty);
        set => SetValue(SelectedTextColorProperty, value);
    }

    public Color UnselectedTextColor
    {
        get => (Color)GetValue(UnselectedTextColorProperty);
        set => SetValue(UnselectedTextColorProperty, value);
    }

    public double ItemFontSize
    {
        get => (double)GetValue(ItemFontSizeProperty);
        set => SetValue(ItemFontSizeProperty, value);
    }

    public IEnumerable<string> SelectedItems
    {
        get => Items
            .Where(item => item.IsSelected)
            .Select(item => item.Text)
            .ToList();
    }

    public event EventHandler<IEnumerable<string>> SelectionChanged;

    public MultiSelectLogItemComponent()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MultiSelectLogItemComponent control)
        {
            control.TitleLabel.Text = newValue?.ToString();
        }
    }

    private static void OnOptionsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MultiSelectLogItemComponent control && newValue is string options)
        {
            control.Items.Clear();
            foreach (var opt in options.Split('|', StringSplitOptions.RemoveEmptyEntries))
            {
                control.Items.Add(new SelectableItem(opt.Trim(), control));
            }
        }
    }

    private static void OnOptionsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MultiSelectLogItemComponent control && newValue is IEnumerable<string> options)
        {
            control.Items.Clear();
            foreach (var opt in options)
            {
                control.Items.Add(new SelectableItem(opt.Trim(), control));
            }
        }
    }

    public void RaiseSelectionChanged()
    {
        var selectedItems = Items
            .Where(item => item.IsSelected)
            .Select(item => item.Text)
            .ToList();

        SelectionChanged?.Invoke(this, selectedItems);
    }
}
