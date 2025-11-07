using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using OvulaeApp.Helpers.Styles;
using System.Collections.Generic;
using System.Linq;

namespace OvulaeApp.Views.Components.Dashboard;

public partial class CategorySelector : ContentView
{
    public event Action<string> CategoryChanged;

    public CategorySelector()
    {
        InitializeComponent();
        this.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Categories) || e.PropertyName == nameof(CategoryString) || e.PropertyName == nameof(IsCategoryFilter) ||
                e.PropertyName == nameof(FontSize) || e.PropertyName == nameof(ActiveCategoryBackgroundColor) || 
                e.PropertyName == nameof(ActiveCategoryTextColor) || e.PropertyName == nameof(InActiveCategoryStrokeColor) ||
                e.PropertyName == nameof(ActiveCategoryStrokeColor) || e.PropertyName == nameof(CategoryCornerRadius) || e.PropertyName == nameof(InActiveCategoryBackgroundColor) ||
                e.PropertyName == nameof(InActiveCategoryTextColor))
                RenderCategories();
        };
    }

    public static readonly BindableProperty CategoriesProperty =
        BindableProperty.Create(nameof(Categories), typeof(List<string>), typeof(CategorySelector), null, propertyChanged: OnItemSpacingChanged);

    public List<string> Categories
    {
        get => (List<string>)GetValue(CategoriesProperty);
        set => SetValue(CategoriesProperty, value);
    }

    public static readonly BindableProperty CategoryStringProperty =
        BindableProperty.Create(nameof(CategoryString), typeof(string), typeof(CategorySelector), null, propertyChanged: OnCategoryStringChanged);

    private static void OnCategoryStringChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CategorySelector)bindable;
        if (newValue is string str)
        {
            control.Categories = str.Split('|').Select(x => x.Trim()).ToList();
        }
    }

    public string CategoryString
    {
        get => (string)GetValue(CategoryStringProperty);
        set => SetValue(CategoryStringProperty, value);
    }

    public static readonly BindableProperty DefaultSelectedIndexProperty =
        BindableProperty.Create(nameof(DefaultSelectedIndex), typeof(int), typeof(CategorySelector), 0);

    public int DefaultSelectedIndex
    {
        get => (int)GetValue(DefaultSelectedIndexProperty);
        set => SetValue(DefaultSelectedIndexProperty, value);
    }

    public static readonly BindableProperty IsCategoryFilterProperty =
        BindableProperty.Create(nameof(IsCategoryFilter), typeof(bool), typeof(CategorySelector), true, propertyChanged: OnItemSpacingChanged);

    public bool IsCategoryFilter
    {
        get => (bool)GetValue(IsCategoryFilterProperty);
        set => SetValue(IsCategoryFilterProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(int), typeof(CategorySelector), 12, propertyChanged: OnItemSpacingChanged);

    public int FontSize
    {
        get => (int)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly BindableProperty CategoryCornerRadiusProperty =
        BindableProperty.Create(nameof(CategoryCornerRadius), typeof(int), typeof(CategorySelector), 5, propertyChanged: OnItemSpacingChanged);

    public int CategoryCornerRadius
    {
        get => (int)GetValue(CategoryCornerRadiusProperty);
        set => SetValue(CategoryCornerRadiusProperty, value);
    }

    public static readonly BindableProperty ActiveCategoryBackgroundColorProperty =
        BindableProperty.Create(nameof(ActiveCategoryBackgroundColor), typeof(Brush), typeof(CategorySelector), OvulaeColors.BRUSH.BrushThemeClr2, propertyChanged: OnItemSpacingChanged);

    public Brush ActiveCategoryBackgroundColor
    {
        get => (Brush)GetValue(ActiveCategoryBackgroundColorProperty);
        set => SetValue(ActiveCategoryBackgroundColorProperty, value);
    }

    public static readonly BindableProperty InActiveCategoryBackgroundColorProperty =
        BindableProperty.Create(nameof(InActiveCategoryBackgroundColor), typeof(Brush), typeof(CategorySelector), Brush.Transparent, propertyChanged: OnItemSpacingChanged);

    public Brush InActiveCategoryBackgroundColor
    {
        get => (Brush)GetValue(InActiveCategoryBackgroundColorProperty);
        set => SetValue(InActiveCategoryBackgroundColorProperty, value);
    }

    public static readonly BindableProperty ActiveCategoryTextColorProperty =
        BindableProperty.Create(nameof(ActiveCategoryTextColor), typeof(Color), typeof(CategorySelector), Colors.White, propertyChanged: OnItemSpacingChanged);

    public Color ActiveCategoryTextColor
    {
        get => (Color)GetValue(ActiveCategoryTextColorProperty);
        set => SetValue(ActiveCategoryTextColorProperty, value);
    }

    public static readonly BindableProperty InActiveCategoryStrokeColorProperty =
        BindableProperty.Create(nameof(InActiveCategoryStrokeColor), typeof(Brush), typeof(CategorySelector), OvulaeColors.BRUSH.BrushThemeClr2, propertyChanged: OnItemSpacingChanged);

    public Brush InActiveCategoryStrokeColor
    {
        get => (Brush)GetValue(InActiveCategoryStrokeColorProperty);
        set => SetValue(InActiveCategoryStrokeColorProperty, value);
    }

    public static readonly BindableProperty ActiveCategoryStrokeColorProperty =
        BindableProperty.Create(nameof(ActiveCategoryStrokeColor), typeof(Brush), typeof(CategorySelector), OvulaeColors.BRUSH.BrushThemeClr2, propertyChanged: OnItemSpacingChanged);

    public Brush ActiveCategoryStrokeColor
    {
        get => (Brush)GetValue(ActiveCategoryStrokeColorProperty);
        set => SetValue(ActiveCategoryStrokeColorProperty, value);
    }

    public static readonly BindableProperty InActiveCategoryTextColorProperty =
        BindableProperty.Create(nameof(InActiveCategoryTextColor), typeof(Color), typeof(CategorySelector), OvulaeColors.COLOR.ThemeClr2, propertyChanged: OnItemSpacingChanged);

    public Color InActiveCategoryTextColor
    {
        get => (Color)GetValue(InActiveCategoryTextColorProperty);
        set => SetValue(InActiveCategoryTextColorProperty, value);
    }

    public static readonly BindableProperty ItemSpacingProperty =
    BindableProperty.Create(
        nameof(ItemSpacing),
        typeof(double),
        typeof(CategorySelector),
        7.0, // default value
        propertyChanged: OnItemSpacingChanged);

    private static void OnItemSpacingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CategorySelector selector && newValue is double spacing)
        {
            selector.CategoryStack.Spacing = spacing;
        }
    }

    public double ItemSpacing
    {
        get => (double)GetValue(ItemSpacingProperty);
        set => SetValue(ItemSpacingProperty, value);
    }

    private int _activeIndex = -1;

    public string ActiveCategory { get; set; }

    private void RenderCategories()
    {
        CategoryStack.Children.Clear();
        if (Categories == null || !Categories.Any()) return;

        for (int i = 0; i < Categories.Count; i++)
        {
            var index = i;
            var text = Categories[i];

            var label = new Label
            {
                Text = text,
                FontSize = FontSize,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = InActiveCategoryTextColor,
                FontAttributes = FontAttributes.None,
                Margin = new Thickness(0)
            };

            var border = new Border
            {
                Padding = new Thickness(12, 2),
                StrokeShape = new RoundRectangle { CornerRadius = CategoryCornerRadius },
                Stroke = InActiveCategoryStrokeColor,
                Background = InActiveCategoryBackgroundColor,
                Content = label
            };

            if (IsCategoryFilter)
            {
                border.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() =>
                    {
                        if (_activeIndex == index) return;
                        SetActive(index);

                        // Fire the local event for non-command usage
                        CategoryChanged?.Invoke(text);

                        // Fire the bound Command for MVVM usage
                        CategoryTappedCommand?.Execute(text);
                    })
                });
            }

            CategoryStack.Children.Add(border);
        }

        if (IsCategoryFilter)
        {
            SetActive(DefaultSelectedIndex);
        }
        else
        {
            foreach (var child in CategoryStack.Children.OfType<Border>())
            {
                child.Background = ActiveCategoryBackgroundColor;
                child.Stroke = ActiveCategoryStrokeColor;
                ((Label)child.Content).TextColor = ActiveCategoryTextColor;
                ((Label)child.Content).FontAttributes = FontAttributes.Bold;
            }
        }
    }

    private void SetActive(int index)
    {
        if (_activeIndex >= 0 && _activeIndex < CategoryStack.Children.Count)
        {
            var old = (Border)CategoryStack.Children[_activeIndex];
            old.Background = Colors.Transparent;
            old.Stroke = InActiveCategoryStrokeColor;
            ((Label)old.Content).TextColor = InActiveCategoryTextColor;
            ((Label)old.Content).FontAttributes = FontAttributes.None;
        }

        var selected = (Border)CategoryStack.Children[index];
        selected.Background = ActiveCategoryBackgroundColor;
        selected.Stroke = ActiveCategoryStrokeColor;
        ((Label)selected.Content).TextColor = ActiveCategoryTextColor;
        ((Label)selected.Content).FontAttributes = FontAttributes.Bold;

        _activeIndex = index;

        ActiveCategory = ((Label)selected.Content).Text;
    }

    public static readonly BindableProperty CategoryTappedCommandProperty =
    BindableProperty.Create(
        nameof(CategoryTappedCommand),
        typeof(Command<string>),
        typeof(CategorySelector),
        null);

    public Command<string> CategoryTappedCommand
    {
        get => (Command<string>)GetValue(CategoryTappedCommandProperty);
        set => SetValue(CategoryTappedCommandProperty, value);
    }
}
