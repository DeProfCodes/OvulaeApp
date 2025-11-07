using System.Windows.Input;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Views.Components.Modals;

namespace OvulaeApp.Views.Components.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardHeader : ContentView
    {
        public static readonly BindableProperty LeftIconTypeProperty =
            BindableProperty.Create(nameof(LeftIconType), typeof(HeaderLeftIcon), typeof(DashboardHeader),
                HeaderLeftIcon.Menu, propertyChanged: OnLeftIconChanged);

        public static readonly BindableProperty LeftIconCommandProperty =
            BindableProperty.Create(nameof(LeftIconCommand), typeof(ICommand), typeof(DashboardHeader));

        public static readonly BindableProperty RightIconCommandProperty =
            BindableProperty.Create(nameof(RightIconCommand), typeof(ICommand), typeof(DashboardHeader));

        public event EventHandler LeftIconTapped;


        public static readonly BindableProperty OpenSideMenuCommandProperty =
            BindableProperty.Create(nameof(OpenSideMenuCommand), typeof(ICommand), typeof(DashboardHeader), null);

        public ICommand OpenSideMenuCommand
        {
            get => (ICommand)GetValue(OpenSideMenuCommandProperty);
            set => SetValue(OpenSideMenuCommandProperty, value);
        }

        public HeaderLeftIcon LeftIconType
        {
            get => (HeaderLeftIcon)GetValue(LeftIconTypeProperty);
            set => SetValue(LeftIconTypeProperty, value);
        }

        public ICommand LeftIconCommand
        {
            get => (ICommand)GetValue(LeftIconCommandProperty);
            set => SetValue(LeftIconCommandProperty, value);
        }

        public ICommand RightIconCommand
        {
            get => (ICommand)GetValue(RightIconCommandProperty);
            set => SetValue(RightIconCommandProperty, value);
        }

        private SpinnerLoader spinnerLoader;
        private BrandedLoader appLoader;

        public DashboardHeader()
        {
            InitializeComponent();
            UpdateLeftIcon();
        }

        private static void OnLeftIconChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is DashboardHeader header)
                header.UpdateLeftIcon();
        }

        private void UpdateLeftIcon()
        {
            string iconSource = LeftIconType switch
            {
                HeaderLeftIcon.Menu => "menu_icon_white.png",
                HeaderLeftIcon.BackButton => "left_chev_edge_white.png",
                _ => "menu_icon.png"
            };
            LeftIconImage.Source = iconSource;
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

        private async void SettingsIconTapped(object sender, TappedEventArgs e)
        {
            await VisualEventsHelper.TapDimEffectGray(SettingsBtn);
            
            if (appLoader != null)
            {
                await appLoader.ShowAsync(NavigationsHelper.GetLoaderMessage(PageNameTypes.SettingsPage), NavigationsHelper.GetLoaderModalLines(PageNameTypes.SettingsPage));
            }
            else if (spinnerLoader != null)
                await spinnerLoader.ShowSpinnerAsync();

            var pageName = NavigationsHelper.GetCommonAppPagePage(PageNameTypes.SettingsPage);

            await Shell.Current.GoToAsync(pageName);

            if (appLoader != null)
            {
                await appLoader.HideAsync();
            }
            else if (spinnerLoader != null)
                await spinnerLoader.HideSpinnerAsync();

            if (spinnerLoader != null)
                await spinnerLoader.ShowSpinnerAsync();
        }
    }

    public enum HeaderLeftIcon
    {
        Menu,
        BackButton
    }
}
