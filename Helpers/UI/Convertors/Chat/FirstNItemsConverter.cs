using System.Globalization;

namespace OvulaeApp.Helpers.UI.Convertors.Chat
{
    public class FirstNItemsMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is IEnumerable<object> items && values[1] is int count)
            {
                return items.Take(count).ToList();
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
