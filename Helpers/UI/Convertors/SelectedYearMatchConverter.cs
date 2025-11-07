using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Views.Components.Controls;

namespace OvulaeApp.Helpers.UI.Convertors
{
    public class SelectedYearMatchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is YearPicker picker && value is int year)
            {
                return picker.SelectedYear == year;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
