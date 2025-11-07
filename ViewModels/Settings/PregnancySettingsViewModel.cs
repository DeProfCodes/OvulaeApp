using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService;

namespace OvulaeApp.ViewModels.Settings
{    
    public class EditSettingsPregnancySettingsViewModel : BaseViewModel
    {
        public EditFieldProperty LMPDateProp { get; set; }

        public EditFieldProperty PregnancyCurrentWeekProp { get; set; }
        public EditFieldProperty PregnancyDueDateProp { get; set; }

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

        public EditSettingsPregnancySettingsViewModel()
        {
            UpdatePregnancyDetails(LocalStorageService.UserCycleProfile.LastPeriodDate.Value, false);
        }

        public void UpdatePregnancyDetails(DateTime LMP, bool isUpdate = false)
        {
            try
            {
                var LMPDate = $"{LMP: dd MMM yyyy}";
                var week = $"{SharedCommonFunctions.GetCurrentPregnancyWeekFromLMP(LMP)} WEEKS";
                var dueDate = $"{SharedCommonFunctions.GetPregnancyDueDateFromLMP(LMP): dd MMM yyyy}";

                if (isUpdate)
                {
                    LMPDateProp.Update(LMPDate);
                    PregnancyCurrentWeekProp.Update(week);
                    PregnancyDueDateProp.Update(dueDate);
                }
                else
                {
                    LMPDateProp = new EditFieldProperty(LMPDate);
                    PregnancyCurrentWeekProp = new EditFieldProperty(week);
                    PregnancyDueDateProp = new EditFieldProperty(dueDate);
                }
                SaveButtonEnabled = isUpdate;
            }
            catch
            {

            }
        }
    }

}
