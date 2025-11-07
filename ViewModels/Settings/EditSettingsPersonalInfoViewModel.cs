using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Enums;
using OvulaeShared.Enums.Status;
using OvulaeShared.Models.User;

namespace OvulaeApp.ViewModels.Settings
{
    public class EditSettingsPersonalInfoViewModel : BaseViewModel
    {
        private int _selectedYear = LocalStorageService.UserBodyMetrics.Year;
        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                SetProperty(ref _selectedYear, value);
                if (BirthYearProp != null)
                {
                    BirthYearProp.Update($"{_selectedYear}");
                }
            }
        }
        public EditFieldProperty BirthYearProp { get; set; }
        public EditFieldProperty FirstnameProp { get; set; }
        public EditFieldProperty LastnameProp { get; set; }

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

        public EditSettingsPersonalInfoViewModel()
        {
            UpdatePersonalInformation(LocalStorageService.UserDetails, ModalUpdateType.None, false);
        }

        public void UpdatePersonalInformation(UserModel userInfo, ModalUpdateType updateType, bool isUpdate = false)
        {
            try
            {
                if (!isUpdate)
                {
                    SelectedYear = LocalStorageService.UserBodyMetrics.Year;

                    FirstnameProp = new EditFieldProperty(userInfo.Firstname);
                    LastnameProp = new EditFieldProperty(userInfo.Lastname);
                    BirthYearProp = new EditFieldProperty($"{SelectedYear}");
                }
                else
                {
                    if (updateType == ModalUpdateType.All || updateType == ModalUpdateType.AgeUpdate)
                    {
                        BirthYearProp.Update($"{SelectedYear}");
                    }
                    else if (updateType == ModalUpdateType.All || updateType == ModalUpdateType.FirstnameUpdate)
                    {
                        FirstnameProp.Update(userInfo.Firstname);
                    }
                    else if (updateType == ModalUpdateType.All || updateType == ModalUpdateType.LastnameUpdate)
                    {
                        LastnameProp.Update(userInfo.Lastname);
                    }
                }
                SaveButtonEnabled = isUpdate;
            }
            catch
            {

            }
        }
    }
}
