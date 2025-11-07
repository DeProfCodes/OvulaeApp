using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Views.Components.Dashboard;

namespace OvulaeApp.Helpers.UI.Convertors
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var control = parameter as MultiSelectLogItemComponent;
            bool isSelected = value is bool b && b;
            return isSelected ? control?.SelectedColor : control?.UnselectedColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
