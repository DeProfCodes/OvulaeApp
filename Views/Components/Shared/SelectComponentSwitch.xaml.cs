using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using OvulaeApp.Helpers.Styles;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OvulaeApp.Views.Components.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectComponentSwitch : ContentView
    {
        public class SwitchOption : INotifyPropertyChanged
        {
            public string Label { get; set; }

            private bool _isToggled;
            public bool IsToggled
            {
                get => _isToggled;
                set
                {
                    _isToggled = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(ObservableCollection<SwitchOption>), typeof(SelectComponentSwitch), new ObservableCollection<SwitchOption>(), propertyChanged: OnItemsChanged);

        public static readonly BindableProperty SpacingProperty =
            BindableProperty.Create(nameof(Spacing), typeof(double), typeof(SelectComponentSwitch), 10.0);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(SelectComponentSwitch), 16.0);

        public static event EventHandler SelectionChanged;

        public ObservableCollection<SwitchOption> ItemsSource
        {
            get => (ObservableCollection<SwitchOption>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public double Spacing
        {
            get => (double)GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty OnTrackColorProperty =
            BindableProperty.Create(nameof(OnTrackColor), typeof(Color), typeof(SelectComponentSwitch), OvulaeColors.COLOR.ThemeClr2);

        public static readonly BindableProperty OffTrackColorProperty =
            BindableProperty.Create(nameof(OffTrackColor), typeof(Color), typeof(SelectComponentSwitch), OvulaeColors.COLOR.ThemeGray);

        public Color OnTrackColor
        {
            get => (Color)GetValue(OnTrackColorProperty);
            set => SetValue(OnTrackColorProperty, value);
        }

        public Color OffTrackColor
        {
            get => (Color)GetValue(OffTrackColorProperty);
            set => SetValue(OffTrackColorProperty, value);
        }

        public SelectComponentSwitch()
        {
            InitializeComponent();
        }

        private static void OnItemsChanged(BindableObject bindable, object oldVal, object newVal)
        {
            var control = (SelectComponentSwitch)bindable;
            control.BuildSwitches();
        }

        private void BuildSwitches()
        {
            SwitchListLayout.Children.Clear();

            foreach (var item in ItemsSource)
            {
                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = 50 }
                    },
                    Margin = new Thickness(0, 0, 0, 5)
                };

                var label = new Label
                {
                    Text = item.Label,
                    FontSize = FontSize,
                    TextColor = Colors.Black,
                    VerticalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0)
                };
                label.SetBinding(Label.TextProperty, new Binding(nameof(SwitchOption.Label), source: item));

                var toggle = new Switch
                {
                    HorizontalOptions = LayoutOptions.End,
                };
                toggle.SetBinding(Controls.Switch.IsToggledProperty, new Binding(nameof(SwitchOption.IsToggled), source: item));

                grid.Add(label, 0, 0);
                grid.Add(toggle, 1, 0);

                SwitchListLayout.Children.Add(grid);
            }

            // 🔔 Notify listeners
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
