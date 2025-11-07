using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Controls.Xaml;
using OvulaeApp.Helpers.Styles;

namespace OvulaeApp.Views.Components.Shared
{
    public enum SelectionType
    {
        Single,
        Multiple
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectComponent : ContentView
    {
        public SelectComponent()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty OptionListProperty =
            BindableProperty.Create(nameof(OptionList), typeof(string), typeof(SelectComponent), string.Empty, propertyChanged: OnOptionListChanged);

        public static readonly BindableProperty OptionSpacingProperty =
            BindableProperty.Create(nameof(OptionSpacing), typeof(double), typeof(SelectComponent), 10.0);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(SelectComponent), 16.0);

        public static readonly BindableProperty SelectionTypeProperty =
            BindableProperty.Create(nameof(SelectionType), typeof(SelectionType), typeof(SelectComponent), SelectionType.Single);

        public string OptionList
        {
            get => (string)GetValue(OptionListProperty);
            set => SetValue(OptionListProperty, value);
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

        public SelectionType SelectionType
        {
            get => (SelectionType)GetValue(SelectionTypeProperty);
            set => SetValue(SelectionTypeProperty, value);
        }

        public static readonly BindableProperty OptionsSourceProperty =
            BindableProperty.Create(nameof(OptionsSource), typeof(IEnumerable<string>), typeof(SelectComponent), default(IEnumerable<string>), propertyChanged: OnOptionsSourceChanged);

        public IEnumerable<string> OptionsSource
        {
            get => (IEnumerable<string>)GetValue(OptionsSourceProperty);
            set => SetValue(OptionsSourceProperty, value);
        }

        public event EventHandler SelectionChanged;

        public int SelectedIndex { get;  set; } = -1;
        public string SelectedValue { get; set; } = "";
        public List<int> SelectedIndexes { get; private set; } = new();
        public List<string> SelectedValues { get; private set; } = new();

        private static void OnOptionsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SelectComponent)bindable;
            control.RenderOptions();
        }

        private static void OnOptionListChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SelectComponent)bindable;
            control.RenderOptions();
        }

        private void RenderOptions()
        {
            OptionsContainer.Children.Clear();

            var options = OptionsSource?.ToList() ?? OptionList.Split('|').ToList();

            for (int index = 0; index < options.Count; index++)
            {
                var option = options[index];
                var border = new Border
                {
                    Padding = new Thickness(15, 10),
                    Stroke = OvulaeColors.COLOR.ThemeClrMain,
                    StrokeShape = new RoundRectangle { CornerRadius = 10 },
                    Content = new Label
                    {
                        Text = option,
                        FontSize = FontSize,
                        TextColor = Colors.Black
                    },
                    Margin = new Thickness(0, 0, 0, OptionSpacing)
                };

                var tapGesture = new TapGestureRecognizer();
                var capturedOption = option;
                var capturedIndex = index;
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
                {
                    SetUnselected(child);
                }
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

            // 🔔 Notify listeners
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

        public List<string> GetSelectedValues() => SelectionType == SelectionType.Single ? new() { SelectedValue } : SelectedValues;
        public List<int> GetSelectedIndexes() => SelectionType == SelectionType.Single ? new() { SelectedIndex } : SelectedIndexes;
    }
}
