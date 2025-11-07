using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Models.Dashboard.Education;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.CycleServices;
using OvulaeApp.Services.LocalDataService.EducationServices;
using OvulaeApp.Services.LocalDataService.OvulationServices;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.Module;
using OvulaeApp.ViewModels.Shared;
using OvulaeApp.Views.Components.Dashboard;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.Education;
using OvulaeApp.Views.PeriodTracker.Dashboard;
using OvulaeApp.Views.PregnancyTracker.Dashboard;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;

namespace OvulaeApp.Views.OvulationTracker.Dashboard
{
    public partial class OvulationDashboardHomePage : ContentPage
    {
        private readonly IUserLocalService _userServ;
        private readonly IEducationService _eduServ;
        private readonly IOvulationService _ovuServ;
        private readonly ICycleService _cylceServ;

        private PeriodOvulationTrackViewModel viewModel;

        private int _carouselPosition = 0;
        private CancellationTokenSource _carouselTimerToken;

        private DateTime _lastAutoSlideTime = DateTime.MinValue;
        private DateTime _lastManualInteraction = DateTime.MinValue;
        private bool _userInteracted = false;

        public bool _isNavigating { get; set; }

        public OvulationDashboardHomePage(IOvulationService ovuServ, IEducationService eduServ, IUserLocalService userServ, ICycleService cylceServ)
        {
            InitializeComponent();

            _userServ = userServ;
            _eduServ = eduServ;
            _ovuServ = ovuServ;
            _cylceServ = cylceServ;
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                var OvulationData = _ovuServ.GetAllOvulationData();
                viewModel = new PeriodOvulationTrackViewModel(_cylceServ, OvulationData, _eduServ, Spinner, ModuleType.Ovulation);
                BindingContext = viewModel;

                BaseTabs.SetLoaders(Spinner, AppLoader);
                SideMenu.ConfigureComponents(Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch, YesNoPopup, MenopauseTrackerOnBoard);
                Header.SetLoaders(Spinner, AppLoader);

                Header.OpenSideMenuCommand = new Command(async () =>
                {
                    await SideMenu.OpenAsync();
                });

                EducationalBooksListingRow recommendedRow = FindRecommendedRow();
                recommendedRow.CoverItemTapped += OnCoverItemTapped;

                _carouselTimerToken = new CancellationTokenSource();

                // Start timer that rotates carousel
                StartAutoCarouselTimer(_carouselTimerToken.Token);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Dashboard page opening failed: {ex.Message}");

                /*
                await AppLoader.ShowAsync("Logging out..");
                await Shell.Current.GoToAsync(nameof(LoginPage));
                await AppLoader.HideAsync();
                */
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

        private void StageTipsCarousel_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            try
            {
                if (_carouselPosition != e.CurrentPosition)
                {
                    _userInteracted = true;
                    _lastManualInteraction = DateTime.Now;
                    _carouselPosition = e.CurrentPosition;
                }
            }
            catch
            {
                
            }
        }

        protected override void OnDisappearing()
        {
            try
            {
                base.OnDisappearing();

                // Cancel the timer
                _carouselTimerToken?.Cancel();
                _carouselTimerToken?.Dispose();
                _carouselTimerToken = null;
            }
            catch
            {
                
            }
        }

        private void StartAutoCarouselTimer(CancellationToken token)
        {
            try
            {
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    if (token.IsCancellationRequested)
                        return false;

                    var now = DateTime.Now;

                    // Wait 60s after user interaction
                    if (_userInteracted && (now - _lastManualInteraction).TotalSeconds < 60)
                        return true;

                    // Ensure 12s has passed since the last auto-slide
                    if ((now - _lastAutoSlideTime).TotalSeconds < 12)
                        return true;

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (StageTipsCarousel?.ItemsSource is IList<ModuleDashboardCard> list && list.Count > 1)
                        {
                            _carouselPosition = (_carouselPosition + 1) % list.Count;
                            StageTipsCarousel.Position = _carouselPosition;

                            _lastAutoSlideTime = DateTime.Now;
                            _userInteracted = false; // resume timer after 60s
                        }
                    });

                    return true;
                });
            }
            catch
            {
                
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
#if ANDROID
            var eixt = await YesNoPopup.ShowYesNoModal("Exit App?", "Do you want to exit the app?");

            if (eixt == ModalCloseType.Accept)
            {
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            }
#endif
        }

        private async void ReadMoreLinkTapped(object sender, TappedEventArgs e)
        {
            if (_isNavigating || viewModel.IsBusy)
                return;

            viewModel.IsBusy = true;
            _isNavigating = true;

            try
            {
                await Spinner.ShowSpinnerAsync();

                await Shell.Current.GoToAsync($"{nameof(OvulationPhaseDetailsPage)}?ovulationPhase={viewModel.OvulationPhase.GetDisplayName()}&datesRange={viewModel.CurrentPhaseDateRangeTitle}");

                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load ovulation details, error: {ex.Message}");
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
                viewModel.IsBusy = false;
                _isNavigating = false;
            }
        }

        private async void LogMyDayTapped(object sender, TappedEventArgs e)
        {
            if (_isNavigating || viewModel.IsBusy)
                return;

            viewModel.IsBusy = true;
            _isNavigating = true;

            try
            {
                await Spinner.ShowSpinnerAsync();

                await Shell.Current.GoToAsync($"{nameof(OvulationDashboardDayLoggerPage)}");

                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load ovulation details, error: {ex.Message}");
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
                viewModel.IsBusy = false;
                _isNavigating = false;
            }
        }

        private async void ImPregnantTapped(object sender, TappedEventArgs e)
        {
            await SideMenu.HandlePregnancyTrackerSwitch();
        }

        private async void SwitchToPeriodTrackerTapped(object sender, TappedEventArgs e)
        {
            if (_isNavigating || viewModel.IsBusy)
                return;

            viewModel.IsBusy = true;
            _isNavigating = true;

            await SideMenu.HandlePeriodOvulationTrackerSwitch(ModuleType.PeriodTracker);

            viewModel.IsBusy = false;
            _isNavigating = false;
        }
    }
}
