using Microsoft.Maui.Controls.Shapes;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums;

namespace OvulaeApp.Views.Components.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardBottomTab : ContentView
    {
        public static readonly BindableProperty ActiveTabProperty =
            BindableProperty.Create(nameof(ActiveTab), typeof(DashboardTab), typeof(DashboardBottomTab),
                DashboardTab.Home, propertyChanged: OnTabChanged);

        public DashboardTab ActiveTab
        {
            get => (DashboardTab)GetValue(ActiveTabProperty);
            set => SetValue(ActiveTabProperty, value);
        }

        private SpinnerLoader spinnerLoader;
        private BrandedLoader appLoader;

        public DashboardBottomTab()
        {
            InitializeComponent();
            RenderTabs();
        }

        private static void OnTabChanged(BindableObject bindable, object oldVal, object newVal)
        {
            if (bindable is DashboardBottomTab dash)
                dash.RenderTabs();
        }

        public void SetSpinnerLoader(SpinnerLoader spinnerLoader)
        {
            this.spinnerLoader = spinnerLoader;
        }

        public void SetAppLoader(BrandedLoader appLoader)
        {
            this.appLoader = appLoader;
        }

        public void SetLoaders(SpinnerLoader spinnerLoader, BrandedLoader appLoader)
        {
            this.spinnerLoader = spinnerLoader;
            this.appLoader = appLoader;
        }

        private void RenderTabs()
        {
            TabContainer.Children.Clear();
            TabContainer.ColumnDefinitions.Clear();

            var tabs = new List<DashboardTab> { DashboardTab.Home, DashboardTab.Track, DashboardTab.Learn, DashboardTab.Alerts, DashboardTab.Profile };

            for (int i = 0; i < tabs.Count; i++)
            {
                TabContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                var tab = tabs[i];
                bool isActive = tab == ActiveTab;
                string fileName = $"{tab.GetDisplayShortName()}{(isActive ? "_active" : "")}_icon.png";
                string iconPath = $"{fileName}";

                var image = new Image
                {
                    Source = iconPath,
                    WidthRequest = 25,
                    HeightRequest = 25,
                    Aspect = Aspect.AspectFit,
                    HorizontalOptions = LayoutOptions.Center
                };

                var label = new Label
                {
                    Text = tab.ToString(),
                    FontSize = 12,
                    FontFamily = "Arial Black",
                    FontAttributes = isActive ? FontAttributes.Bold : FontAttributes.None,
                    TextColor = isActive ? Color.FromArgb("#CB6CE6") : Colors.Black,
                    HorizontalOptions = LayoutOptions.Center
                };

                var layout = new VerticalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Center
                };
                layout.Children.Add(image);
                layout.Children.Add(label);

                var border = new Border
                {
                    Padding = new Thickness(10, 5),
                    Stroke = Colors.Transparent,
                    StrokeShape = new RoundRectangle { CornerRadius = 10 },
                    Content = layout,
                    ClassId = tab.GetDisplayName()
                };

                var capturedTab = tab;
                border.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => OnTabTapped(capturedTab))
                });

                Grid.SetColumn(border, i);
                TabContainer.Children.Add(border);
            }
        }


        private async void OnTabTapped(DashboardTab tappedTab)
        {
            var targetBorder = TabContainer.Children.OfType<Border>().FirstOrDefault(b => b.ClassId == tappedTab.GetDisplayName());
            VisualEventsHelper.TapDimEffectPurple(targetBorder);

            var pageNameType = NavigationsHelper.GetTabToPageNameType(tappedTab);
            if (appLoader != null)
            {
                await appLoader.ShowAsync(NavigationsHelper.GetLoaderMessage(pageNameType), NavigationsHelper.GetLoaderModalLines(pageNameType));
            }
            else if (spinnerLoader != null)
                await spinnerLoader.ShowSpinnerAsync();

            var pageName = NavigationsHelper.GetCommonAppPagePage(pageNameType);

            await Shell.Current.GoToAsync(pageName);

            if (appLoader != null)
            {
                await appLoader.HideAsync();
            }
            else if(spinnerLoader != null)
                await spinnerLoader.HideSpinnerAsync();
        }
    }
}
