using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Enums;
using OvulaeShared.Helpers.ModuleHelpers;

namespace OvulaeApp.Views.Components.Modals.Dashboard
{
    public partial class MultiPurposeChangeModal : ContentView
    {
        private ModalUpdateType ChangeType = ModalUpdateType.None;

        private TaskCompletionSource<ModalCloseType> _resultCompletionSource;

        private DateTime LMP = DateTime.MinValue;

        public MultiPurposeChangeModal()
        {
            InitializeComponent();
        }

        private double GetModalHeight(ModalUpdateType changeType)
        {
            switch (changeType)
            {
                case ModalUpdateType.LMP: return 240;

                case ModalUpdateType.CurrentWeekAlong: return 230;
                case ModalUpdateType.PregnancyDueDate: return 240;

                case ModalUpdateType.WeightUpdate: return 270;
                case ModalUpdateType.HeightUpdate: return 270;

                case ModalUpdateType.FirstnameUpdate: return 230;
                case ModalUpdateType.LastnameUpdate: return 230;

                default: return 230;
            }
        }

        private void InitializeSettings()
        {
            try
            {
                if (LMP == DateTime.MinValue)
                    LMP = LocalStorageService.UserCycleProfile.LastPeriodDate.Value;
                
                LMPDate.SelectedDate = LMP;
                WeeksValue.Value = SharedCommonFunctions.GetCurrentPregnancyWeekFromLMP(LMP);
                PregnancyDueDate.SelectedDate = SharedCommonFunctions.GetPregnancyDueDateFromLMP(LMP);

                WeightValue.Value = (int) LocalStorageService.UserBodyMetrics.Weight;
                WeightUnit.DefaultSelectedIndex = LocalStorageService.UserBodyMetrics.WeightUnit.ToLower() == "kg" ? 0 : 1;
                HeightValue.Value = (int) LocalStorageService.UserBodyMetrics.Height;
                HeightUnit.DefaultSelectedIndex = LocalStorageService.UserBodyMetrics.HeightUnit.ToLower() == "cm" ? 0 : 1;

                FirstnameEntry.Text = LocalStorageService.UserDetails.Firstname;
                LastnameEntry.Text = LocalStorageService.UserDetails.Lastname;
            }
            catch
            {
                
            }
        }

        private void ShowSettingsBody(ModalUpdateType changeType)
        {
            //Shared
            LMP_Setting.IsVisible = changeType == ModalUpdateType.LMP;
            
            //Pregnancy
            Weeks_Setting.IsVisible = changeType == ModalUpdateType.CurrentWeekAlong;
            PregnancyDue_Setting.IsVisible = changeType == ModalUpdateType.PregnancyDueDate;

            //Personal
            Firstname_Setting.IsVisible = changeType == ModalUpdateType.FirstnameUpdate;
            Lastname_Setting.IsVisible = changeType == ModalUpdateType.LastnameUpdate;

            //Health
            Weight_Setting.IsVisible = changeType == ModalUpdateType.WeightUpdate;
            Height_Setting.IsVisible = changeType == ModalUpdateType.HeightUpdate;

            MainModalBody.HeightRequest = GetModalHeight(changeType);
            InitializeSettings();
        }

        public Task<ModalCloseType> ShowChangeRequestModal(ModalUpdateType changeType)
        {
            ModalTitle.Text = $"Modify {changeType.GetDisplayGroupName()}";
            
            MainTitle.Text = changeType.GetDisplayShortName();
            MainSubTitle.Text = changeType.GetDisplayDescription();

            ChangeType = changeType;

            ShowSettingsBody(changeType);

            this.IsVisible = true;
            _resultCompletionSource = new TaskCompletionSource<ModalCloseType>();

            this.FadeTo(1, 200);

            return _resultCompletionSource.Task;
        }

        public async Task HideAsync(ModalCloseType result = ModalCloseType.None)
        {
            await this.FadeTo(0, 200);
            this.IsVisible = false;

            _resultCompletionSource?.TrySetResult(result);
        }

        private async void CloseButton_Tapped(object sender, TappedEventArgs e)
        {
            await HideAsync(ModalCloseType.CloseButton);
        }

        private async void CancelButtonTapped(object sender, TappedEventArgs e)
        {
            await HideAsync(ModalCloseType.Reject);
        }

        private async void SubmitButtonTapped(object sender, TappedEventArgs e)
        {
            try
            {
                if (ChangeType.GetDisplayGroupName().ToLower() == "pregnancy")
                {
                    DateTime calculatedLMP = DateTime.Now;

                    if (ChangeType == ModalUpdateType.LMP)
                    {
                        calculatedLMP = LMPDate.SelectedDate;
                    }
                    else if (ChangeType == ModalUpdateType.CurrentWeekAlong)
                    {
                        int currentWeek = WeeksValue.Value;
                        calculatedLMP = DateTime.Today.AddDays(-(currentWeek - 1) * 7);
                    }
                    else if (ChangeType == ModalUpdateType.PregnancyDueDate)
                    {
                        calculatedLMP = PregnancyDueDate.SelectedDate.AddDays(-280);
                    }

                    LocalStorageService.TempCalculatedLMP = calculatedLMP;
                    await HideAsync(ModalCloseType.Accept);
                }
                else if (ChangeType.GetDisplayGroupName().ToLower() == "personal")
                {
                    if (ChangeType == ModalUpdateType.FirstnameUpdate)
                    {
                        LocalStorageService.TempUserDetails.Firstname = FirstnameEntry.Text;
                    }
                    else if (ChangeType == ModalUpdateType.LastnameUpdate)
                    {
                        LocalStorageService.TempUserDetails.Lastname = LastnameEntry.Text;
                    }
                    await HideAsync(ModalCloseType.Accept);
                }
                else if (ChangeType.GetDisplayGroupName().ToLower() == "health")
                {
                    if (ChangeType == ModalUpdateType.WeightUpdate)
                    {
                        LocalStorageService.TempBodyMetrics.Weight = WeightValue.Value;
                        LocalStorageService.TempBodyMetrics.WeightUnit = WeightUnit.ActiveCategory;
                    }
                    else if (ChangeType == ModalUpdateType.HeightUpdate)
                    {
                        LocalStorageService.TempBodyMetrics.Height = HeightValue.Value;
                        LocalStorageService.TempBodyMetrics.HeightUnit = HeightUnit.ActiveCategory;
                    }
                    await HideAsync(ModalCloseType.Accept);
                }
            }
            catch
            {
                await HideAsync(ModalCloseType.Error);
            }
        }

        private void SendOTPbutton(object sender, EventArgs e)
        {

        }

        private void ConfirmOTP(object sender, EventArgs e)
        {

        }
    }
}