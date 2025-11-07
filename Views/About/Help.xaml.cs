using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.ChatBot;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.Chat;
using OvulaeShared.Enums.App;
using OvulaeShared.Services.Email;

namespace OvulaeApp.Views.About
{
    public partial class HelpPage : ContentPage
    {
        private OvulaeFAQChatPageViewModel viewModel;

        private readonly IChatBotService _chatBot;
        private readonly IOvulaeEmailService _emailServ;

        public HelpPage(IChatBotService chatBot, IOvulaeEmailService emailServ)
        {
            InitializeComponent();

            _chatBot = chatBot;
            _emailServ = emailServ;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                viewModel = new OvulaeFAQChatPageViewModel(ChatCollection, _chatBot, _emailServ);
                BindingContext = viewModel;

                viewModel.LoadInitialWelcome();

                BaseTabs.SetLoaders(Spinner, AppLoader);
                SideMenu.ConfigureComponents(
                    Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch, YesNoModal, MenopauseTrackerOnBoard,
                    PregnancyComplete
                );
                Header.SetLoaders(Spinner, AppLoader);

                Header.OpenSideMenuCommand = new Command(async () =>
                {
                    await SideMenu.OpenAsync();
                });

                var module = LocalStorageService.AppPrimaryGoal;
                
                if(module == ModuleType.PeriodTracker) TrackerModule.DefaultSelectedIndex = 1;
                else if (module == ModuleType.Ovulation) TrackerModule.DefaultSelectedIndex = 2;
                else if (module == ModuleType.Pregnancy) TrackerModule.DefaultSelectedIndex = 3;

            }
            catch
            {
                
            }
        }

        private async void OnEmailTapped(object sender, EventArgs e)
        {
            try
            {
                await Launcher.Default.OpenAsync("mailto:support@ovulae.com");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Unable to open email client.", "OK");
            }
        }

        private async void OnWebsiteTapped(object sender, EventArgs e)
        {
            try
            {
                await Launcher.Default.OpenAsync("https://ovulae.com");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Unable to open the website.", "OK");
            }
        }
    }
}
