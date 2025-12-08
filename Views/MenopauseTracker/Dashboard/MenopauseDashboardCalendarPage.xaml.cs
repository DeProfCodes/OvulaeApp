using CommunityToolkit.Maui.Views;
using Newtonsoft.Json;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.MenopauseServices;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.MenopauseTracker;
using OvulaeApp.ViewModels.PeriodTracker;
using OvulaeApp.ViewModels.Shared;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.App;
using OvulaeShared.Models.User;

namespace OvulaeApp.Views.MenopauseTracker.Dashboard
{
    public partial class MenopauseDashboardCalendarPage : ContentPage
    {
        private readonly IMenopauseService _menopauseServ;
        private readonly IUserLocalService _usersServ;

        private MenopauseCalendarPageViewModel viewModel;

        public bool IsNavigating { get; set; }

        public UserCycleProfile userCycleProfile;
        private bool IsEditing = false;

        private DateTime _tempDate;
        private TimeSpan _tempTime;

        public MenopauseDashboardCalendarPage(IMenopauseService menopauseServ, IUserLocalService usersServ)
        {
            InitializeComponent();
            _menopauseServ = menopauseServ;
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
                base.OnAppearing();
                LoadAppointmentData();
                SetCrossComponents();

                viewModel = new MenopauseCalendarPageViewModel(_menopauseServ);
                viewModel.DayDetailsTray = DayDetailsTray;

                BindingContext = viewModel;

                //viewModel.Load();

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
            SideMenu.ConfigureComponents(
                Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch, YesNoPopup
            );
            Header.SetLoaders(Spinner, AppLoader);

            Header.OpenSideMenuCommand = new Command(async () =>
            {
                await SideMenu.OpenAsync();
            });
        }

        private void NextAppointmentDate()
        {
            
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
            if (IsNavigating || viewModel.IsBusy)
                return;

            viewModel.IsBusy = true;
            IsNavigating = true;

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
                IsNavigating = false;
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
