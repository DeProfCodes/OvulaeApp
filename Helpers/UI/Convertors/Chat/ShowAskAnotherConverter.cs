using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Helpers.UI.Convertors.Chat
{
    public class ShowAskAnotherConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 4 &&
                values[0] is bool isUser &&
                values[1] is bool isFAQVisible &&
                values[2] is bool isTyping &&
                values[3] is bool isFAQContainer)
            {
                return !isUser && !isFAQVisible && !isTyping && isFAQContainer;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
