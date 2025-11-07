using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.ViewModels.Components.Controls
{
    public class YearPickerViewModel
    {
        public ObservableCollection<int> Years { get; set; }

        private bool _isYearPickerVisible;
        public bool IsYearPickerVisible
        {
            get => _isYearPickerVisible;
            set => SetProperty(ref _isYearPickerVisible, value);
        }

        private int _selectedYear;
        public int SelectedYear
        {
            get => _selectedYear;
            set => SetProperty(ref _selectedYear, value);
        }

        public YearPickerViewModel()
        {
            Years = new ObservableCollection<int>();
            for (int year = 2013; year >= 1930; year--)
                Years.Add(year);

            SelectedYear = 2003;
        }

        public void ToggleYearPicker()
        {
            IsYearPickerVisible = !IsYearPickerVisible;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
