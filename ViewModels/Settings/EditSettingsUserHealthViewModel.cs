using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Helpers.ModuleHelpers;
using OvulaeShared.Models.User;

namespace OvulaeApp.ViewModels.Settings
{
    public class EditSettingsUserHealthViewModel : BaseViewModel
    {
        //Health
        public EditFieldProperty WeightProp { get; set; }
        public EditFieldProperty HeightProp { get; set; }
        public EditFieldProperty BMIProp { get; set; }
        public EditFieldProperty BloodTypeProp { get; set; }

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

        public EditSettingsUserHealthViewModel()
        {
            UpdateHealthInformation(LocalStorageService.UserBodyMetrics, ModalUpdateType.None, false);
        }

        public void UpdateHealthInformation(UserBodyMetric bodyMetric, ModalUpdateType updateType, bool isUpdate = false)
        {
            try
            {
                if (!isUpdate)
                {
                    WeightProp = new EditFieldProperty($"{bodyMetric.Weight} {bodyMetric.WeightUnit}");
                    HeightProp = new EditFieldProperty($"{bodyMetric.Height} {bodyMetric.HeightUnit}");
                    BMIProp = new EditFieldProperty($"{SharedCommonFunctions.CalculateBMI(bodyMetric)}");
                    BloodTypeProp = new EditFieldProperty($"{bodyMetric.BloodGroup}{bodyMetric.RhFactor}");
                }
                else
                {
                    if (updateType == ModalUpdateType.All || updateType == ModalUpdateType.WeightUpdate)
                    {
                        WeightProp.Update($"{bodyMetric.Weight} {bodyMetric.WeightUnit}");
                    }
                    else if (updateType == ModalUpdateType.All || updateType == ModalUpdateType.HeightUpdate)
                    {
                        HeightProp.Update($"{bodyMetric.Height} {bodyMetric.HeightUnit}");
                    }
                    else if (updateType == ModalUpdateType.All || updateType == ModalUpdateType.BloodTypeUpdate)
                    {
                        BloodTypeProp.Update($"{bodyMetric.BloodGroup}{bodyMetric.RhFactor}");
                    }
                    BMIProp.Update($"{SharedCommonFunctions.CalculateBMI(bodyMetric)}");
                }
                SaveButtonEnabled = isUpdate;
            }
            catch
            {

            }
        }
    }

   
}
