using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.Views.Components.Controls;
using OvulaeApp.Views.Components.Controls.Buttons;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OvulaeApp.Views.Components.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectComponentCheck : ContentView
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                nameof(ItemsSource), typeof(IEnumerable<string>), typeof(SelectComponentCheck),
                propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty OptionStringProperty =
            BindableProperty.Create(
                nameof(OptionString), typeof(string), typeof(SelectComponentCheck), string.Empty,
                propertyChanged: OnOptionStringChanged);

        public static readonly BindableProperty IsMultiSelectProperty =
            BindableProperty.Create(nameof(IsMultiSelect), typeof(bool), typeof(SelectComponentCheck), true);

        public static readonly BindableProperty ShowSelectAllButtonProperty =
            BindableProperty.Create(nameof(ShowSelectAllButton), typeof(bool), typeof(SelectComponentCheck), true);

        public static readonly BindableProperty OptionSpacingProperty =
            BindableProperty.Create(nameof(OptionSpacing), typeof(double), typeof(SelectComponentCheck), 10.0);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(SelectComponentCheck), 16.0);

        public static readonly BindableProperty EnableSelectedTextFormatingProperty =
            BindableProperty.Create(nameof(EnableSelectedTextFormating), typeof(bool), typeof(SelectComponentCheck), true);

        public IEnumerable<string> ItemsSource
        {
            get => (IEnumerable<string>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public string OptionString
        {
            get => (string)GetValue(OptionStringProperty);
            set => SetValue(OptionStringProperty, value);
        }

        public bool IsMultiSelect
        {
            get => (bool)GetValue(IsMultiSelectProperty);
            set => SetValue(IsMultiSelectProperty, value);
        }

        public bool ShowSelectAllButton
        {
            get => (bool)GetValue(ShowSelectAllButtonProperty);
            set => SetValue(ShowSelectAllButtonProperty, value);
        }

        public double OptionSpacing
        {
            get => (double)GetValue(OptionSpacingProperty);
            set => SetValue(OptionSpacingProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public bool EnableSelectedTextFormating
        {
            get => (bool)GetValue(EnableSelectedTextFormatingProperty);
            set => SetValue(EnableSelectedTextFormatingProperty, value);
        }

        public event EventHandler SelectionChanged;

        public ObservableCollection<string> SelectedItems { get; } = new();

        private GradientButton _selectAllButton;

        public SelectComponentCheck()
        {
            InitializeComponent();
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SelectComponentCheck checklist && newValue is IEnumerable<string> newItems)
            {
                checklist.BuildOptions(newItems);
            }
        }

        private static void OnOptionStringChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SelectComponentCheck checklist && newValue is string text && !string.IsNullOrWhiteSpace(text))
            {
                var parsedItems = text.Split('|').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s));
                checklist.BuildOptions(parsedItems);
            }
        }

        private void BuildOptions(IEnumerable<string> options)
        {
            OptionsContainer.Children.Clear();
            SelectedItems.Clear();

            foreach (var option in options)
            {
                var checkbox = new Checkbox { Margin = new Thickness(0, 4, 10, 0), WidthRequest = 16, HeightRequest = 16 };
                var label = new Label
                {
                    Text = option,
                    FontSize = FontSize,
                    TextColor = Colors.Black,
                    VerticalOptions = LayoutOptions.Center,
                    LineBreakMode = LineBreakMode.WordWrap,
                };

                var labelTapGesture = new TapGestureRecognizer();
                labelTapGesture.Tapped += (s, e) => checkbox.IsChecked = !checkbox.IsChecked;
                label.GestureRecognizers.Add(labelTapGesture);

                var layout = new HorizontalStackLayout { Spacing = 0 };
                layout.Children.Add(checkbox);
                layout.Children.Add(label);

                checkbox.CheckedChanged += (s, e) =>
                {
                    var cb = s as Checkbox;
                    if (cb == null)
                        return;

                    bool isChecked = cb.IsChecked;

                    label.TextColor = (isChecked && EnableSelectedTextFormating) ? OvulaeColors.COLOR.ThemeClr2 : Colors.Black;
                    label.FontAttributes = (isChecked && EnableSelectedTextFormating) ? FontAttributes.Bold : FontAttributes.None;
                    
                    if (IsMultiSelect)
                    {
                        if (isChecked && !SelectedItems.Contains(option))
                            SelectedItems.Add(option);
                        else if (!isChecked && SelectedItems.Contains(option))
                            SelectedItems.Remove(option);
                    }
                    else
                    {
                        if (isChecked)
                        {
                            SelectedItems.Clear();
                            SelectedItems.Add(option);
                            UncheckOthers(cb);
                        }
                        else
                        {
                            SelectedItems.Remove(option);
                        }
                    }
                    // ✅ Fire SelectionChanged after logic completes
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                };

                OptionsContainer.Children.Add(layout);
            }

            if (ShowSelectAllButton && IsMultiSelect)
            {
                _selectAllButton = new GradientButton
                {
                    Text = "SELECT ALL",
                    GradientColorLeft = Colors.Transparent,
                    GradientColorRight = Colors.Transparent,
                    ButtonTextColor = OvulaeColors.COLOR.ThemeClr2,
                    Stroke = OvulaeColors.COLOR.ThemeClr2,
                    CornerRadius = 12,
                    Margin = new Thickness(0, 10, 0, 0),
                    ButtonPadding = new Thickness(10, 5),
                    WidthRequest = 120,
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold
                };

                _selectAllButton.Tapped += (s, e) => SelectAllItems(options);

                OptionsContainer.Children.Add(_selectAllButton);
            }
        }

        private void UncheckOthers(Checkbox selected)
        {
            foreach (var layout in OptionsContainer.Children.OfType<HorizontalStackLayout>())
            {
                var checkbox = layout.Children.OfType<Checkbox>().FirstOrDefault();
                if (checkbox != null && checkbox != selected)
                    checkbox.IsChecked = false;
            }
        }

        private void SelectAllItems(IEnumerable<string> options)
        {
            SelectedItems.Clear();
            foreach (var layout in OptionsContainer.Children.OfType<HorizontalStackLayout>())
            {
                var checkbox = layout.Children.OfType<Checkbox>().FirstOrDefault();
                var label = layout.Children.OfType<Label>().FirstOrDefault();

                if (checkbox != null)
                {
                    _selectAllButton.Text = !checkbox.IsChecked ? "UNSELECT ALL" : "SELECT ALL";
                    checkbox.IsChecked = !checkbox.IsChecked;
                }
            }
            foreach (var option in options)
            {
                if (!SelectedItems.Contains(option))
                    SelectedItems.Add(option);
            }

            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
