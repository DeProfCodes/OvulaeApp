using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.PregnancyServices;
using OvulaeApp.Services.LocalDataService.SymptomsServices;
using OvulaeApp.Services.LocalDataService.TipsServices;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.PregnancyTracker;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.App;

namespace OvulaeApp.Views.PregnancyTracker.Dashboard
{
    public partial class PregnancyDashboardCalendarPage : ContentPage
    {
        private PregnancyCalendarPageViewModel viewModel;
        private IPregnancyService _pregServ;
        private ISymptomsService _symptomsServ;
        private ITipsService _tipsServ;
        private readonly IUserLocalService _usersServ;

        public bool _isNavigating { get; set; }

        public PregnancyDashboardCalendarPage(IPregnancyService pregServ, ITipsService tipsServ, ISymptomsService symptomsServ, IUserLocalService usersServ)
        {
            InitializeComponent();
            
            _pregServ = pregServ;
            _tipsServ = tipsServ;
            _symptomsServ = symptomsServ;
            _usersServ = usersServ;
        }

        protected override void OnAppearing()
        {
            try
            {
                BaseTabs.SetLoaders(Spinner, AppLoader);
                SideMenu.ConfigureComponents(
                     Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, PregnancyComplete, ModuleTrackerSwitch,
                     YesNoModal, MenopauseTrackerOnBoard
                 );
                Header.SetLoaders(Spinner, AppLoader);

                Header.OpenSideMenuCommand = new Command(async () =>
                {
                    await SideMenu.OpenAsync();
                });

                var pregnancyDataList = new PregnancyAllWeeksData
                {
                    PregWeekData = _pregServ.GetAllWeeksPregnancyData(),
                    Symptoms = _symptomsServ.GetModuleAllWeeksData(ModuleType.Pregnancy),
                    Tips = _tipsServ.GetModuleAllWeeksData(ModuleType.Pregnancy)
                };

                var lmp = LocalStorageService.UserCycleProfile.LastPeriodDate.Value;
                (var pregStartDate, DateTime pregConceptionDate) = SharedCommonFunctions.CalculatePregnancyStartDates(lmp);

                DateTime pregnancyStartDate = pregStartDate;


                viewModel = new PregnancyCalendarPageViewModel();
                viewModel.DayDetailsTray = this.DayDetailsTray;

                BindingContext = viewModel;

                viewModel.ApplyPregnancyDataToCalendar(pregnancyDataList, pregnancyStartDate);

                DayDetailsTray.BindingContext = viewModel;

                base.OnAppearing();
            }
            catch
            {
                
            }
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
                // If LoadMonth is async or heavy, consider making VM method async and await here
                await Task.Delay(300); // simulate delay if needed
            }
            catch (Exception ex)
            {
                // Optional: log the error or show a friendly message
                Console.WriteLine($"Navigation failed: {ex.Message}");
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

                var update = false;

                var editRes = await EditPregnancyModal.ShowPregnancyDateEditModal();
                if (editRes == ModalCloseType.Accept)
                {
                    var yesRes = await YesNoModal.ShowYesNoModal("Confirm Changes", "Are you sure you want to update your pregnancy duration information?");
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

                            await AppLoader.ShowAsync("Reloading new Pregnancy Dates");
                            await Shell.Current.GoToAsync(nameof(PregnancyDashboardCalendarPage));
                            await AppLoader.HideAsync();
                        }
                        else
                        {
                            await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed", "Your changes were not saved, please try again.", "Ok"));
                        }
                    }
                }

                if (!update)
                {
                    LocalStorageService.UserCycleProfile.LastPeriodDate = tempLMP;
                }
            }
            catch
            {

            }
        }
    }
}
