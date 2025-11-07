using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Enums;
using OvulaeShared.Enums.HealthProfile;

namespace OvulaeApp.ViewModels.Settings
{
    public class EditSettingMenopauseViewModel : BaseViewModel
    {
        public EditFieldProperty LastPeriodDate { get; set; }
        public EditFieldProperty IsUsingHormonalTreatment { get; set; }

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

        public EditSettingMenopauseViewModel()
        {
            UpdateFromProfile(false);
        }

        public void UpdateFromProfile(bool isUpdate = false)
        {
            try
            {
                var lmpText =  $"{LocalStorageService.UserCycleProfile.LastPeriodDate:dd MMM yyyy}";
                var hormonalText = LocalStorageService.UserCycleProfile.Treatments.Contains(TreatmentTypes.HRT.GetDisplayDescription()) ? "Yes" : "No";

                if (isUpdate)
                {
                    LastPeriodDate?.Update(lmpText);
                    IsUsingHormonalTreatment?.Update(hormonalText);
                }
                else
                {
                    LastPeriodDate = new EditFieldProperty(lmpText);
                    IsUsingHormonalTreatment = new EditFieldProperty(hormonalText);
                }
                SaveButtonEnabled = isUpdate;
            }
            catch
            {
                // log or ignore
            }
        }
    }
}
