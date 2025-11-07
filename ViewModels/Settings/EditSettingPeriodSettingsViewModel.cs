using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Models.User;

namespace OvulaeApp.ViewModels.Settings
{    
    public class EditSettingPeriodSettingsViewModel : BaseViewModel
    {
        public EditFieldProperty LMPDateProp { get; set; }

        public EditFieldProperty CycleLength { get; set; }
        public EditFieldProperty PeriodDuration { get; set; }

        private bool _saveButtonEnabled;
        public bool SaveButtonEnabled
        {
            get => _saveButtonEnabled;
            set
            {
                _saveButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        public EditSettingPeriodSettingsViewModel()
        {
            UpdatePeriodDetails(LocalStorageService.UserCycleProfile, false);
        }

        public void UpdatePeriodDetails(UserCycleProfile userCycle, bool isUpdate = false)
        {
            try
            {
                if (isUpdate)
                {
                    LMPDateProp.Update($"{userCycle.LastPeriodDate: dd MMM yyyy}");
                    CycleLength.Update($"{userCycle.CycleLengthDays} Days");
                    PeriodDuration.Update($"{userCycle.PeriodLengthDays} Days");
                }
                else
                {
                    LMPDateProp = new EditFieldProperty($"{userCycle.LastPeriodDate: dd MMM yyyy}");
                    CycleLength = new EditFieldProperty($"{userCycle.CycleLengthDays} Days");
                    PeriodDuration = new EditFieldProperty($"{userCycle.PeriodLengthDays} Days");
                }
                SaveButtonEnabled = isUpdate;
            }
            catch
            {

            }
        }
    }

}
