using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.PeriodTracker;
using OvulaeApp.ViewModels.Shared;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.App;
using OvulaeShared.Models.User;
using OvulaeShared.Services.Module.CycleServices;
using OvulaeShared.Services.Module.OvulationServices;
using System;
using System.Threading.Tasks;

namespace OvulaeApp.Views.OvulationTracker.Dashboard
{
    public partial class OvulationDashboardCalendarPage : ContentPage
    {
        private readonly ICycleService _cycleServ;
        private readonly IOvulationService _ovuServ;
        private readonly IUserLocalService _usersServ;

        private PeriodOvulationCalendarPageViewModel viewModel;

        public UserCycleProfile userCycleProfile;
        private bool IsEditing = false;

        private DateTime _tempDate;
        private TimeSpan _tempTime;

        public OvulationDashboardCalendarPage(IOvulationService ovuServ, ICycleService cycleServ, IUserLocalService usersServ)
        {
            InitializeComponent();
            _ovuServ = ovuServ;
            _cycleServ = cycleServ;
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

                BaseTabs.SetLoaders(Spinner, AppLoader);
                SideMenu.ConfigureComponents(Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch, YesNoPopup, MenopauseTrackerOnBoard);
                Header.SetLoaders(Spinner, AppLoader);
                
                Header.OpenSideMenuCommand = new Command(async () =>
                {
                    await SideMenu.OpenAsync();
                });

                var ovulationData = _ovuServ.GetAllPhasesData();

                viewModel = new PeriodOvulationCalendarPageViewModel(_cycleServ, ovulationData, ModuleType.Ovulation, Spinner);
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
                    var yesRes = await YesNoPopup.ShowYesNoModal("Confirm Changes", "Are you sure you want to update your cycle information?");
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
                            await Shell.Current.GoToAsync(nameof(OvulationDashboardCalendarPage));
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
                    LocalStorageService.UserCycleProfile.CycleLengthDays = tempCycle;
                    LocalStorageService.UserCycleProfile.PeriodLengthDays = tempDuration;
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
