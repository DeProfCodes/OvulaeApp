using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.Models;
using OvulaeApp.Models.Dashboard.Education;

namespace OvulaeApp.Views.Components.Dashboard;

public partial class EducationalBooksListing : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(EducationalBooksListing), default(string));

    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(nameof(Items), typeof(List<EducationCover>), typeof(EducationalBooksListing), propertyChanged: OnItemsChanged);

    public static readonly BindableProperty CoverStrokeProperty =
        BindableProperty.Create(nameof(CoverStroke), typeof(Brush), typeof(EducationalBooksListing), OvulaeColors.BRUSH.BrushThemeClr2);

    public static readonly BindableProperty CoverBackgroundProperty =
        BindableProperty.Create(nameof(CoverBackground), typeof(Brush), typeof(EducationalBooksListing), OvulaeColors.BRUSH.BrushThemeClrLight);


    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public List<EducationCover> Items
    {
        get => (List<EducationCover>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public Brush CoverStroke
    {
        get => (Brush)GetValue(CoverStrokeProperty);
        set => SetValue(CoverStrokeProperty, value);
    }

    public Brush CoverBackground
    {
        get => (Brush)GetValue(CoverBackgroundProperty);
        set => SetValue(CoverBackgroundProperty, value);
    }

    public static readonly BindableProperty LabelFontSizeProperty =
    BindableProperty.Create(nameof(LabelFontSize), typeof(double), typeof(EducationalBooksListing), 12.0);

    public static readonly BindableProperty LabelMarginProperty =
        BindableProperty.Create(nameof(LabelMargin), typeof(Thickness), typeof(EducationalBooksListing), new Thickness(5));

    public static readonly BindableProperty LabelTextColorProperty =
        BindableProperty.Create(nameof(LabelTextColor), typeof(Color), typeof(EducationalBooksListing), Colors.Black);

    public static readonly BindableProperty LabelFontAttributesProperty =
        BindableProperty.Create(nameof(LabelFontAttributes), typeof(FontAttributes), typeof(EducationalBooksListing), FontAttributes.Bold);

    public static readonly BindableProperty ScrollPaddingProperty =
        BindableProperty.Create(nameof(ScrollPadding), typeof(Thickness), typeof(EducationalBooksListing), new Thickness(20, 0, 0, 0));

    public double LabelFontSize
    {
        get => (double)GetValue(LabelFontSizeProperty);
        set => SetValue(LabelFontSizeProperty, value);
    }

    public Thickness LabelMargin
    {
        get => (Thickness)GetValue(LabelMarginProperty);
        set => SetValue(LabelMarginProperty, value);
    }

    public Color LabelTextColor
    {
        get => (Color)GetValue(LabelTextColorProperty);
        set => SetValue(LabelTextColorProperty, value);
    }

    public FontAttributes LabelFontAttributes
    {
        get => (FontAttributes)GetValue(LabelFontAttributesProperty);
        set => SetValue(LabelFontAttributesProperty, value);
    }

    public Thickness ScrollPadding
    {
        get => (Thickness)GetValue(ScrollPaddingProperty);
        set => SetValue(ScrollPaddingProperty, value);
    }

    public static readonly BindableProperty CoverTappedCommandProperty =
    BindableProperty.Create(nameof(CoverTappedCommand), typeof(Command<EducationCover>), typeof(EducationalBooksListing));

    public Command<EducationCover> CoverTappedCommand
    {
        get => (Command<EducationCover>)GetValue(CoverTappedCommandProperty);
        set => SetValue(CoverTappedCommandProperty, value);
    }

    public event EventHandler<EducationCover> CoverItemTapped;

    public EducationalBooksListing()
    {
        InitializeComponent();
    }

    private static void OnItemsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EducationalBooksListing view && newValue is List<EducationCover> newItems)
        {
            view.BuildItems(newItems);
        }
    }

    public void Filter(string query)
    {
        if (Items == null) return;

        var filtered = Items
            .Where(x => (!string.IsNullOrWhiteSpace(x.Title) && x.Title.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                        (x.Categories?.Any(c => c.ToString().Contains(query, StringComparison.OrdinalIgnoreCase)) ?? false))
            .ToList();

        BuildItems(filtered);
    }

    private void BuildItems(List<EducationCover> items)
    {
        BookStack.Children.Clear();

        foreach (var item in items)
        {
            var coverItem = new EducationCoverItem
            {
                BindingContext = item,
                Data = item,
                LabelFontSize = LabelFontSize,
                LabelFontAttributes = LabelFontAttributes,
                LabelTextColor = LabelTextColor,
                LabelMargin = LabelMargin,
                CoverStroke = CoverStroke,
                CoverBackground = CoverBackground,
            };

            coverItem.CoverTapped += (s, tappedCover) =>
            {
                CoverItemTapped?.Invoke(this, tappedCover);

                if (CoverTappedCommand?.CanExecute(tappedCover) ?? false)
                    CoverTappedCommand.Execute(tappedCover);
            };

            BookStack.Children.Add(coverItem);
        }
    }
}
