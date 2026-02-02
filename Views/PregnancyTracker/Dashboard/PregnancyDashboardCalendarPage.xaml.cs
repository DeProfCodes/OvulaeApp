using CommunityToolkit.Maui.Views;
using Newtonsoft.Json;
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
using OvulaeShared.Helpers.ModuleHelpers;
using OvulaeShared.Models.User;

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

        public UserCycleProfile userCycleProfile;
        private bool IsEditing = false;

        private DateTime _tempDate;
        private TimeSpan _tempTime;

        public PregnancyDashboardCalendarPage(IPregnancyService pregServ, ITipsService tipsServ, ISymptomsService symptomsServ, IUserLocalService usersServ)
        {
            InitializeComponent();
            
            _pregServ = pregServ;
            _tipsServ = tipsServ;
            _symptomsServ = symptomsServ;
            _usersServ = usersServ;

            userCycleProfile = LocalStorageService.UserCycleProfile;

            InitializeAppointmentControls();

            // Set up event handlers
            AddNewAppDateBtn.Clicked += OnAddNewAppointmentClicked;
            AddEditAppDateBtn.Clicked += OnEditSaveAppointmentClicked;
            RemoveAppDateBtn.Clicked += OnRemoveAppointmentClicked;

            // Set picker defaults
            AppDatePicker.MinimumDate = DateTime.Today;
            AppDatePicker.Date = DateTime.Today;
            AppTimePicker.Time = new TimeSpan(10, 0, 0); // Default 10 AM
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

                LoadAppointmentData(); 

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

        private void InitializeAppointmentControls()
        {
            // Start with everything hidden
            AppointmentDetailsContainer.IsVisible = false;
            NoAppSet.IsVisible = false;

            // Hide pickers initially
            AppDateContainer.IsVisible = false;
            AppTimeContainer.IsVisible = false;
        }

        private void LoadAppointmentData()
        {
            try
            {
                // Check if appointment exists
                bool hasAppointment = userCycleProfile?.NextAppointmentDate != null &&
                                     userCycleProfile.NextAppointmentDate > DateTime.MinValue;

                if (hasAppointment)
                {
                    ShowAppointmentDetails();
                }
                else
                {
                    ShowNoAppointmentState();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading appointment: {ex.Message}");
                ShowNoAppointmentState();
            }
        }

        private void ShowAppointmentDetails()
        {
            // Show appointment section, hide "no appointment"
            AppointmentDetailsContainer.IsVisible = true;
            NoAppSet.IsVisible = false;

            // Update displays
            UpdateAppointmentDisplays();

            // Start in display mode (not editing)
            ShowDisplayMode();
        }

        private void UpdateAppointmentDisplays()
        {
            if (userCycleProfile?.NextAppointmentDate == null)
                return;

            var appointmentDate = userCycleProfile.NextAppointmentDate.Value;

            // Update date display
            AppDateDisplay.Text = appointmentDate.ToString("dd MMMM yyyy");

            // Update time display if time exists
            if (appointmentDate.TimeOfDay.TotalMinutes > 0)
            {
                AppTimeDisplay.Text = appointmentDate.ToString("hh:mm tt");
            }
            else
            {
                AppTimeDisplay.Text = "No time set";
            }
        }

        private void ShowDisplayMode()
        {
            IsEditing = false;

            // Hide pickers, show labels
            AppDateContainer.IsVisible = false;
            AppTimeContainer.IsVisible = false;
            AppDateDisplay.IsVisible = true;
            AppTimeDisplay.IsVisible = true;

            // Update button text
            AddEditAppDateBtn.Text = "Edit";

            // Show remove button
            RemoveAppDateBtn.IsVisible = true;
        }

        private void ShowEditMode()
        {
            IsEditing = true;

            // Set temp values from current appointment or defaults
            if (userCycleProfile?.NextAppointmentDate != null)
            {
                _tempDate = userCycleProfile.NextAppointmentDate.Value.Date;
                _tempTime = userCycleProfile.NextAppointmentDate.Value.TimeOfDay;
            }
            else
            {
                _tempDate = DateTime.Today.AddDays(7);
                _tempTime = new TimeSpan(10, 0, 0);
            }

            // Update pickers with temp values
            AppDatePicker.Date = _tempDate;
            AppTimePicker.Time = _tempTime;

            // Show pickers, hide labels
            AppDateDisplay.IsVisible = false;
            AppTimeDisplay.IsVisible = false;
            AppDateContainer.IsVisible = true;
            AppTimeContainer.IsVisible = true;

            // Update button text
            AddEditAppDateBtn.Text = "Save";
        }

        private void ShowNoAppointmentState()
        {
            // Show "no appointment" section, hide appointment details
            NoAppSet.IsVisible = true;
            AppointmentDetailsContainer.IsVisible = false;
        }

        private void OnAddNewAppointmentClicked(object sender, EventArgs e)
        {
            // Initialize new appointment
            userCycleProfile ??= new UserCycleProfile();

            // Show appointment details section
            AppointmentDetailsContainer.IsVisible = true;
            NoAppSet.IsVisible = false;

            // Start in edit mode
            ShowEditMode();
        }

        private async void OnEditSaveAppointmentClicked(object sender, EventArgs e)
        {
            if (IsEditing)
            {
                // SAVE mode
                await SaveAppointment();
            }
            else
            {
                // EDIT mode
                ShowEditMode();
            }
        }

        private async void OnRemoveAppointmentClicked(object sender, EventArgs e)
        {
            var confirmed = await DisplayAlert(
                "Remove Appointment",
                "Are you sure you want to remove this appointment?",
                "Yes, Remove",
                "Cancel");

            if (confirmed)
            {
                await RemoveAppointment();
            }
        }

        private async Task SaveAppointment()
        {
            try
            {
                // Get values from pickers
                var selectedDate = AppDatePicker.Date;
                var selectedTime = AppTimePicker.Time;

                // Combine date and time
                var combinedDateTime = selectedDate.Add(selectedTime);

                // Validate date is in future (or today)
                if (combinedDateTime.Date < DateTime.Today)
                {
                    await DisplayAlert("Error", "Appointment date cannot be in the past", "OK");
                    return;
                }

                // Update user profile
                userCycleProfile.NextAppointmentDate = combinedDateTime;
                LocalStorageService.UserCycleProfile.NextAppointmentDate = combinedDateTime;

                await AppLoader.ShowAsync("Saving new appointment date...");

                var json = JsonConvert.SerializeObject(LocalStorageService.UserCycleProfile);
                var success = await _usersServ.UpdateUserCycleProfile();

                await AppLoader.HideAsync();

                if (success)
                {
                    ShowDisplayMode();

                    UpdateAppointmentDisplays();

                    await Shell.Current.ShowPopupAsync(new BrandedAlertPopup("Success", $"Appointment saved for {combinedDateTime:dd MMMM yyyy} at {combinedDateTime:hh:mm tt}", "OK"));
                }
                else
                {
                    await Shell.Current.ShowPopupAsync(new BrandedAlertPopup("Error", "Failed to save appointment", "OK"));
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.ShowPopupAsync(new BrandedAlertPopup("Error", $"Failed to save: {ex.Message}", "OK"));
            }
        }

        private async Task RemoveAppointment()
        {
            try
            {
                // Clear appointment
                userCycleProfile.NextAppointmentDate = null;
                LocalStorageService.UserCycleProfile.NextAppointmentDate = null;

                // Save to database
                await AppLoader.ShowAsync("Removing appointment date...");

                var success = await _usersServ.UpdateUserCycleProfile();

                await AppLoader.HideAsync();

                if (success)
                {
                    // Switch to "no appointment" state
                    ShowNoAppointmentState();

                    await Shell.Current.ShowPopupAsync(new BrandedAlertPopup("Success", "Appointment removed", "OK"));
                }
                else
                {
                    await Shell.Current.ShowPopupAsync(new BrandedAlertPopup("Error", "Failed to remove appointment", "OK"));
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.ShowPopupAsync(new BrandedAlertPopup("Error", $"Failed to remove: {ex.Message}", "OK"));
            }
        }
    }
}
