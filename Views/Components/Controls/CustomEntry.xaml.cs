using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using System;

namespace OvulaeApp.Views.Components.Controls
{
    public partial class CustomEntry : ContentView
    {
        public CustomEntry()
        {
            InitializeComponent();
            UpdateVisibility();
            InternalEntry.TextChanged += (s, e) => OnTextChanged?.Invoke(this, e);
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(CustomEntry), default(string), BindingMode.TwoWay, propertyChanged: OnStateChanged);

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(CustomEntry), default(string));

        public static readonly BindableProperty EntryMarginProperty =
            BindableProperty.Create(nameof(EntryMargin), typeof(Thickness), typeof(CustomEntry), new Thickness(10, 0), propertyChanged: OnStateChanged);

        public static readonly BindableProperty EntryTextColorProperty =
            BindableProperty.Create(nameof(EntryTextColor), typeof(Color), typeof(CustomEntry), Colors.Black, propertyChanged: OnStateChanged);

        public static readonly BindableProperty EntryFontSizeProperty =
            BindableProperty.Create(nameof(EntryFontSize), typeof(double), typeof(CustomEntry), 14.0, propertyChanged: OnStateChanged);

        public static readonly BindableProperty BorderCornerRadiusProperty =
            BindableProperty.Create(nameof(BorderCornerRadius), typeof(double), typeof(CustomEntry), 8.0, propertyChanged: OnStateChanged);

        public static readonly BindableProperty BorderStrokeProperty =
            BindableProperty.Create(nameof(BorderStroke), typeof(Brush), typeof(CustomEntry), Brush.Black, propertyChanged: OnStateChanged);

        public static readonly BindableProperty BorderMarginProperty =
            BindableProperty.Create(nameof(BorderMargin), typeof(Thickness), typeof(CustomEntry), new Thickness(0, 0, 0, 0), propertyChanged: OnStateChanged);

        public static readonly BindableProperty BorderPaddingProperty =
            BindableProperty.Create(nameof(BorderPadding), typeof(Thickness), typeof(CustomEntry), new Thickness(0, 0, 0, 0), propertyChanged: OnStateChanged);

        public static readonly BindableProperty EntryPaddingProperty =
            BindableProperty.Create(nameof(EntryPadding), typeof(Thickness), typeof(CustomEntry), new Thickness(0, 0, 0, 0), propertyChanged: OnStateChanged);

        public static readonly BindableProperty BorderWidthRequestProperty =
            BindableProperty.Create(nameof(BorderWidthRequest), typeof(double), typeof(CustomEntry), 250.0, propertyChanged: OnStateChanged);

        public static readonly BindableProperty BorderHeightRequestProperty =
            BindableProperty.Create(nameof(BorderHeightRequest), typeof(double), typeof(CustomEntry), 40.0, propertyChanged: OnStateChanged);

        public static readonly BindableProperty BorderHorizontalOptionsProperty =
            BindableProperty.Create(nameof(BorderHorizontalOptions), typeof(LayoutOptions), typeof(CustomEntry), LayoutOptions.Start, propertyChanged: OnStateChanged);

        public static readonly BindableProperty EntryKeyboardProperty =
            BindableProperty.Create(nameof(EntryKeyboard), typeof(Keyboard), typeof(CustomEntry), Keyboard.Default, propertyChanged: OnStateChanged);

        public static readonly BindableProperty EntryBackgroundColorProperty =
            BindableProperty.Create(nameof(EntryBackgroundColor), typeof(Brush), typeof(CustomEntry), Brush.Transparent, propertyChanged: OnStateChanged);

        public static readonly BindableProperty IsFullWidthProperty =
        BindableProperty.Create(nameof(IsFullWidth), typeof(bool), typeof(CustomEntry), false, propertyChanged: OnIsFullWidthChanged);


        public bool ShowEntry => !IsReadOnly && IsEntryEnabled;
        public bool ShowLabel => IsReadOnly || !IsEntryEnabled;

        private static void OnStateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomEntry control)
            {
                control.UpdateVisibility();
            }
        }

        private void UpdateVisibility()
        {
            OnPropertyChanged(nameof(ShowEntry));
            OnPropertyChanged(nameof(ShowLabel));
        }


        public bool IsFullWidth
        {
            get => (bool)GetValue(IsFullWidthProperty);
            set => SetValue(IsFullWidthProperty, value);
        }

        private static void OnIsFullWidthChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomEntry entry)
                entry.UpdateWidthLogic();
        }

        private void UpdateWidthLogic()
        {
            if (IsFullWidth)
            {
                BorderHorizontalOptions = LayoutOptions.FillAndExpand;
                BorderWidthRequest = -1;
            }
        }

        public Brush EntryBackgroundColor
        {
            get => (Brush)GetValue(EntryBackgroundColorProperty);
            set => SetValue(EntryBackgroundColorProperty, value);
        }

        public Keyboard EntryKeyboard
        {
            get => (Keyboard)GetValue(EntryKeyboardProperty);
            set => SetValue(EntryKeyboardProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public Thickness EntryMargin
        {
            get => (Thickness)GetValue(EntryMarginProperty);
            set => SetValue(EntryMarginProperty, value);
        }

        public Color EntryTextColor
        {
            get => (Color)GetValue(EntryTextColorProperty);
            set => SetValue(EntryTextColorProperty, value);
        }

        public double EntryFontSize
        {
            get => (double)GetValue(EntryFontSizeProperty);
            set => SetValue(EntryFontSizeProperty, value);
        }

        public double BorderCornerRadius
        {
            get => (double)GetValue(BorderCornerRadiusProperty);
            set => SetValue(BorderCornerRadiusProperty, value);
        }

        public Brush BorderStroke
        {
            get => (Brush)GetValue(BorderStrokeProperty);
            set => SetValue(BorderStrokeProperty, value);
        }

        public Thickness BorderPadding
        {
            get => (Thickness)GetValue(BorderPaddingProperty);
            set => SetValue(BorderPaddingProperty, value);
        }

        public Thickness EntryPadding
        {
            get => (Thickness)GetValue(EntryPaddingProperty);
            set => SetValue(EntryPaddingProperty, value);
        }

        public Thickness BorderMargin
        {
            get => (Thickness)GetValue(BorderMarginProperty);
            set => SetValue(BorderMarginProperty, value);
        }

        public double BorderWidthRequest
        {
            get => (double)GetValue(BorderWidthRequestProperty);
            set => SetValue(BorderWidthRequestProperty, value);
        }

        public double BorderHeightRequest
        {
            get => (double)GetValue(BorderHeightRequestProperty);
            set => SetValue(BorderHeightRequestProperty, value);
        }

        public LayoutOptions BorderHorizontalOptions
        {
            get => (LayoutOptions)GetValue(BorderHorizontalOptionsProperty);
            set => SetValue(BorderHorizontalOptionsProperty, value);
        }

        public static readonly BindableProperty IsReadOnlyProperty =
            BindableProperty.Create(nameof(IsReadOnly), typeof(bool), typeof(CustomEntry), false, propertyChanged: OnStateChanged);

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public static readonly BindableProperty IsEntryEnabledProperty =
            BindableProperty.Create(nameof(IsEntryEnabled), typeof(bool), typeof(CustomEntry), true, propertyChanged: OnStateChanged);

        public bool IsEntryEnabled
        {
            get => (bool)GetValue(IsEntryEnabledProperty);
            set => SetValue(IsEntryEnabledProperty, value);
        }

        public event EventHandler<TextChangedEventArgs> OnTextChanged;
    }
}
