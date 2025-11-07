using System;
using Microsoft.Maui.Controls;

namespace OvulaeApp.Views.Components.Dashboard;

public partial class TipCheckboxCard : ContentView
{
    public TipCheckboxCard()
    {
        InitializeComponent();

        CheckBox.CheckedChanged += OnCheckboxChanged;
    }

    private void OnCheckboxChanged(object sender, bool isChecked)
    {
        TitleLabel.TextColor = isChecked ? CheckedTextColor : TitleTextColor;
        Changed?.Invoke(this, isChecked);
    }

    public event Action<TipCheckboxCard, bool> Changed;

    public static readonly BindableProperty ContainerBGColorProperty =
        BindableProperty.Create(nameof(ContainerBGColor), typeof(Color), typeof(TipCheckboxCard), Colors.LightGray);

    public Color ContainerBGColor
    {
        get => (Color)GetValue(ContainerBGColorProperty);
        set => SetValue(ContainerBGColorProperty, value);
    }

    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(TipCheckboxCard), default(string));

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public static readonly BindableProperty TitleTextColorProperty =
        BindableProperty.Create(nameof(TitleTextColor), typeof(Color), typeof(TipCheckboxCard), Colors.Black);

    public Color TitleTextColor
    {
        get => (Color)GetValue(TitleTextColorProperty);
        set => SetValue(TitleTextColorProperty, value);
    }

    public static readonly BindableProperty CheckedTextColorProperty =
        BindableProperty.Create(nameof(CheckedTextColor), typeof(Color), typeof(TipCheckboxCard), Colors.Green);

    public Color CheckedTextColor
    {
        get => (Color)GetValue(CheckedTextColorProperty);
        set => SetValue(CheckedTextColorProperty, value);
    }

    public static readonly BindableProperty SubTextProperty =
        BindableProperty.Create(nameof(SubText), typeof(string), typeof(TipCheckboxCard), default(string));

    public string SubText
    {
        get => (string)GetValue(SubTextProperty);
        set => SetValue(SubTextProperty, value);
    }

    public static readonly BindableProperty SubTextFontSizeProperty =
        BindableProperty.Create(nameof(SubTextFontSize), typeof(double), typeof(TipCheckboxCard), 12.0);

    public double SubTextFontSize
    {
        get => (double)GetValue(SubTextFontSizeProperty);
        set => SetValue(SubTextFontSizeProperty, value);
    }

    public static readonly BindableProperty SubTextColorProperty =
        BindableProperty.Create(nameof(SubTextColor), typeof(Color), typeof(TipCheckboxCard), Colors.Gray);

    public Color SubTextColor
    {
        get => (Color)GetValue(SubTextColorProperty);
        set => SetValue(SubTextColorProperty, value);
    }

    public static readonly BindableProperty TitleLabelFontSizeProperty =
        BindableProperty.Create(nameof(TitleLabelFontSize), typeof(double), typeof(TipCheckboxCard), 18.0);

    public double TitleLabelFontSize
    {
        get => (double)GetValue(TitleLabelFontSizeProperty);
        set => SetValue(TitleLabelFontSizeProperty, value);
    }

    public static readonly BindableProperty CheckboxSizeProperty =
        BindableProperty.Create(nameof(CheckboxSize), typeof(double), typeof(TipCheckboxCard), 18.0);

    public double CheckboxSize
    {
        get => (double)GetValue(CheckboxSizeProperty);
        set => SetValue(CheckboxSizeProperty, value);
    }

    public static readonly BindableProperty IsCheckedProperty =
        BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(TipCheckboxCard), false, BindingMode.TwoWay);

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

}