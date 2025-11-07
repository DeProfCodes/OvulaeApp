using System.Net.Mail;
using System.Reflection.PortableExecutable;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.ApplicationModel.Communication;
using OvulaeApp.Helpers.Constants;
using OvulaeApp.Helpers.Controls;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Helpers.Pages.Authentication;
using OvulaeApp.Services;

using OvulaeApp.Services.LocalDataService.PregnancyServices;
using OvulaeApp.Services.LocalDataService.SymptomsServices;
using OvulaeApp.Services.UserInterface.Components;
using OvulaeApp.ViewModels.PregnancyTracker;
using OvulaeApp.ViewModels.Shared;
using OvulaeApp.Views.Components.Modals;

namespace OvulaeApp.Views.PregnancyTracker.Dashboard
{
    [QueryProperty(nameof(Week), "week")]
    public partial class PregnancyDashboardSymptomsPage : ContentPage
    {
        private readonly ISymptomsService _symptomsServ;
        private PregnancySymptomsViewModel viewModel;
        public int Week { get; set; }


        public PregnancyDashboardSymptomsPage(ISymptomsService symptomsServ)
        {
            InitializeComponent();

            _symptomsServ = symptomsServ;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = LoadPageAsync();
        }

        private async Task LoadPageAsync()
        {
            try
            {
                await Spinner.ShowSpinnerAsync();

                if (viewModel == null && Week > 0)
                {
                    viewModel = new PregnancySymptomsViewModel(_symptomsServ, Week);
                    await viewModel.InitializeAsync();

                    await Task.Delay(50);
                    BindingContext = viewModel;

                    // Setup UI components
                    BaseTabs.SetLoaders(Spinner, AppLoader);
                    SideMenu.ConfigureComponents(
                        Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, PregnancyComplete, ModuleTrackerSwitch,
                        YesNoModal, MenopauseTrackerOnBoard
                    );
                    Header.SetLoaders(Spinner, AppLoader);
                    Header.OpenSideMenuCommand = new Command(async () => await SideMenu.OpenAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open symptoms page, error: {ex.Message}");
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
            }
        }

    }
}
