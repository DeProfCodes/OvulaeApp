using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.ViewModels.GoalSetting
{
    public class BodyMetricsViewModel : INotifyPropertyChanged
    {
        private int _selectedYear = 2005;
        public int SelectedYear
        {
            get => _selectedYear;
            set => SetProperty(ref _selectedYear, value);
        }

        private double _weigthValue;
        public double WeightStepper
        {
            get => _weigthValue;
            set => SetProperty(ref _weigthValue, value);
        }

        private double _heightStepper;
        public double HeightStepper
        {
            get => _heightStepper;
            set => SetProperty(ref _heightStepper, value);
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
