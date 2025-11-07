using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Models.Dashboard.Education;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.DietServices;
using OvulaeApp.Services.LocalDataService.EducationServices;
using OvulaeApp.Services.LocalDataService.PregnancyServices;
using OvulaeApp.ViewModels.PregnancyTracker;
using OvulaeApp.Views.Authentication;
using OvulaeApp.Views.Components.Dashboard;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.Education;
using OvulaeApp.Views.PeriodTracker.Dashboard;
using OvulaeShared.Enums.App;
using OvulaeShared.Models.Diet;
using OvulaeShared.ViewModel.Diet;

namespace OvulaeApp.Views.PregnancyTracker.Dashboard
{
    public partial class PregnancyDashboardHomePage : ContentPage
    {
        private readonly IPregnancyService _pregServ;
        private readonly IDietService _dietServ;
        private readonly IEducationService _eduServ;

        private bool _isNavigating = false;

        private PregnancyDashboardViewModel viewModel;

        public PregnancyDashboardHomePage(IPregnancyService pregServ, IEducationService eduServ)
        {
            InitializeComponent();

            _pregServ = pregServ;
            _eduServ = eduServ;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                viewModel = new PregnancyDashboardViewModel(_pregServ, _eduServ);

                BindingContext = viewModel;

                EducationalBooksListingRow recommendedRow = FindRecommendedRow();
                recommendedRow.CoverItemTapped += OnCoverItemTapped;

                InitializeControls();

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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dashboard page opening failed: {ex.Message}");
            }
        }

        private EducationalBooksListingRow FindRecommendedRow()
        {
            return this.FindByName<EducationalBooksListingRow>("RecommendedRow");
        }

        private async void OnCoverItemTapped(object sender, EducationCover cover)
        {
            if (_isNavigating || viewModel.IsBusy)
                return;

            viewModel.IsBusy = true;
            _isNavigating = true;

            try
            {
                if (cover == null)
                    return;

                Console.WriteLine($"Tapped cover: {cover.Title} (BookId: {cover.BookId})");

                await Spinner.ShowSpinnerAsync();
                await Shell.Current.GoToAsync($"{nameof(EducationDetailsPage)}?bookId={cover.BookId}");
                await Spinner.HideSpinnerAsync();
            }
            catch
            {

            }
            finally
            {
                await Spinner.HideSpinnerAsync();
                viewModel.IsBusy = false;
                _isNavigating = false;
            }
        }

        private async void InitializeControls()
        {
            try
            {
                var name = !string.IsNullOrEmpty(LocalStorageService.UserDetails.Firstname) ? LocalStorageService.UserDetails.Firstname : "Jane";
                //IntroName.Text = $"👋 Hello, {name}!";

                UpdateWeekTexts();

                if (viewModel.Week >= 41)
                {
                    await PregnancyIsComplete();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load pregnancy home page data, error: {ex.Message}");
            }
        }

        private void UpdateWeekTexts()
        {
            try
            {
                PrevWeekText.Text = $"Week {viewModel.Week - 1}";
                CurrentWeek.Text = $"Week {viewModel.Week}";
                NextWeekText.Text = $"Week {viewModel.Week + 1}";

                //WeekRecommendationsText.Text = $"Week {viewModel.Week} Recommendations";
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Failed to load pregnancy home page week data, error: {ex.Message}");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            try
            {
                HandleBackPressedAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async void HandleBackPressedAsync()
        {
            if (_isNavigating || viewModel.IsBusy)
                return;

            _isNavigating = true;
            viewModel.IsBusy = true;

            viewModel.IsBusy = true;
#if ANDROID
            var eixt = await YesNoPopup.ShowYesNoModal("Exit App?", "Do you want to exit the app?");

            if (eixt == ModalCloseType.Accept)
            {
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            }
#endif
            _isNavigating = false;
            viewModel.IsBusy = false;
        }

        private async void ReadMoreBabyInfo_Tapped(object sender, TappedEventArgs e)
        {
            if (_isNavigating || viewModel.IsBusy) 
                return;

            _isNavigating = true;
            viewModel.IsBusy = true;

            try
            {
                await Spinner.ShowSpinnerAsync();

                await Shell.Current.GoToAsync($"{nameof(PregnancyDashboardBabyInfoPage)}?week={viewModel.Week}");

                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load baby development data, error: {ex.Message}");
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
                viewModel.IsBusy = false;
                _isNavigating = false;
            }
        }

        private async void QuickLinks_Tapped(object sender, TappedEventArgs e)
        {
            if (_isNavigating || viewModel.IsBusy) return;

            _isNavigating = true;
            viewModel.IsBusy = true;

            try
            {
                await Spinner.ShowSpinnerAsync();
                
                string navTarget = null;

                if (sender is Border border)
                {
                    if (border == QLinksSymptoms)
                        navTarget = $"{nameof(PregnancyDashboardSymptomsPage)}?week={viewModel.Week}";
                    else if (border == QLinksTips)
                        navTarget = $"{nameof(PregnancyDashboardTipsPage)}?week={viewModel.Week}";
                    else if (border == QLinksPregEd)
                        navTarget = $"{nameof(EducationMainPage)}?moduleName={ModuleType.Pregnancy}";
                    else if (border == QLinkLogDay)
                        navTarget = $"{nameof(PregnancyDashboardDayLoggerPage)}?week={viewModel.Week}";
                }

                if (navTarget != null)
                {
                    await Shell.Current.GoToAsync(navTarget);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load quick links page, error: {ex.Message}");
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
                viewModel.IsBusy = false;
                _isNavigating = false;
            }
        }


        private async void PregEdDetails_Tapped(object sender, TappedEventArgs e)
        {
            if (_isNavigating || viewModel.IsBusy)
                return;

            _isNavigating = true;
            viewModel.IsBusy = true;

            try
            {
                if (sender is Border border && border.BindingContext is EducationCover cover)
                {
                    await Spinner.ShowSpinnerAsync();

                    await Shell.Current.GoToAsync($"{nameof(EducationDetailsPage)}?bookId={cover.BookId}");

                    await Spinner.HideSpinnerAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load pregEd page, error: {ex.Message}");
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
                viewModel.IsBusy = false;
                _isNavigating = false;
            }
        }

        private async void NavigateWeeks_Tapped(object sender, TappedEventArgs e)
        {
            if (_isNavigating || viewModel.IsBusy)
                return;

            _isNavigating = true;
            viewModel.IsBusy = true;

            try
            {
                if (viewModel.Week >= 41)
                {
                    await PregnancyIsComplete();
                }
                if (sender is Border)
                {
                    var btn = sender as Border;
                    if (btn == PrevWeekBtnBrd)
                    {
                        viewModel.GoToPreviousWeek();
                        PrevWeekBtn.IsVisible = viewModel.Week > 0;
                    }
                    else if (btn == NextWeekBtnBrd)
                    {
                        viewModel.GoToNextWeek();
                        NextWeekBtn.IsVisible = viewModel.Week < 45;
                    }
                    UpdateWeekTexts();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to navigate preg data week, error: {ex.Message}");
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
                viewModel.IsBusy = false;
                _isNavigating = false;
            }
        }

        private async Task PregnancyIsComplete()
        {
            if (_isNavigating || viewModel.IsBusy)
                return;

            _isNavigating = true;
            viewModel.IsBusy = true;

            try
            {
                var pregComplete = await YesNoModal.ShowYesNoModal("Pregnancy Complete?", $"You are in Week {viewModel.Week}, is you pregnancy complete and would you like to Log it?");
                if (pregComplete == ModalCloseType.Accept)
                {
                    var response = await PregnancyComplete.ShowOnboardingModal();
                    if (response == ModalCloseType.Accept)
                    {
                        LocalStorageService.PeriodTrackerSet = false;

                        await AppLoader.ShowAsync("Switching to Period Tracker...");

                        await Shell.Current.GoToAsync(nameof(PeriodDashboardHomePage));

                        await AppLoader.HideAsync();
                    }
                    else
                    {
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Not Switched", "Once you are ready to switch to Period Tracker, just click Complete Pregnancy button."));
                    }
                }
            }
            catch
            {
                await AppLoader.HideAsync();
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
                viewModel.IsBusy = false;
                _isNavigating = false;
            }
        }
    }
}
