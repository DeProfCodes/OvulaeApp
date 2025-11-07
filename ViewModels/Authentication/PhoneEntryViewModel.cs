using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OvulaeShared.Helpers.CommonFunctions;

namespace OvulaeApp.ViewModels.Authentication
{
    public class PhoneEntryViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> CountryCodes { get; } = new ObservableCollection<string>();

        private string _selectedCountryCode;
        public string SelectedCountryCode
        {
            get => _selectedCountryCode;
            set
            {
                if (_selectedCountryCode != value)
                {
                    _selectedCountryCode = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public PhoneEntryViewModel()
        {
            LoadCountryCodes();
        }

        private void LoadCountryCodes()
        {
            var countryList = CountryCodeFunctions.COUNTRY_CODE_FLAG;

            foreach (var code in countryList)
                CountryCodes.Add(code);

            SelectedCountryCode = CountryCodes.FirstOrDefault(c => c.Contains("+27")); // Default to SA
        }

        private static readonly Dictionary<string, string> CountryCodeToName = CountryCodeFunctions.COUNTRY_CODE_TO_NAME;

        public static string GetCountryNameFromCode(string dialCode)
        {
            if (string.IsNullOrWhiteSpace(dialCode))
                return null;

            return CountryCodeToName.TryGetValue(dialCode.Trim(), out var name) ? name : "Unknown";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
