using System;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.CycleServices;
using OvulaeApp.Services.LocalDataService.PeriodTrackerServices;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.PeriodTracker;
using OvulaeApp.ViewModels.Shared;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.App;

namespace OvulaeApp.Views.PeriodTracker.Dashboard
{
    public partial class PeriodDashboardCalendarPage : ContentPage
    {
        private readonly IPeriodTrackerService _periodServ;
        private readonly ICycleService _cycleServ;
        private readonly IUserLocalService _usersServ;

        private PeriodOvulationCalendarPageViewModel viewModel;

        public bool _isNavigating { get; set; }

        public PeriodDashboardCalendarPage(IPeriodTrackerService periodServ, ICycleService cycleServ, IUserLocalService usersServ)
        {
            InitializeComponent();
            _periodServ = periodServ;
            _cycleServ = cycleServ;
            _usersServ = usersServ;
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                SetCrossComponents();

                var periodData = _periodServ.GetAllPhaseData();

                viewModel = new PeriodOvulationCalendarPageViewModel(_cycleServ, periodData, ModuleType.PeriodTracker, Spinner);
                viewModel.DayDetailsTray = DayDetailsTray;

                BindingContext = viewModel;

                viewModel.ApplyModuleDataToCalendar();

                DayDetailsTray.BindingContext = viewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnAppearing: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Failed to load page.", "OK");
            }
        }

        private void SetCrossComponents()
        {
            BaseTabs.SetLoaders(Spinner, AppLoader);
            SideMenu.ConfigureComponents(Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch, YesNoModal, MenopauseTrackerOnBoard);
            Header.SetLoaders(Spinner, AppLoader);

            Header.OpenSideMenuCommand = new Command(async () =>
            {
                await SideMenu.OpenAsync();
            });
        }

        private async void PreviousMonth_Tapped(object sender, TappedEventArgs e)
        {
            await NavigateWithSpinnerAsync(() => viewModel.GoPreviousMonth());
        }

        private async void NextMonth_Tapped(object sender, TappedEventArgs e)
        {
            await NavigateWithSpinnerAsync(() => viewModel.GoNextMonth());
        }

        private async Task NavigateWithSpinnerAsync(Action navigationAction)
        {
            if (_isNavigating || viewModel.IsBusy)
                return;

            viewModel.IsBusy = true;
            _isNavigating = true;

            try
            {
                await Spinner.ShowSpinnerAsync();
                navigationAction.Invoke();
                await Task.Delay(100); // smooth animation
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failed: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Navigation failed. Please try again.", "OK");
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
                viewModel.IsBusy = false;
                _isNavigating = false;
            }
        }

        private async void EditCycleDaysTapped(object sender, EventArgs e)
        {
            try
            {
                var tempLMP = LocalStorageService.UserCycleProfile.LastPeriodDate.Value;
                var tempCycle = LocalStorageService.UserCycleProfile.CycleLengthDays.Value;
                var tempDuration = LocalStorageService.UserCycleProfile.PeriodLengthDays.Value;

                var update = false;

                var editRes = await EditCyclesModal.ShowEditCycleDaysModal();
                if (editRes == ModalCloseType.Accept)
                {
                    var yesRes = await YesNoModal.ShowYesNoModal("Confirm Changes", "Are you sure you want to update your cycle information?");
                    if (yesRes == ModalCloseType.Accept)
                    {
                        await AppLoader.ShowAsync("Saving your changes...");

                        var updateRes = await _usersServ.UpdateUserCycleProfile();

                        await AppLoader.HideAsync();

                        if (updateRes)
                        {
                            var popup = new BrandedAlertPopup("Saved", "Your changes have been saved", "Ok");
                            await Shell.Current.CurrentPage.ShowPopupAsync(popup);

                            popup.ShowWithResultAsync();

                            update = true;

                            await AppLoader.ShowAsync("Reloading new Cycle Dates");
                            await Shell.Current.GoToAsync(nameof(PeriodDashboardCalendarPage));
                            await AppLoader.HideAsync();
                        }
                        else
                        {
                            await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed", "Your changes were not saved, please try again.", "Ok"));
                        } 
                    }
                }

                if(!update)
                {
                    LocalStorageService.UserCycleProfile.LastPeriodDate = tempLMP;
                    LocalStorageService.UserCycleProfile.CycleLengthDays = tempCycle;
                    LocalStorageService.UserCycleProfile.PeriodLengthDays = tempDuration;
                }
            }
            catch
            {
                
            }
        }
    }
}
